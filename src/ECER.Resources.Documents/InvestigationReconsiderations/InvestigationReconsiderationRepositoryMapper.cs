using ECER.Utilities.DataverseSdk.Model;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.InvestigationReconsiderations;

internal interface IInvestigationReconsiderationRepositoryMapper
{
  ecer_ReconsiderationInvestigationOutcome MapInvestigationReconsiderationRequest(InvestigationReconsideration source);

  InvestigationReconsideration MapInvestigationReconsiderationRequest(ecer_ReconsiderationInvestigationOutcome source);

  List<InvestigationReconsideration> MapInvestigationReconsiderationRequests(IEnumerable<ecer_ReconsiderationInvestigationOutcome> source);

  List<ecer_ReconsiderationInvestigationOutcome_StatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<InvestigationReconsiderationStatusCode> source);
}

[Mapper]
internal partial class InvestigationReconsiderationRepositoryMapper : IInvestigationReconsiderationRepositoryMapper
{
  public ecer_ReconsiderationInvestigationOutcome MapInvestigationReconsiderationRequest(InvestigationReconsideration source) => new ecer_ReconsiderationInvestigationOutcome
  {
    ecer_ReconsiderationInvestigationOutcomeId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    ecer_ExplanationandEvidence = source.ExplanationAndEvidence,
  };

  public InvestigationReconsideration MapInvestigationReconsiderationRequest(ecer_ReconsiderationInvestigationOutcome source) => new InvestigationReconsideration()
  {
    Id = source.ecer_ReconsiderationInvestigationOutcomeId?.ToString(),
    Status = source.StatusCode.HasValue ? MapInvestigationReconsiderationStatus(source.StatusCode.Value) : default,
    Type = source.ecer_Type.HasValue ? MapInvestigationReconsiderationType(source.ecer_Type.Value) : default,
    //reconsideration end date resides in a different entity based on the type
    ReconsiderationEndDate = source.ecer_Type switch
    {
      ecer_ReconsiderationInvestigationType.InvestigationOutcome => source.ecer_reconsiderationinvestigationoutcome_InvestigationId?.ecer_reconsiderationperiodenddate,
      ecer_ReconsiderationInvestigationType.ImmediateAction => source.ecer_reconsiderationinvestigationoutcome_ImmediateActionId?.ecer_reconsiderationperiodenddate,
      _ => null
    },
    ExplanationAndEvidence = source.ecer_ExplanationandEvidence,
    Files = (source.ecer_bcgov_documenturl_ReconsiderationInvestigationOutcomeId ?? Array.Empty<bcgov_DocumentUrl>()).Select(MapFileInfo).ToList(),
  };

  public List<InvestigationReconsideration> MapInvestigationReconsiderationRequests(IEnumerable<ecer_ReconsiderationInvestigationOutcome> source) => source.Select(MapInvestigationReconsiderationRequest).ToList();

  public List<ecer_ReconsiderationInvestigationOutcome_StatusCode> MapInvestigationReconsiderationStatusCodes(IEnumerable<InvestigationReconsiderationStatusCode> source) => source.Select(MapInvestigationReconsiderationStatus).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InvestigationReconsiderationStatusCode MapInvestigationReconsiderationStatus(ecer_ReconsiderationInvestigationOutcome_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ReconsiderationInvestigationOutcome_StatusCode MapInvestigationReconsiderationStatus(InvestigationReconsiderationStatusCode source);

  public List<ecer_ReconsiderationInvestigationType> MapInvestigationReconsiderationType(IEnumerable<InvestigationReconsiderationType> source) => source.Select(MapInvestigationReconsiderationType).ToList();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InvestigationReconsiderationType MapInvestigationReconsiderationType(ecer_ReconsiderationInvestigationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_ReconsiderationInvestigationType MapInvestigationReconsiderationType(InvestigationReconsiderationType source);

  private static FileInfo MapFileInfo(bcgov_DocumentUrl source) => new(source.bcgov_DocumentUrlId?.ToString() ?? string.Empty)
  {
    Name = source.bcgov_FileName,
    Size = source.bcgov_FileSize,
    Url = source.bcgov_Url,
    Extention = source.bcgov_FileExtension,
  };
}
