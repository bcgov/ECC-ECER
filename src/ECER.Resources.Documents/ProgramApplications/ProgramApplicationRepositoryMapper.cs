using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.ObjectStorage.Providers;
using Riok.Mapperly.Abstractions;

namespace ECER.Resources.Documents.ProgramApplications;

internal interface IProgramApplicationRepositoryMapper
{
  ecer_PostSecondaryInstituteProgramApplicaiton MapProgramApplication(ProgramApplication source);
  List<ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode> MapApplicationStatuses(IEnumerable<ApplicationStatus> source);
  List<ProgramApplication> MapProgramApplications(IEnumerable<ecer_PostSecondaryInstituteProgramApplicaiton> source);
  List<NavigationMetadata> MapNavigationMetadata(IEnumerable<ecer_ProgramApplicationComponentGroup> source);
  List<ComponentGroupWithComponents> MapComponentGroupsWithComponents(IEnumerable<ecer_ProgramApplicationComponentGroup> source);
  ecer_ProgramApplicationComponent MapProgramApplicationComponent(ProgramApplicationComponent source);
}

[Mapper]
internal partial class ProgramApplicationRepositoryMapper : IProgramApplicationRepositoryMapper
{
  public ecer_PostSecondaryInstituteProgramApplicaiton MapProgramApplication(ProgramApplication source) => new()
  {
    ecer_PostSecondaryInstituteProgramApplicaitonId = string.IsNullOrWhiteSpace(source.Id) ? null : Guid.Parse(source.Id),
    StatusCode = MapApplicationStatus(source.Status),
    ecer_statusreasondetail = MapStatusReasonDetail(source.StatusReasonDetail),
    ecer_Name = source.ProgramApplicationName,
    ecer_ApplicationType = MapApplicationType(source.ProgramApplicationType),
    ecer_ProjectedLength = MapDecimal(source.ProgramLength),
    ecer_Onlinemethodsofinstruction = source.OnlineMethodOfInstruction?.Select(MapMethodOfInstruction).ToList(),
    ecer_Deliverymethodforpracticuminstructor = source.DeliveryMethod?.Select(MapDeliveryMethod).ToList(),
    ecer_ProgramEnrollment = source.EnrollmentOptions?.Select(MapWorkHoursType).ToList(),
    ecer_AdmissionOptions = source.AdmissionOptions?.Select(MapAdmissionOptions).ToList(),
    ecer_MinimumStudentEnrollmentperCourse = MapEnrollment(source.MinimumEnrollment),
    ecer_MaximumStudentEnrollmentperCourse = MapEnrollment(source.MaximumEnrollment),
    ecer_OnlineDeliveryHoursPercentage = MapDecimal(source.OnlineDeliveryHoursPercentage),
    ecer_InpersonHoursPercentage = MapDecimal(source.InPersonHoursPercentage),
    ecer_OtherAdmissionOptions = source.OtherAdmissionOptions,
  };

  public List<ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode> MapApplicationStatuses(IEnumerable<ApplicationStatus> source) => source.Select(MapApplicationStatus).ToList();

  public List<ProgramApplication> MapProgramApplications(IEnumerable<ecer_PostSecondaryInstituteProgramApplicaiton> source) => source.Select(MapProgramApplication).ToList();

  public List<NavigationMetadata> MapNavigationMetadata(IEnumerable<ecer_ProgramApplicationComponentGroup> source) => source.Select(MapNavigationMetadata).ToList();

  public List<ComponentGroupWithComponents> MapComponentGroupsWithComponents(IEnumerable<ecer_ProgramApplicationComponentGroup> source) => source.Select(MapComponentGroupWithComponents).ToList();

  public ecer_ProgramApplicationComponent MapProgramApplicationComponent(ProgramApplicationComponent source) => new()
  {
    ecer_ProgramApplicationComponentId = Guid.Parse(source.Id),
    ecer_Componentanswer = source.Answer,
  };

