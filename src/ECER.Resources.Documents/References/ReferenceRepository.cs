using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Documents.References;

internal class ReferenceRepository : IReferenceRepository
{
  private readonly EcerContext context;

  public ReferenceRepository(EcerContext context)
  {
    this.context = context;
  }

  public async Task<bool> SubmitReferenceResponse(ReferenceSubmissionRequest request, CancellationToken ct)
  {
    await Task.CompletedTask;

    return true;
  }
}
