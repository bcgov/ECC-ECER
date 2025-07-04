﻿using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace ECER.Utilities.DataverseSdk.Queries;

public class Join2QueryExpressionBuilder<TEntity> : QueryExpressionBuilder<TEntity>
  where TEntity : Entity
{
  protected abstract record JoinData;
  protected sealed record OneToManyJoinData(Type EnityType, string RelationshipSchemaName, string RelatedLogicalEntityName, string RelatedEntityForeignKeyAttributeName) : JoinData;
  protected sealed record ManyToOneJoinData(Type EnityType, string RelationshipSchemaName, string RelatedLogicalEntityName, string KeyAttributeName, string RelatedEntityForeignKeyAttributeName) : JoinData;

  protected Collection<JoinData> Joins { get; } = [];

  public Join2QueryExpressionBuilder(QueryExpressionBuilder<TEntity> parentBuilder)
    : base(parentBuilder)
  {
    Joins = parentBuilder is Join2QueryExpressionBuilder<TEntity> join ? join.Joins : [];
  }

  public NestedJoin2QueryExpressionBuilder<TEntity, TRelatedEntity> Include<TRelatedEntity>([NotNull] Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> selector)
    where TRelatedEntity : Entity
  {
    var relatedEntityType = typeof(TRelatedEntity);
    var entityType = typeof(TEntity);

    var propertyName = ((MemberExpression)selector.Body).Member.Name;
    Joins.Add(CreateOneToMany(entityType, relatedEntityType, propertyName));

    return new NestedJoin2QueryExpressionBuilder<TEntity, TRelatedEntity>(this);
  }

  public NestedJoin2QueryExpressionBuilder<TEntity, TRelatedEntity> Include<TRelatedEntity>([NotNull] Expression<Func<TEntity, TRelatedEntity>> selector)
  where TRelatedEntity : Entity
  {
    var relatedEntityType = typeof(TRelatedEntity);
    var entityType = typeof(TEntity);

    var propertyName = ((MemberExpression)selector.Body).Member.Name;
    Joins.Add(CreateManyToOne(entityType, relatedEntityType, propertyName));

    return new NestedJoin2QueryExpressionBuilder<TEntity, TRelatedEntity>(this);
  }

  public override IEnumerable<TEntity> Execute()
  {
    var rootEntities = base.Execute().ToList();
    if (rootEntities.Count != 0)
    {
      var keys = rootEntities.Select(e => e.Id).ToArray();
      var relatedEntitiesMap = new Dictionary<JoinData, EntityCollection>();
      var allEntities = new List<Microsoft.Xrm.Sdk.Entity>();
      allEntities.AddRange(rootEntities);
      foreach (var join in Joins)
      {
        var entities = join switch
        {
          OneToManyJoinData j =>
          rootEntities.Any(e => e.GetType() == j.EnityType) ?
          QuerySubEntities(j.RelatedLogicalEntityName, j.RelatedEntityForeignKeyAttributeName, keys) :
          QuerySubEntities(j.RelatedLogicalEntityName, j.RelatedEntityForeignKeyAttributeName,
          allEntities.Any(e => e.GetType() == j.EnityType) ? allEntities.Where(e => e.GetType() == j.EnityType).Select(i => i.Id).ToArray() : Array.Empty<Guid>()
          ),

          ManyToOneJoinData j => QuerySubEntities(j.RelatedLogicalEntityName, j.RelatedEntityForeignKeyAttributeName, rootEntities
            .Select(e => e.GetAttributeValue<EntityReference>(j.KeyAttributeName))
            .Where(er => er != null)
            .Select(er => er!.Id)
            .Distinct()
            .ToArray()),
          _ => throw new NotImplementedException()
        };

        relatedEntitiesMap.Add(join, entities);
        allEntities.AddRange(entities.Entities);
      }

      foreach (var entity in allEntities) // Process all entities, not just rootEntities
      {
        foreach (var related in relatedEntitiesMap)
        {
          if (related.Key is OneToManyJoinData o2mj)
          {
            // Identify the parent entity
            var parentEntity = FindParentEntity(entity, o2mj);

            if (parentEntity != null)
            {
              // Get related entities that belong to the identified parent
              var relatedEntities = related.Value.Entities
                  .Where(e => e.GetAttributeValue<EntityReference>(o2mj.RelatedEntityForeignKeyAttributeName)!.Id == parentEntity.Id)
                  .ToArray();

              AddRelatedEntities(parentEntity, o2mj.RelationshipSchemaName, relatedEntities);
            }
          }
          else if (related.Key is ManyToOneJoinData m2oj)
          {
            // Handle Many-to-One relationships
            try
            {
              if (entity.GetAttributeValue<EntityReference>(m2oj.KeyAttributeName) == null) continue;
            }
            catch 
            {
              continue;
            }
            var key = entity.GetAttributeValue<EntityReference>(m2oj.KeyAttributeName)!.Id;
            var relatedEntity = related.Value.Entities.SingleOrDefault(e => e.GetAttributeValue<Guid>(m2oj.RelatedEntityForeignKeyAttributeName) == key);

            if (relatedEntity != null)
            {
              AddRelatedEntities(entity, m2oj.RelationshipSchemaName, relatedEntity);
            }
          }
          else
          {
            throw new NotImplementedException();
          }
        }
      }
    }

    return rootEntities;
  }

  private Entity? FindParentEntity(Entity baseEntity, OneToManyJoinData joinData)
  {
    // Check if the baseEntity matches the target type
    if (baseEntity.GetType() == joinData.EnityType)
    {
      return baseEntity;
    }

    // Iterate over all related entities
    foreach (var relatedEntities in baseEntity.RelatedEntities.Values)
    {
      foreach (var entity in relatedEntities.Entities)
      {
        // Recursive call to find the parent in nested entities
        var parentEntity = FindParentEntity(entity, joinData);
        if (parentEntity != null)
        {
          return parentEntity;
        }
      }
    }

    return null; // Return null if no parent is found
  }

  protected virtual EntityCollection QuerySubEntities(string entityName, string keyAttributeName, params Guid[] keys)
  {
    if (keys == null || keys.Length == 0)
    {
      return new EntityCollection();
    }

    var query = new QueryExpression(entityName)
    {
      Criteria = new FilterExpression(LogicalOperator.Or)
      {
        Conditions = { new ConditionExpression(attributeName: keyAttributeName, conditionOperator: ConditionOperator.In, values: keys) }
      }
    };
    query.ColumnSet.AllColumns = true;
    var entities = RetreiveEntities(Context, query);

    return entities;
  }

  protected virtual void AddRelatedEntities([NotNull] Entity rootEntity, string relationshipName, params Entity[] relatedEntities)
  {
    var relationship = new Relationship(relationshipName);

    if (!rootEntity.RelatedEntities.Contains(relationship))
    {
      rootEntity.RelatedEntities.Add(relationship, new EntityCollection(relatedEntities));
    }
  }

  protected static JoinData CreateOneToMany([NotNull] Type entityType, [NotNull] Type relatedEntityType, [NotNull] string propoertyName)
  {
    var relatedEntityLogicalName = relatedEntityType.GetCustomAttribute<EntityLogicalNameAttribute>()!.LogicalName;
    var rootProperty = entityType.GetProperty(propoertyName)!;
    var relationshipSchemaName = rootProperty.GetCustomAttribute<RelationshipSchemaNameAttribute>()!.SchemaName;
    var relatedEntityKeyProperty = relatedEntityType.GetProperties().Single(p => p.GetCustomAttribute<RelationshipSchemaNameAttribute>()?.SchemaName == relationshipSchemaName);
    var relatedEntityForeignKeyAttributeName = relatedEntityKeyProperty.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;

    return new OneToManyJoinData(entityType, relationshipSchemaName, relatedEntityLogicalName, relatedEntityForeignKeyAttributeName);
  }

  protected static JoinData CreateManyToOne([NotNull] Type entityType, [NotNull] Type relatedEntityType, [NotNull] string propertyName)
  {
    var relatedEntityLogicalName = relatedEntityType.GetCustomAttribute<EntityLogicalNameAttribute>()!.LogicalName;
    var rootProperty = entityType.GetProperty(propertyName)!;
    var relatedKeyProperty = relatedEntityType.GetProperty(nameof(Entity.Id))!;
    var keyAttributeName = rootProperty.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;
    var relationshipSchemaName = rootProperty.GetCustomAttribute<RelationshipSchemaNameAttribute>()!.SchemaName;
    var relatedEntityForeignKeyAttributeName = relatedKeyProperty.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;

    return new ManyToOneJoinData(entityType, relationshipSchemaName, relatedEntityLogicalName, keyAttributeName, relatedEntityForeignKeyAttributeName);
  }
}

public class NestedJoin2QueryExpressionBuilder<TEntity, TRelatedEntity> : Join2QueryExpressionBuilder<TEntity>
  where TEntity : Entity
  where TRelatedEntity : Entity
{
  public NestedJoin2QueryExpressionBuilder(QueryExpressionBuilder<TEntity> parentBuilder) : base(parentBuilder)
  {
  }

  public NestedJoin2QueryExpressionBuilder<TEntity, TRelatedEntity> IncludeNested<TNestedEntity>([NotNull] Expression<Func<TRelatedEntity, IEnumerable<TNestedEntity>>> selector)
  {
    var propertyName = ((MemberExpression)selector.Body).Member.Name;
    Joins.Add(CreateOneToMany(typeof(TRelatedEntity), typeof(TNestedEntity), propertyName));
    return this;
  }

  public NestedJoin2QueryExpressionBuilder<TEntity, TRelatedEntity> IncludeNested<TNestedEntity>([NotNull] Expression<Func<TRelatedEntity, TNestedEntity>> selector)
  {
    var propertyName = ((MemberExpression)selector.Body).Member.Name;
    Joins.Add(CreateManyToOne(typeof(TRelatedEntity), typeof(TNestedEntity), propertyName));

    return this;
  }
}