  private ProgramApplication MapProgramApplication(ecer_PostSecondaryInstituteProgramApplicaiton source) => new(
    source.ecer_PostSecondaryInstituteProgramApplicaitonId?.ToString(),
    source.ecer_PostSecondaryInstitute?.Id.ToString() ?? string.Empty)
  {
    ProgramApplicationName = source.ecer_Name,
    Status = MapApplicationStatus(source.StatusCode),
    StatusReasonDetail = MapStatusReasonDetail(source.ecer_statusreasondetail),
    ProgramTypes = source.ecer_ProgramType?.Select(MapProgramCertificationType).ToList(),
    DeliveryType = MapDeliveryType(source.ecer_DeliveryType),
    ProgramApplicationType = MapApplicationType(source.ecer_ApplicationType),
    ComponentsGenerationCompleted = source.ecer_ComponentsGenerationCompleted,
    ProgramRepresentativeId = source.ecer_PSIProgramRepresentative?.Id.ToString(),
    ProgramLength = MapFloat(source.ecer_ProjectedLength),
    OnlineMethodOfInstruction = source.ecer_Onlinemethodsofinstruction?.Select(MapMethodOfInstruction).ToList(),
    DeliveryMethod = source.ecer_Deliverymethodforpracticuminstructor?.Select(MapDeliveryMethod).ToList(),
    EnrollmentOptions = source.ecer_ProgramEnrollment?.Select(MapWorkHoursType).ToList(),
    AdmissionOptions = source.ecer_AdmissionOptions?.Select(MapAdmissionOptions).ToList(),
    MinimumEnrollment = MapEnrollment(source.ecer_MinimumStudentEnrollmentperCourse),
    MaximumEnrollment = MapEnrollment(source.ecer_MaximumStudentEnrollmentperCourse),
    OnlineDeliveryHoursPercentage = MapFloat(source.ecer_OnlineDeliveryHoursPercentage),
    InPersonHoursPercentage = MapFloat(source.ecer_InpersonHoursPercentage),
    ProgramCampuses = (source.ecer_ProgramApplicationId_ecer_postsecondaryinstituteprogramapplicaiton ?? Array.Empty<ecer_ProgramCampus>())
      .Select(MapProgramCampus)
      .ToList(),
    OtherAdmissionOptions = source.ecer_OtherAdmissionOptions,
    InstituteInfoEntryProgress = source.ecer_InstitutionProgramInformationEntryProgress?.ToString(),
    DeclarationDate = source.ecer_DateofApplicationShort,
    DeclarationAccepted = source.ecer_AgreeNotifyofChanges == ecer_YesNoNull.Yes,
    DeclarantName = source.ecer_SubmittedByProgramRepresentativeIdName,
    DeclarantId = source.ecer_SubmittedByProgramRepresentativeId?.Id.ToString(),
    ProgramProfileId = source.ecer_FromProgramProfileId?.Id.ToString(),
    ProgramProfileName = source.ecer_FromProgramProfileId?.Name,
    DeclarationText = source.ecer_DeclarationStatements,
    BasicProgress = source.ecer_BasicEntryProgress?.ToString(),
    IteProgress = source.ecer_ITEEntryProgress?.ToString(),
    SneProgress = source.ecer_SNEEntryProgress?.ToString(),
  };

  private NavigationMetadata MapNavigationMetadata(ecer_ProgramApplicationComponentGroup source) => new(
    source.ecer_ProgramApplicationComponentGroupId?.ToString() ?? string.Empty,
    source.ecer_GroupName ?? string.Empty,
    source.ecer_EntryProgress?.ToString() ?? string.Empty,
    source.ecer_categoryName ?? string.Empty,
    MapDisplayOrder(source.ecer_DisplayOrder),
    NavigationType.Component,
    source.ecer_RFAIRequired.HasValue ? source.ecer_RFAIRequired == ecer_YesNoNull.Yes : null);

  private ComponentGroupWithComponents MapComponentGroupWithComponents(ecer_ProgramApplicationComponentGroup source) => new(
    source.ecer_ProgramApplicationComponentGroupId?.ToString() ?? string.Empty,
    source.ecer_GroupName ?? string.Empty,
    source.ecer_programapplicationcomponentgroup_ComponentGroup?.ecer_Instructions,
    source.ecer_EntryProgress?.ToString() ?? string.Empty,
    source.ecer_categoryName ?? string.Empty,
    MapDisplayOrder(source.ecer_DisplayOrder),
    (source.ecer_programapplicationcomponent_ComponentGroup ?? Array.Empty<ecer_ProgramApplicationComponent>())
      .Select(MapProgramApplicationComponent)
      .ToList());

  private ProgramApplicationComponent MapProgramApplicationComponent(ecer_ProgramApplicationComponent source) => new(
    source.ecer_ProgramApplicationComponentId?.ToString() ?? string.Empty,
    source.ecer_Component ?? string.Empty,
    source.ecer_Question,
    MapDisplayOrder(source.ecer_DisplayOrder),
    source.ecer_Componentanswer,
    MapComponentFiles(source).ToList(),
    source.ecer_RFAIRequired.HasValue ? source.ecer_RFAIRequired == ecer_YesNoNull.Yes : null);

  private static List<FileInfo> MapComponentFiles(ecer_ProgramApplicationComponent source)
  {
    var sharedFiles = (source.ecer_sharedocumenturl_ProgramApplicationComponentId ?? Array.Empty<ecer_ShareDocumentURL>())
      .Select(MapFileInfo)
      .OfType<FileInfo>()
      .ToList();

    if (sharedFiles.Count > 0)
    {
      return sharedFiles;
    }

    return (source.ecer_documenturl_ProgramApplicationComponentId ?? Array.Empty<bcgov_DocumentUrl>())
      .Select(MapFileInfo)
      .ToList();
  }

