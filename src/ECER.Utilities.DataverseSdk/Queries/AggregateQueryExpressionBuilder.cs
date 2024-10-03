using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System.Reflection;

namespace ECER.Utilities.DataverseSdk.Queries;

public class AggregateQueryExpressionBuilder<TEntity> : QueryExpressionBuilder<TEntity>
  where TEntity : Entity
{
  protected internal AggregateQueryExpressionBuilder(QueryExpressionBuilder<TEntity> parentBuilder) : base(parentBuilder)
  {
  }

  public int Count()
  {
    var atrributeName = typeof(TEntity)!.GetProperty(nameof(Entity.Id))!.GetCustomAttribute<AttributeLogicalNameAttribute>()!.LogicalName;

    Query.ColumnSet.AllColumns = false;
    foreach (var linkEntity in Query.LinkEntities)
    {
      linkEntity.Columns.AllColumns = false;
    }
    Query.ColumnSet.AttributeExpressions.Add(new XrmAttributeExpression(atrributeName, XrmAggregateType.Count, "Count"));

    var results = RetreiveEntities();

    var countColumn = (AliasedValue)results.Entities[0]["Count"];
    return (int)countColumn.Value;
  }
}
