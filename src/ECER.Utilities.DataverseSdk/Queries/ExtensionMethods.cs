using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ECER.Utilities.DataverseSdk.Queries;

public static class ExtensionMethods
{
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

  private static readonly MethodInfo translateMethod =
    Assembly.GetAssembly(typeof(Entity))!
    .GetType("Microsoft.Xrm.Sdk.Linq.QueryProvider")!
    .GetMethod("GetQueryExpression", BindingFlags.NonPublic | BindingFlags.Instance)!;

  private static readonly Type linkLookupListType =
    Assembly.GetAssembly(typeof(Entity))!
    .GetType("Microsoft.Xrm.Sdk.Linq.QueryProvider")!
    .GetNestedType("LinkLookup", BindingFlags.NonPublic)!;

#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

  /// <summary>
  /// Converts a LINQ CRM query to QueryExpression by invoking the CRM LINQ QueryProvider's internal capability
  /// </summary>
  internal static QueryExpression ToQueryExpression<T>(this IQueryable<T> query)
  {
    // `QueryProvider.Translate` method has a bug (link lookup list is initialized to null) so it fails - this is cloning the correct implementation
    object?[] args = [query.Expression, null, null, null, null, (Activator.CreateInstance(typeof(List<>).MakeGenericType(linkLookupListType)))];
    return (QueryExpression)translateMethod.Invoke(query.Provider, args)!;
  }

  /// <summary>
  /// Creates a query expression builder from a LINQ query
  /// </summary>
  public static QueryExpressionBuilder<TEntity> From<TEntity>(this OrganizationServiceContext context, [NotNull] IQueryable<TEntity> query)
    where TEntity : Entity
    => new QueryExpressionBuilder<TEntity>(context, query.ToQueryExpression());
}