  private static ProgramCampus MapProgramCampus(ecer_ProgramCampus source) => new()
  {
    Id = source.Id.ToString(),
    CampusId = source.ecer_CampusId?.Id.ToString(),
    Name = source.ecer_CampusIdName,
    StartDate = source.ecer_Startdate,
    EndDate = source.ecer_Enddate,
  };

  private static FileInfo? MapFileInfo(ecer_ShareDocumentURL source)
  {
    var documentUrl = source.ecer_sharedocumenturl_DocumentURLId;
    if (documentUrl?.bcgov_DocumentUrlId == null)
    {
      return null;
    }

    return new FileInfo(documentUrl.bcgov_DocumentUrlId.Value.ToString())
    {
      ShareDocumentUrlId = source.ecer_ShareDocumentURLId?.ToString(),
      Name = documentUrl.bcgov_FileName,
      Url = documentUrl.bcgov_Url,
      Size = documentUrl.bcgov_FileSize,
      Extension = documentUrl.bcgov_FileExtension,
      EcerWebApplicationType = ParseApplicationName(documentUrl.ecer_ApplicationName),
    };
  }

  private static FileInfo MapFileInfo(bcgov_DocumentUrl source) => new(source.bcgov_DocumentUrlId?.ToString() ?? string.Empty)
  {
    Name = source.bcgov_FileName,
    Url = source.bcgov_Url,
    Size = source.bcgov_FileSize,
    Extension = source.bcgov_FileExtension,
    EcerWebApplicationType = ParseApplicationName(source.ecer_ApplicationName),
  };

  private static EcerWebApplicationType ParseApplicationName(string? source) =>
    Enum.TryParse<EcerWebApplicationType>(source, out var value)
      ? value
      : EcerWebApplicationType.PSP;

  private static int MapDisplayOrder(string? source) => int.TryParse(source, out var value) ? value : 0;

  private static decimal? MapDecimal(float? source) => source.HasValue ? Convert.ToDecimal(source.Value) : null;

  private static float? MapFloat(decimal? source) => source.HasValue ? Convert.ToSingle(source.Value) : null;

  private static int? MapEnrollment(float? source) => source.HasValue ? Convert.ToInt32(source.Value) : null;

  private static float? MapEnrollment(int? source) => source.HasValue ? Convert.ToSingle(source.Value) : null;

  private ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode? MapApplicationStatus(ApplicationStatus? source) => source.HasValue ? MapApplicationStatus(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode MapApplicationStatus(ApplicationStatus source);

  private ApplicationStatus? MapApplicationStatus(ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode? source) => source.HasValue ? MapApplicationStatus(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationStatus MapApplicationStatus(ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode source);

  private ecer_Statusreasondetail? MapStatusReasonDetail(ApplicationStatusReasonDetail? source) => source.HasValue ? MapStatusReasonDetail(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_Statusreasondetail MapStatusReasonDetail(ApplicationStatusReasonDetail source);

  private ApplicationStatusReasonDetail? MapStatusReasonDetail(ecer_Statusreasondetail? source) => source.HasValue ? MapStatusReasonDetail(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationStatusReasonDetail MapStatusReasonDetail(ecer_Statusreasondetail source);

  private ecer_PSIApplicationType? MapApplicationType(ApplicationType? source) => source.HasValue ? MapApplicationType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSIApplicationType MapApplicationType(ApplicationType source);

  private ApplicationType? MapApplicationType(ecer_PSIApplicationType? source) => source.HasValue ? MapApplicationType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationType MapApplicationType(ecer_PSIApplicationType source);

  private ecer_PSIDeliveryType? MapDeliveryType(DeliveryType? source) => source.HasValue ? MapDeliveryType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSIDeliveryType MapDeliveryType(DeliveryType source);

  private DeliveryType? MapDeliveryType(ecer_PSIDeliveryType? source) => source.HasValue ? MapDeliveryType(source.Value) : null;

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial DeliveryType MapDeliveryType(ecer_PSIDeliveryType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSIProgramType MapProgramCertificationType(ProgramCertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ProgramCertificationType MapProgramCertificationType(ecer_PSIProgramType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSPMethodofInstruction MapMethodOfInstruction(MethodofInstruction source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial MethodofInstruction MapMethodOfInstruction(ecer_PSPMethodofInstruction source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSPDeliveryMethodforInstructor MapDeliveryMethod(DeliveryMethodforInstructor source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial DeliveryMethodforInstructor MapDeliveryMethod(ecer_PSPDeliveryMethodforInstructor source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_WorkHoursType MapWorkHoursType(WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial WorkHoursType MapWorkHoursType(ecer_WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ecer_PSPAdmissionOptions MapAdmissionOptions(AdmissionOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial AdmissionOptions MapAdmissionOptions(ecer_PSPAdmissionOptions source);
}
