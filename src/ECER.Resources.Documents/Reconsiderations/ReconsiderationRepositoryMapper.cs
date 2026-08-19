using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.Reconsiderations;

internal interface IReconsiderationRepositoryMapper
{
  ecer_ReconsiderationRequest MapReconsiderationRequest(Reconsideration source);

  Reconsideration MapReconsiderationRequest(ecer_ReconsiderationRequest source);

  List<Reconsideration> MapReconsiderationRequests(IEnumerable<ecer_ReconsiderationRequest> source);

  List<ecer_ReconsiderationRequest_StatusCode> MapReconsiderationStatusCodes(IEnumerable<ReconsiderationStatusCode> source);
}

[Mapper]
internal partial class ReconsiderationRepositoryMapper : IReconsiderationRepositoryMapper
{
  public ecer_ReconsiderationRequest MapReconsiderationRequest(Reconsideration source) => new ecer_ReconsiderationRequest
  {
    ecer_ReconsiderationRequestId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_ReconsiderationDetails = source.ReconsiderationDetails,
    ecer_ExplanationandEvidence = source.ExplanationAndEvidence,
  };

  public Reconsideration MapReconsiderationRequest(ecer_ReconsiderationRequest source) => new Reconsideration()
  {
    Id = source.ecer_ReconsiderationRequestId?.ToString(),
    Status = source.StatusCode.HasValue ? MapReconsiderationStatus(source.StatusCode.Value) : default,
    ReconsiderationDetails = source.ecer_ReconsiderationDetails,
    ExplanationAndEvidence = source.ecer_ExplanationandEvidence,
    Files = (source.ecer_bcgov_documenturl_ReconsiderationRequestId ?? Array.Empty<bcgov_DocumentUrl>()).Select(MapFileInfo).ToList(),
    ReconsiderationEndDate = source.ecer_reconsiderationrequest_ApplicationId?.ecer_ReconsiderationPeriodEndDate,
    ApplicationId = source.ecer_reconsiderationrequest_ApplicationId?.Id.ToString(),
  };

  public List<Reconsideration> MapReconsiderationRequests(IEnumerable<ecer_ReconsiderationRequest> source) => source.Select(MapReconsiderationRequest).ToList();

  public List<ecer_ReconsiderationRequest_StatusCode> MapReconsiderationStatusCodes(IEnumerable<ReconsiderationStatusCode> source) => source.Select(MapReconsiderationStatus).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ReconsiderationStatusCode MapReconsiderationStatus(ecer_ReconsiderationRequest_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ReconsiderationRequest_StatusCode MapReconsiderationStatus(ReconsiderationStatusCode source);

  private static FileInfo MapFileInfo(bcgov_DocumentUrl source) => new(source.bcgov_DocumentUrlId?.ToString() ?? string.Empty)
  {
    Name = source.bcgov_FileName,
    Size = source.bcgov_FileSize,
    Url = source.bcgov_Url,
    Extention = source.bcgov_FileExtension,
  };
}
