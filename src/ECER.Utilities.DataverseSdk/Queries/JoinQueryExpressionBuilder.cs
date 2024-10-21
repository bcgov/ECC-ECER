using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Linq.Expressions;
using System.Reflection;

namespace ECER.Utilities.DataverseSdk.Queries;

public class JoinQueryExpressionBuilder<TEntity> : QueryExpressionBuilder<TEntity>
  where TEntity : Entity
{
  private readonly Dictionary<string, IncludeMetadata> map;

  protected internal JoinQueryExpressionBuilder(QueryExpressionBuilder<TEntity> parentBuilder) : base(parentBuilder)

  {
    map = parentBuilder is JoinQueryExpressionBuilder<TEntity> join ? join.map : [];
  }

  /// <summary>
  /// Includes a relation of an entity using a 1:N relation selector in a query.
  /// </summary>
  public NestedJoinQueryExpressionBuilder<TEntity, TRelatedEntity> Include<TRelatedEntity>(
    Expression<Func<TEntity, IEnumerable<TRelatedEntity>>> relationSelector)
    where TRelatedEntity : Entity
  {
    ArgumentNullException.ThrowIfNull(relationSelector);

    var relationPropertyName = ((MemberExpression)relationSelector.Body).Member.Name;
    var link = AddOneToManyLink<TEntity, TRelatedEntity>(relationPropertyName);
    return new NestedJoinQueryExpressionBuilder<TEntity, TRelatedEntity>(this, link);
  }

  /// <summary>
  /// Includes a relation of an entity using a N:1 relation selector in a query.
  /// </summary>
  public NestedJoinQueryExpressionBuilder<TEntity, TRelatedEntity> Include<TRelatedEntity>(
    Expression<Func<TEntity, TRelatedEntity>> relationSelector)
    where TRelatedEntity : Entity
  {
    ArgumentNullException.ThrowIfNull(relationSelector);

    var relationPropertyName = ((MemberExpression)relationSelector.Body).Member.Name;
    var link = AddManyToOneLink<TEntity, TRelatedEntity>(relationPropertyName);
    return new NestedJoinQueryExpressionBuilder<TEntity, TRelatedEntity>(this, link);
  }

  protected LinkEntity AddOneToManyLink<TEntity1, TRelatedEntity1>(string relationPropertyName)
    where TEntity1 : Entity
    where TRelatedEntity1 : Entity
  {
    var entityType = typeof(TEntity1);
    var relatedEntityType = typeof(TRelatedEntity1)!;

    var relatedEntityLogicalName = relatedEntityType.GetCustomAttribute<EntityLogicalNameAttribute>()!.LogicalName;

    var entityIdAttributeName = entityType.GetProperty(nameof(Entity.Id))!.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;
    var relatedEntityIdAttributeName = relatedEntityType.GetProperty(nameof(Entity.Id))!.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;

    var entityRelationName = entityType.GetProperty(relationPropertyName)!.GetCustomAttribute<RelationshipSchemaNameAttribute>()!.SchemaName;
    var relatedEntityRelationName = relatedEntityType.GetProperty(relationPropertyName)!.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;

    return AddLink(relatedEntityLogicalName, entityIdAttributeName, relatedEntityRelationName, relatedEntityType, entityRelationName, relatedEntityIdAttributeName);
  }

  protected LinkEntity AddManyToOneLink<TEntity1, TRelatedEntity1>(string relationPropertyName)
  where TEntity1 : Entity
  where TRelatedEntity1 : Entity
  {
    var entityType = typeof(TEntity1);
    var relatedEntityType = typeof(TRelatedEntity1)!;

    var relatedEntityLogicalName = relatedEntityType.GetCustomAttribute<EntityLogicalNameAttribute>()!.LogicalName;

    var relatedEntityIdAttributeName = relatedEntityType.GetProperty(nameof(Entity.Id))!.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;

    var entityRelationName = entityType.GetProperty(relationPropertyName)!.GetCustomAttribute<RelationshipSchemaNameAttribute>()!.SchemaName;
    var relatedEntityRelationName = entityType.GetProperty(relationPropertyName)!.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;

    return AddLink(relatedEntityLogicalName, relatedEntityRelationName, relatedEntityIdAttributeName, relatedEntityType, entityRelationName, relatedEntityIdAttributeName);
  }

  private LinkEntity AddLink(string linkToEntityName, string linkFromAttributedName, string linkToAttributedName, Type relatedEntityType, string entityRelationName, string relatedEntityIdAttributeName)
  {
    map.Add(relatedEntityType.Name, new IncludeMetadata(relatedEntityType, relatedEntityIdAttributeName, linkToEntityName, entityRelationName));
    var link = Query.AddLink(linkToEntityName, linkFromAttributedName, linkToAttributedName, JoinOperator.LeftOuter);
    link.Columns.AllColumns = true;
    link.EntityAlias = relatedEntityType.Name;
    return link;
  }

  /// <summary>
  /// Run the join on the server and return the results
  /// </summary>
  public override IEnumerable<TEntity> Execute()
  {
    var entities = Materialize(RetreiveEntities()).ToList();

#pragma warning disable S125 // Sections of code should not be commented out
    //AttachEntities(entities);
#pragma warning restore S125 // Sections of code should not be commented out

    return entities;
  }

  private IEnumerable<TEntity> Materialize(EntityCollection entityCollection)
  {
    //group the returned table by the entity id
    return entityCollection.Entities.GroupBy(e => e.Id, e => e, (_, entityRows) =>
    {
      //take the first entity instance in the group, they should all be the same, and transform to the correct entity type
      var entity = entityRows.First().ToEntity<TEntity>();
      var relatedEntitiesMap = new Dictionary<string, EntityCollection>();
      foreach (var row in entityRows)
      {
        //group the attributes that are joined by their link alias - the key format is alias.attribute_name
        var groupedAttributes = row.Attributes.Where(a => a.Value is AliasedValue).GroupBy(a => a.Key.Split('.')[0], a => new { a.Key, Value = (AliasedValue)a.Value });
        foreach (var group in groupedAttributes)
        {
          // get the joined entity metadata
          var metadata = map[group.Key];
          var attributes = new AttributeCollection();
          //map the entity's attributes, extract its primary id and create a new entity instance
          attributes.AddRange(group.Select(e => new KeyValuePair<string, object>(e.Value.AttributeLogicalName, e.Value.Value)));
          var entityId = (Guid)attributes.Single(a => a.Key == metadata.EntityIdAttributeName).Value;
          var relatedEntity = (Entity)Activator.CreateInstance(metadata.EntityType, entityId)!;
          relatedEntity.Attributes = attributes;
          //add the entity to the related entities map
          if (!relatedEntitiesMap.ContainsKey(metadata.RelationshipAttributeName)) relatedEntitiesMap.Add(metadata.RelationshipAttributeName, new EntityCollection());
          if (!relatedEntitiesMap[metadata.RelationshipAttributeName].Entities.Any(e => e.Id == entityId)) relatedEntitiesMap[metadata.RelationshipAttributeName].Entities.Add(relatedEntity);
        }
      }
      //add the related entities to the parent entity
      foreach (var related in relatedEntitiesMap)
      {
        entity.RelatedEntities.Add(new Relationship(related.Key), related.Value);
      }
      return entity;
    });
  }

  private sealed record IncludeMetadata(Type EntityType, string EntityIdAttributeName, string EntityLogicalName, string RelationshipAttributeName);
}
