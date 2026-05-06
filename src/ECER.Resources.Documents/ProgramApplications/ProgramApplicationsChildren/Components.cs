using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.ProgramApplications;

internal sealed partial class ProgramApplicationRepository
{
  public async Task<ApplicationFileInfo> CreateDocumentUrlAndShare(CreateDocumentUrlRequest request, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var documentGuid = Guid.Parse(request.FileId);
    var programApplicationGuid = Guid.Parse(request.ProgramApplicationId);
    var componentGroupGuid = Guid.Parse(request.ComponentGroupId);
    var componentGuid = Guid.Parse(request.ComponentId);
    var instituteGuid = Guid.Parse(request.InstituteId);
    var fileId = request.FileId;
    var fileName = request.FileName;
    var fileSize = request.FileSize;
    var folder = request.Folder;

    var institute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(d => d.ecer_PostSecondaryInstituteId == instituteGuid)
      ?? throw new InvalidOperationException($"Institute '{request.FileId}' not found");

    var componentGroup = context.ecer_ProgramApplicationComponentGroupSet.SingleOrDefault(cg => cg.ecer_ProgramApplicationComponentGroupId == componentGroupGuid)
      ?? throw new InvalidOperationException($"Component group '{request.ComponentGroupId}' not found");

    var programApplication = context.ecer_PostSecondaryInstituteProgramApplicaitonSet.SingleOrDefault(a => a.ecer_PostSecondaryInstituteProgramApplicaitonId == programApplicationGuid)
      ?? throw new InvalidOperationException($"Program application '{request.ProgramApplicationId}' not found");

    var component = context.ecer_ProgramApplicationComponentSet.SingleOrDefault(c => c.ecer_ProgramApplicationComponentId == componentGuid)
      ?? throw new InvalidOperationException($"Component '{request.ComponentId}' not found");

    var extension = Path.GetExtension(fileName);

    var documentUrl = new bcgov_DocumentUrl
    {
      bcgov_DocumentUrlId = documentGuid,
      bcgov_FileName = fileName,
      bcgov_FileSize = fileSize,
      bcgov_FileExtension = extension,
      bcgov_Url = folder,
      StatusCode = bcgov_DocumentUrl_StatusCode.Active,
      StateCode = bcgov_documenturl_statecode.Active,
      ecer_ApplicationName = EcerWebApplicationType.PSP.ToString(),
    };

    context.AddObject(documentUrl);
    context.AddLink(documentUrl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_psiid_ecer_postsecondaryinstitute, institute);
    context.AddLink(documentUrl, bcgov_DocumentUrl.Fields.ecer_documenturl_ProgramApplicationComponentGroupId, componentGroup);
    context.AddLink(documentUrl, bcgov_DocumentUrl.Fields.ecer_bcgov_documenturl_programapplicationid_ecer_postsecondaryinstituteprogramapplicaiton, programApplication);
    context.AddLink(documentUrl, bcgov_DocumentUrl.Fields.ecer_documenturl_ProgramApplicationComponentId, component);
    context.SaveChanges();

    // Dynamics auto-creates the ecer_ShareDocumentURL on save; query it back to get its ID
    var autoShare = context.ecer_ShareDocumentURLSet
      .Where(s => s.ecer_DocumentURLId.Id == documentGuid)
      .AsEnumerable()
      .FirstOrDefault();

    var shareId = autoShare?.ecer_ShareDocumentURLId?.ToString() ?? string.Empty;

    return new ApplicationFileInfo(fileId, shareId, fileName, fileSize, folder, extension, EcerWebApplicationType.PSP);
  }

  public async Task<ApplicationFileInfo> CreateShareOnly(
    string documentId, string programApplicationId, string componentGroupId,
    string componentId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var documentGuid = Guid.Parse(documentId);
    var programApplicationGuid = Guid.Parse(programApplicationId);
    var componentGroupGuid = Guid.Parse(componentGroupId);
    var componentGuid = Guid.Parse(componentId);

    var docUrl = context.bcgov_DocumentUrlSet.SingleOrDefault(d => d.bcgov_DocumentUrlId == documentGuid)
      ?? throw new InvalidOperationException($"Document '{documentId}' not found");

    var shareGuid = Guid.NewGuid();
    var shareDocumentUrl = new ecer_ShareDocumentURL
    {
      ecer_ShareDocumentURLId = shareGuid,
      ecer_DocumentURLId = new EntityReference(bcgov_DocumentUrl.EntityLogicalName, documentGuid),
      ecer_ProgramApplicationId = new EntityReference(ecer_PostSecondaryInstituteProgramApplicaiton.EntityLogicalName, programApplicationGuid),
      ecer_ProgramApplicationComponentGroupId = new EntityReference(ecer_ProgramApplicationComponentGroup.EntityLogicalName, componentGroupGuid),
      ecer_ProgramApplicationComponentId = new EntityReference(ecer_ProgramApplicationComponent.EntityLogicalName, componentGuid),
      ecer_URL = docUrl.bcgov_Url,
      StatusCode = ecer_ShareDocumentURL_StatusCode.Active,
      StateCode = ecer_sharedocumenturl_statecode.Active,
    };

    context.AddObject(shareDocumentUrl);
    context.SaveChanges();

    return new ApplicationFileInfo(
      documentId,
      shareGuid.ToString(),
      docUrl.bcgov_FileName ?? string.Empty,
      docUrl.bcgov_FileSize ?? string.Empty,
      docUrl.bcgov_Url ?? string.Empty,
      docUrl.bcgov_FileExtension,
      EcerWebApplicationType.PSP);
  }

