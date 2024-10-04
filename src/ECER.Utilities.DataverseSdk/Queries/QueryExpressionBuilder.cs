using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Utilities.DataverseSdk.Queries;

public class QueryExpressionBuilder<TEntity>
  where TEntity : Entity
{
  protected OrganizationServiceContext Context { get; }
  protected QueryExpression Query { get; }

  public QueryExpressionBuilder(OrganizationServiceContext context, QueryExpression query)
  {
    this.Context = context;
    Query = query;
  }

  protected QueryExpressionBuilder(QueryExpressionBuilder<TEntity> parentBuilder)
  {
    ArgumentNullException.ThrowIfNull(parentBuilder);
    Context = parentBuilder.Context;
    Query = parentBuilder.Query;
  }

  /// <summary>
  /// Execute the query expression on the server and return the entities.
  /// </summary>
  /// <returns></returns>
  public virtual IEnumerable<TEntity> Execute()
  {
    var entities = RetreiveEntities();
    return entities.Entities.Select(e => e.ToEntity<TEntity>()).ToList();
  }

  /// <summary>
  /// Starts a new aggregate query expression builder.
  /// </summary>
  public AggregateQueryExpressionBuilder<TEntity> Aggregate() => new(this);

  /// <summary>
  /// Starts a new join query expression builder.
  /// </summary>
  /// <returns></returns>

  public Join2QueryExpressionBuilder<TEntity> Join() => new(this);


  protected virtual EntityCollection RetreiveEntities() =>
   ((RetrieveMultipleResponse)Context.Execute(new RetrieveMultipleRequest { Query = Query })).EntityCollection;


  protected virtual void AttachEntities([NotNull] IEnumerable<Entity> entities)
  {
    if (Context.MergeOption != MergeOption.NoTracking)
    {
      //add entities to the context if tracking is enabled
      foreach (var entity in entities) Context.Attach(entity);
    }
  }
}
