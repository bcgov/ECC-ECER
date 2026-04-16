using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers;
using Ganss.Xss;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Accounts.Communications;

internal interface ICommunicationRepositoryMapper
{
  List<ecer_Communication_StatusCode> MapCommunicationStatuses(IEnumerable<CommunicationStatus> source);
  List<Communication> MapCommunications(IEnumerable<ecer_Communication> source);
  Communication MapCommunication(ecer_Communication source);
  ecer_Communication MapCommunication(Communication source);
}

[Mapper]
internal partial class CommunicationRepositoryMapper : ICommunicationRepositoryMapper
{
  private static readonly HtmlSanitizer htmlSanitizer = new();

  public List<ecer_Communication_StatusCode> MapCommunicationStatuses(IEnumerable<CommunicationStatus> source) => source.Select(MapCommunicationStatus).ToList();

  public List<Communication> MapCommunications(IEnumerable<ecer_Communication> source) => source.Select(MapCommunication).ToList();

  public Communication MapCommunication(ecer_Communication source) => new(source.ecer_CommunicationId?.ToString())
  {
    Category = MapCommunicationCategory(source.ecer_PSPCommunicationCategory),
    Body = htmlSanitizer.Sanitize(source.ecer_Message ?? string.Empty, "", null),
    Subject = source.ecer_Name ?? string.Empty,
    From = MapInitiatedFrom(source.ecer_InitiatedFrom) ?? default,
    Acknowledged = source.ecer_Acknowledged.GetValueOrDefault(),
    NotifiedOn = source.ecer_DateNotified.GetValueOrDefault(),
    Status = MapCommunicationStatus(source.StatusCode) ?? default,
    ApplicationId = GetApplicationId(source)?.ToString(),
    IcraEligibilityId = GetIcraEligibilityId(source)?.ToString(),
    DoNotReply = source.ecer_DoNotReply.GetValueOrDefault(),
    Documents = MapCommunicationDocuments(source.ecer_bcgov_documenturl_CommunicationId_ecer_communication),
    LatestMessageNotifiedOn = source.ecer_IsRoot == true ? (source.ecer_LatestMessageNotifiedDate ?? source.ecer_DateNotified) : source.ecer_DateNotified,
    IsRead = source.ecer_IsRoot == true ? source.ecer_AreAllRead : source.ecer_Acknowledged,
    ProgramRepresentativeId = source.ecer_ProgramRepresentativeId?.Id != Guid.Empty ? source.ecer_ProgramRepresentativeId?.Id.ToString() : null,
    EducationInstituteName = source.ecer_EducationInstitutionIdName,
  };

  public ecer_Communication MapCommunication(Communication source) => new()
  {
    ecer_Message = htmlSanitizer.Sanitize(source.Body, "", null),
    ecer_Name = htmlSanitizer.Sanitize(source.Subject, "", null),
    ecer_PSPCommunicationCategory = MapCommunicationCategory(source.Category),
  };

  private List<CommunicationDocument> MapCommunicationDocuments(IEnumerable<bcgov_DocumentUrl>? source) =>
    source?.Select(MapCommunicationDocument).ToList() ?? new List<CommunicationDocument>();

  private CommunicationDocument MapCommunicationDocument(bcgov_DocumentUrl source) => new(source.bcgov_DocumentUrlId?.ToString() ?? string.Empty)
  {
    Name = source.bcgov_FileName,
    Size = source.bcgov_FileSize,
    Url = source.bcgov_Url,
    Extention = source.bcgov_FileExtension,
    EcerWebApplicationType = MapEcerWebApplicationType(source.ecer_ApplicationName),
  };

  private static Guid? GetIcraEligibilityId(ecer_Communication source)
  {
    var allowedStatusCodes = new List<ecer_ICRAEligibilityAssessment_StatusCode>
    {
      ecer_ICRAEligibilityAssessment_StatusCode.Submitted,
      ecer_ICRAEligibilityAssessment_StatusCode.Active,
      ecer_ICRAEligibilityAssessment_StatusCode.ReadyforReview,
      ecer_ICRAEligibilityAssessment_StatusCode.InReview,
      ecer_ICRAEligibilityAssessment_StatusCode.ReadyforAssessment,
    };

    if (source.ecer_communication_ICRAEligibilityAssessmentId == null || source.ecer_communication_ICRAEligibilityAssessmentId.StatusCode == null)
    {
      return null;
    }

    if (!allowedStatusCodes.Contains(source.ecer_communication_ICRAEligibilityAssessmentId.StatusCode.Value))
    {
      return null;
    }

    return source.ecer_communication_ICRAEligibilityAssessmentId.ecer_ICRAEligibilityAssessmentId;
  }

  private static Guid? GetApplicationId(ecer_Communication source)
  {
    var allowedStatusCodes = new List<ecer_Application_StatusCode>
    {
      ecer_Application_StatusCode.Closed,
      ecer_Application_StatusCode.Escalated,
      ecer_Application_StatusCode.InProgress,
      ecer_Application_StatusCode.Pending,
      ecer_Application_StatusCode.PendingPSPConsultationNeeded,
      ecer_Application_StatusCode.PendingQueue,
      ecer_Application_StatusCode.Ready,
      ecer_Application_StatusCode.Submitted
    };

    if (source.ecer_communication_Applicationid == null || source.ecer_communication_Applicationid.StatusCode == null)
    {
      return null;
    }

    if (!allowedStatusCodes.Contains(source.ecer_communication_Applicationid.StatusCode.Value))
    {
      return null;
    }

    return source.ecer_communication_Applicationid.ecer_ApplicationId;
  }

  private static EcerWebApplicationType MapEcerWebApplicationType(string? source) =>
    Enum.TryParse<EcerWebApplicationType>(source, out var applicationType) ? applicationType : default;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSPCommunicationCategories MapCommunicationCategory(CommunicationCategory source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CommunicationCategory MapCommunicationCategory(ecer_PSPCommunicationCategories source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_Communication_StatusCode MapCommunicationStatus(CommunicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial CommunicationStatus MapCommunicationStatus(ecer_Communication_StatusCode source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial InitiatedFrom MapInitiatedFrom(ecer_InitiatedFrom source);

  private ecer_PSPCommunicationCategories? MapCommunicationCategory(CommunicationCategory? source) => source.HasValue ? MapCommunicationCategory(source.Value) : null;

  private CommunicationCategory? MapCommunicationCategory(ecer_PSPCommunicationCategories? source) => source.HasValue ? MapCommunicationCategory(source.Value) : null;

  private CommunicationStatus? MapCommunicationStatus(ecer_Communication_StatusCode? source) => source.HasValue ? MapCommunicationStatus(source.Value) : null;

  private InitiatedFrom? MapInitiatedFrom(ecer_InitiatedFrom? source) => source.HasValue ? MapInitiatedFrom(source.Value) : null;
}