  public async Task<IEnumerable<ApplicationFileInfo>> GetApplicationFiles(string programApplicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var programApplicationGuid = Guid.Parse(programApplicationId);

    var shareDocUrls = context.ecer_ShareDocumentURLSet
      .Where(s => s.ecer_ProgramApplicationId != null && s.ecer_ProgramApplicationId.Id == programApplicationGuid)
      .ToList();

    if (shareDocUrls.Count == 0) return Array.Empty<ApplicationFileInfo>();

    var documentIds = shareDocUrls
      .Where(s => s.ecer_DocumentURLId != null)
      .Select(s => s.ecer_DocumentURLId.Id)
      .Distinct()
      .ToList();

    var docUrls = context.bcgov_DocumentUrlSet
      .WhereIn(d => d.bcgov_DocumentUrlId!.Value, documentIds)
      .AsEnumerable()
      .ToDictionary(d => d.bcgov_DocumentUrlId!.Value);

    return shareDocUrls
      .Where(s => s.ecer_DocumentURLId != null && docUrls.ContainsKey(s.ecer_DocumentURLId.Id))
      .GroupBy(s => s.ecer_DocumentURLId!.Id)
      .Select(g =>
      {
        var s = g.First();
        var docUrl = docUrls[s.ecer_DocumentURLId!.Id];
        return new ApplicationFileInfo(
          s.ecer_DocumentURLId.Id.ToString(),
          s.ecer_ShareDocumentURLId?.ToString() ?? string.Empty,
          docUrl.bcgov_FileName ?? string.Empty,
          docUrl.bcgov_FileSize ?? string.Empty,
          docUrl.bcgov_Url ?? string.Empty,
          docUrl.bcgov_FileExtension,
          EcerWebApplicationType.PSP);
      })
      .ToList();
  }

  public async Task<IEnumerable<ApplicationFileInfo>> GetApplicationDocumentUrls(string programApplicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var programApplicationGuid = Guid.Parse(programApplicationId);

    var docUrls = context.bcgov_DocumentUrlSet
      .Where(d => d.ecer_programapplicationid != null && d.ecer_programapplicationid.Id == programApplicationGuid)
      .ToList();

    return docUrls.Select(d => new ApplicationFileInfo(
      d.bcgov_DocumentUrlId?.ToString() ?? string.Empty,
      string.Empty,
      d.bcgov_FileName ?? string.Empty,
      d.bcgov_FileSize ?? string.Empty,
      d.bcgov_Url ?? string.Empty,
      d.bcgov_FileExtension,
      EcerWebApplicationType.PSP))
    .ToList();
  }

  public async Task<ShareDocumentUrlDetails> GetShareDocumentUrlDetails(string shareDocumentId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var shareGuid = Guid.Parse(shareDocumentId);

    var share = context.ecer_ShareDocumentURLSet.SingleOrDefault(s => s.ecer_ShareDocumentURLId == shareGuid)
      ?? throw new InvalidOperationException($"Share document '{shareDocumentId}' not found");

    if (share.ecer_DocumentURLId == null)
      throw new InvalidOperationException($"Share document '{shareDocumentId}' has no document reference");

    var documentGuid = share.ecer_DocumentURLId.Id;

    var docUrl = context.bcgov_DocumentUrlSet.SingleOrDefault(d => d.bcgov_DocumentUrlId == documentGuid)
      ?? throw new InvalidOperationException($"Document '{documentGuid}' not found");

    var remainingShareCount = context.ecer_ShareDocumentURLSet
      .Where(s => s.ecer_DocumentURLId != null && s.ecer_DocumentURLId.Id == documentGuid)
      .ToList()
      .Count;

    if (!Enum.TryParse(docUrl.ecer_ApplicationName, out EcerWebApplicationType ecerWebApplicationType))
      ecerWebApplicationType = EcerWebApplicationType.PSP;

    return new ShareDocumentUrlDetails(documentGuid.ToString(), docUrl.bcgov_Url ?? string.Empty, ecerWebApplicationType, remainingShareCount);
  }

  public async Task DeleteShareDocumentUrlById(string shareDocumentId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var shareGuid = Guid.Parse(shareDocumentId);
    var share = context.ecer_ShareDocumentURLSet.SingleOrDefault(s => s.ecer_ShareDocumentURLId == shareGuid)
      ?? throw new InvalidOperationException($"Share document '{shareDocumentId}' not found");

    var documentUrlId = share.ecer_DocumentURLId?.Id;
    var remainingShareCount = documentUrlId.HasValue
      ? context.ecer_ShareDocumentURLSet
          .Where(s => s.ecer_DocumentURLId != null && s.ecer_DocumentURLId.Id == documentUrlId.Value)
          .ToList()
          .Count
      : 0;

    context.DeleteObject(share);

    if (documentUrlId.HasValue && remainingShareCount <= 1)
    {
      var docUrl = context.bcgov_DocumentUrlSet.SingleOrDefault(d => d.bcgov_DocumentUrlId == documentUrlId.Value);
      if (docUrl != null) context.DeleteObject(docUrl);
    }

    context.SaveChanges();
  }

}
