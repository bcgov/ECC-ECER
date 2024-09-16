using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System.Diagnostics;

namespace ECER.Utilities.DataverseSdk.Model;

public partial class EcerContext
{
  internal static readonly string TraceSourceName = typeof(EcerContext).FullName!;
  private static ActivitySource source = new(TraceSourceName);
  private Activity? executionActivity;
  private Activity? saveChangesActivity;
  private bool isInTransaction;

  public new void SaveChanges()
  {
    if (!isInTransaction)
    {
      base.SaveChanges();
    }
  }

  protected override void OnExecuting(OrganizationRequest request)
  {
    executionActivity = source.StartActivity("Execute");
    executionActivity?.SetTag(nameof(request.RequestName), request?.RequestName);
    executionActivity?.SetTag(nameof(request.RequestId), request?.RequestId);
    executionActivity?.SetTag(nameof(request.Parameters), string.Join(',', request?.Parameters.Select(p => $"{p.Key}={p.Value}") ?? []));

    base.OnExecuting(request);
  }

  protected override void OnExecute(OrganizationRequest request, OrganizationResponse response)
  {
    base.OnExecute(request, response);
    executionActivity?.SetStatus(ActivityStatusCode.Ok);
    executionActivity?.Stop();
  }

  protected override void OnExecute(OrganizationRequest request, Exception exception)
  {
    base.OnExecute(request, exception);
    executionActivity?.SetStatus(ActivityStatusCode.Error, exception?.Message);
    executionActivity?.AddTag("Exception", exception);
    executionActivity?.Stop();
  }

  protected override void OnSavingChanges(SaveChangesOptions options)
  {
    saveChangesActivity = source.StartActivity("SaveChanges");
    saveChangesActivity?.AddTag(nameof(SaveChangesOptions), options);
    saveChangesActivity?.AddTag(nameof(isInTransaction), isInTransaction);
    base.OnSavingChanges(options);
  }

  protected override void OnSaveChanges(SaveChangesResultCollection results)
  {
    base.OnSaveChanges(results);
    saveChangesActivity?.AddTag(nameof(results.HasError), results?.HasError);
    foreach (var result in results?.AsEnumerable() ?? [])
    {
      saveChangesActivity?.AddTag(nameof(result.Request.RequestName), result.Request.RequestName);
      if (result.Error != null)
      {
        saveChangesActivity?.AddTag($"{nameof(result.Request.RequestName)}-{nameof(result.Error)}", result.Error);
      }
    }
    saveChangesActivity?.Stop();
  }

  public TransactionContext BeginTransaction()
  {
    if (isInTransaction) throw new InvalidOperationException("Already in a transaction");
    isInTransaction = true;
    return new TransactionContext(this);
  }

  public void CommitTransaction()
  {
    if (isInTransaction)
    {
      saveChangesActivity?.AddEvent(new ActivityEvent(nameof(CommitTransaction)));
      base.SaveChanges();
      isInTransaction = false;
    }
  }

  protected override void Dispose(bool disposing)
  {
    executionActivity?.Dispose();
    saveChangesActivity?.Dispose();
    base.Dispose(disposing);
  }
}

public class TransactionContext(EcerContext context)
{
  public void Commit() => context.CommitTransaction();
}
