using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Linq.Expressions;

namespace ECER.Utilities.DataverseSdk.Queries;

public class NestedJoinQueryExpressionBuilder<TEntity, TChildEntity> : JoinQueryExpressionBuilder<TEntity>
  where TEntity : Entity
  where TChildEntity : Entity

{
  private readonly LinkEntity linkEntity;

  protected internal NestedJoinQueryExpressionBuilder(JoinQueryExpressionBuilder<TEntity> parentBuilder, LinkEntity linkEntity) : base(parentBuilder)
  {
    this.linkEntity = linkEntity;
  }

  /// <summary>
  /// Includes a nested relation of a child entity using the specified relation and attributes.
  /// </summary>
  public NestedJoinQueryExpressionBuilder<TEntity, TChildEntity> IncludeNested<TRelatedEntity>(
    string entityLogicalName,
    string primaryKeyAttributeName,
    string relationAttributeName,
    string linkFromAttributeName,
    string linkToAttributeName)
    where TRelatedEntity : Entity
  {
    var relatedEntityType = typeof(TRelatedEntity)!;
    var parentEntityType = typeof(TChildEntity);
    var link = linkEntity.AddLink(entityLogicalName, linkFromAttributeName, linkToAttributeName, JoinOperator.LeftOuter);
    link.Columns.AllColumns = true;
    link.EntityAlias = $"{parentEntityType.Name}.{relatedEntityType.Name}";

    return new NestedJoinQueryExpressionBuilder<TEntity, TChildEntity>(this, link);
  }

  /// <summary>
  /// Includes a child of a related entity using a relation selector in a query.
  /// </summary>
  public NestedJoinQueryExpressionBuilder<TEntity, TChildEntity> IncludeChild<TRelatedEntity>(
    Expression<Func<TChildEntity, IEnumerable<TRelatedEntity>>> relationSelector
    )
    where TRelatedEntity : Entity
  {
    ArgumentNullException.ThrowIfNull(relationSelector);

    var relationPropertyName = ((MemberExpression)relationSelector.Body).Member.Name;
    var link = AddOneToManyLink<TChildEntity, TRelatedEntity>(relationPropertyName);
    return new NestedJoinQueryExpressionBuilder<TEntity, TChildEntity>(this, link);
  }
}
