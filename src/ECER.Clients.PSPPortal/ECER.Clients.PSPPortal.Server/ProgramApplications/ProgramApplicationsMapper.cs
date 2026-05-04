using ECER.Utilities.ObjectStorage.Providers;
using Riok.Mapperly.Abstractions;
using ContractProgramApplications = ECER.Managers.Registry.Contract.ProgramApplications;

namespace ECER.Clients.PSPPortal.Server.ProgramApplications;

internal interface IProgramApplicationsMapper
{
  ContractProgramApplications.ProgramApplication MapProgramApplication(ProgramApplication source);
  ProgramApplication MapProgramApplication(ContractProgramApplications.ProgramApplication source);
  IEnumerable<ProgramApplication> MapProgramApplications(IEnumerable<ContractProgramApplications.ProgramApplication> source);
  IEnumerable<NavigationMetadata> MapNavigationMetadata(IEnumerable<ContractProgramApplications.NavigationMetadata> source);
  IEnumerable<ComponentGroupWithComponents> MapComponentGroups(IEnumerable<ContractProgramApplications.ComponentGroupWithComponents> source);
  ContractProgramApplications.ComponentGroupWithComponents MapComponentGroup(ComponentGroupWithComponents source);
}

[Mapper]
internal partial class ProgramApplicationsMapper : IProgramApplicationsMapper
{
  public ContractProgramApplications.ProgramApplication MapProgramApplication(ProgramApplication source) => new(source.Id, source.PostSecondaryInstituteId)
  {
    ProgramApplicationName = source.ProgramApplicationName,
    ProgramApplicationType = MapApplicationType(source.ProgramApplicationType),
    Status = MapApplicationStatus(source.Status),
    StatusReasonDetail = MapApplicationStatusReasonDetail(source.StatusReasonDetail),
    ProgramTypes = source.ProgramTypes?.Select(MapProgramCertificationType).ToList(),
    DeliveryType = MapDeliveryType(source.DeliveryType),
    ComponentsGenerationCompleted = source.ComponentsGenerationCompleted,
    ProgramRepresentativeId = source.ProgramRepresentativeId,
    ProgramLength = source.ProgramLength,
    OnlineMethodOfInstruction = source.OnlineMethodOfInstruction?.Select(MapMethodOfInstruction).ToList(),
    DeliveryMethod = source.DeliveryMethod?.Select(MapDeliveryMethodForInstructor).ToList(),
    EnrollmentOptions = source.EnrollmentOptions?.Select(MapWorkHoursType).ToList(),
    AdmissionOptions = source.AdmissionOptions?.Select(MapAdmissionOptions).ToList(),
    MinimumEnrollment = source.MinimumEnrollment,
    MaximumEnrollment = source.MaximumEnrollment,
    InPersonHoursPercentage = source.InPersonHoursPercentage,
    OnlineDeliveryHoursPercentage = source.OnlineDeliveryHoursPercentage,
    ProgramCampuses = source.ProgramCampuses?.Select(MapProgramCampus).ToList(),
    OtherAdmissionOptions = source.OtherAdmissionOptions,
    InstituteInfoEntryProgress = source.InstituteInfoEntryProgress,
    DeclarationDate = source.DeclarationDate,
    DeclarationAccepted = source.DeclarationAccepted,
    DeclarantName = source.DeclarantName,
    ProgramProfileId = source.ProgramProfileId,
    ProgramProfileName = source.ProgramProfileName,
    DeclarationText = source.DeclarationText,
  };

  public ProgramApplication MapProgramApplication(ContractProgramApplications.ProgramApplication source) => new()
  {
    Id = source.Id,
    PostSecondaryInstituteId = source.PostSecondaryInstituteId,
    ProgramApplicationName = source.ProgramApplicationName,
    ProgramApplicationType = MapApplicationType(source.ProgramApplicationType),
    Status = MapApplicationStatus(source.Status),
    StatusReasonDetail = MapApplicationStatusReasonDetail(source.StatusReasonDetail),
    ProgramTypes = source.ProgramTypes?.Select(MapProgramCertificationType).ToList(),
    DeliveryType = MapDeliveryType(source.DeliveryType),
    ComponentsGenerationCompleted = source.ComponentsGenerationCompleted,
    ProgramRepresentativeId = source.ProgramRepresentativeId,
    ProgramLength = source.ProgramLength,
    OnlineMethodOfInstruction = source.OnlineMethodOfInstruction?.Select(MapMethodOfInstruction).ToList(),
    DeliveryMethod = source.DeliveryMethod?.Select(MapDeliveryMethodForInstructor).ToList(),
    EnrollmentOptions = source.EnrollmentOptions?.Select(MapWorkHoursType).ToList(),
    AdmissionOptions = source.AdmissionOptions?.Select(MapAdmissionOptions).ToList(),
    MinimumEnrollment = source.MinimumEnrollment,
    MaximumEnrollment = source.MaximumEnrollment,
    InPersonHoursPercentage = source.InPersonHoursPercentage,
    OnlineDeliveryHoursPercentage = source.OnlineDeliveryHoursPercentage,
    ProgramCampuses = source.ProgramCampuses?.Select(MapProgramCampus).ToList(),
    OtherAdmissionOptions = source.OtherAdmissionOptions,
    InstituteInfoEntryProgress = source.InstituteInfoEntryProgress,
    DeclarationDate = source.DeclarationDate,
    DeclarationAccepted = source.DeclarationAccepted,
    DeclarantName = source.DeclarantName,
    ProgramProfileId = source.ProgramProfileId,
    ProgramProfileName = source.ProgramProfileName,
    DeclarationText = source.DeclarationText,
  };

  public IEnumerable<ProgramApplication> MapProgramApplications(IEnumerable<ContractProgramApplications.ProgramApplication> source) => source.Select(MapProgramApplication).ToList();

  public IEnumerable<NavigationMetadata> MapNavigationMetadata(IEnumerable<ContractProgramApplications.NavigationMetadata> source) => source.Select(MapNavigationMetadata).ToList();

  public IEnumerable<ComponentGroupWithComponents> MapComponentGroups(IEnumerable<ContractProgramApplications.ComponentGroupWithComponents> source) => source.Select(MapComponentGroup).ToList();

  public ContractProgramApplications.ComponentGroupWithComponents MapComponentGroup(ComponentGroupWithComponents source) => new(
    source.Id,
    string.Empty,
    null,
    string.Empty,
    string.Empty,
    default,
    source.Components.Select(MapProgramApplicationComponent).ToList());

  private NavigationMetadata MapNavigationMetadata(ContractProgramApplications.NavigationMetadata source) => new(
    source.Id,
    source.Name,
    source.Status,
    source.CategoryName,
    source.DisplayOrder,
    MapNavigationType(source.NavigationType),
    source.RfaiRequired);

  private ComponentGroupWithComponents MapComponentGroup(ContractProgramApplications.ComponentGroupWithComponents source) => new(
    source.Id,
    source.Name,
    source.Instruction,
    source.Status,
    source.CategoryName,
    source.DisplayOrder,
    source.Components.Select(MapProgramApplicationComponent).ToList());

  private static ProgramCampus MapProgramCampus(ContractProgramApplications.ProgramCampus source) => new()
  {
    Id = source.Id,
    CampusId = source.CampusId,
    Name = source.Name,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
  };

  private static ContractProgramApplications.ProgramCampus MapProgramCampus(ProgramCampus source) => new()
  {
    Id = source.Id,
    CampusId = source.CampusId,
    Name = source.Name,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
  };

  private ProgramApplicationComponent MapProgramApplicationComponent(ContractProgramApplications.ProgramApplicationComponent source) => new(
    source.Id,
    source.Name,
    source.Question,
    source.DisplayOrder,
    source.Answer,
    source.Files?.Select(MapFileInfo).ToList(),
    source.RfaiRequired)
  {
    NewFiles = source.NewFiles.Select(MapFileInfo).ToList(),
    DeletedFiles = source.DeletedFiles.Select(MapFileInfo).ToList(),
  };

  private ContractProgramApplications.ProgramApplicationComponent MapProgramApplicationComponent(ProgramApplicationComponent source) => new(
    source.Id,
    string.Empty,
    null,
    default,
    source.Answer,
    null,
    source.RfaiRequired)
  {
    NewFiles = source.NewFiles.Select(MapFileInfo).ToList(),
    DeletedFiles = source.DeletedFiles.Select(MapFileInfo).ToList(),
  };

  private static FileInfo MapFileInfo(ContractProgramApplications.FileInfo source) => new(source.Id)
  {
    Name = source.Name,
    Url = source.Url,
    Size = source.Size,
    Extension = source.Extension,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  private static ContractProgramApplications.FileInfo MapFileInfo(FileInfo source) => new(source.Id)
  {
    Name = source.Name,
    Url = source.Url,
    Size = source.Size,
    Extension = source.Extension,
    EcerWebApplicationType = EcerWebApplicationType.PSP,
  };

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial MethodofInstruction MapMethodOfInstruction(ContractProgramApplications.MethodofInstruction source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.MethodofInstruction MapMethodOfInstruction(MethodofInstruction source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial DeliveryMethodforInstructor MapDeliveryMethodForInstructor(ContractProgramApplications.DeliveryMethodforInstructor source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.DeliveryMethodforInstructor MapDeliveryMethodForInstructor(DeliveryMethodforInstructor source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial WorkHoursType MapWorkHoursType(ContractProgramApplications.WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.WorkHoursType MapWorkHoursType(WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial AdmissionOptions MapAdmissionOptions(ContractProgramApplications.AdmissionOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.AdmissionOptions MapAdmissionOptions(AdmissionOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationStatus MapApplicationStatus(ContractProgramApplications.ApplicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ApplicationStatus MapApplicationStatus(ApplicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationStatusReasonDetail MapApplicationStatusReasonDetail(ContractProgramApplications.ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ApplicationStatusReasonDetail MapApplicationStatusReasonDetail(ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ApplicationType MapApplicationType(ContractProgramApplications.ApplicationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ApplicationType MapApplicationType(ApplicationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial DeliveryType MapDeliveryType(ContractProgramApplications.DeliveryType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.DeliveryType MapDeliveryType(DeliveryType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ProgramCertificationType MapProgramCertificationType(ContractProgramApplications.ProgramCertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ProgramCertificationType MapProgramCertificationType(ProgramCertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial NavigationType MapNavigationType(ContractProgramApplications.NavigationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.NavigationType MapNavigationType(NavigationType source);

  private MethodofInstruction? MapMethodOfInstruction(ContractProgramApplications.MethodofInstruction? source) => source.HasValue ? MapMethodOfInstruction(source.Value) : null;

  private ContractProgramApplications.MethodofInstruction? MapMethodOfInstruction(MethodofInstruction? source) => source.HasValue ? MapMethodOfInstruction(source.Value) : null;

  private DeliveryMethodforInstructor? MapDeliveryMethodForInstructor(ContractProgramApplications.DeliveryMethodforInstructor? source) => source.HasValue ? MapDeliveryMethodForInstructor(source.Value) : null;

  private ContractProgramApplications.DeliveryMethodforInstructor? MapDeliveryMethodForInstructor(DeliveryMethodforInstructor? source) => source.HasValue ? MapDeliveryMethodForInstructor(source.Value) : null;

  private WorkHoursType? MapWorkHoursType(ContractProgramApplications.WorkHoursType? source) => source.HasValue ? MapWorkHoursType(source.Value) : null;

  private ContractProgramApplications.WorkHoursType? MapWorkHoursType(WorkHoursType? source) => source.HasValue ? MapWorkHoursType(source.Value) : null;

  private AdmissionOptions? MapAdmissionOptions(ContractProgramApplications.AdmissionOptions? source) => source.HasValue ? MapAdmissionOptions(source.Value) : null;

  private ContractProgramApplications.AdmissionOptions? MapAdmissionOptions(AdmissionOptions? source) => source.HasValue ? MapAdmissionOptions(source.Value) : null;

  private ApplicationStatus? MapApplicationStatus(ContractProgramApplications.ApplicationStatus? source) => source.HasValue ? MapApplicationStatus(source.Value) : null;

  private ContractProgramApplications.ApplicationStatus? MapApplicationStatus(ApplicationStatus? source) => source.HasValue ? MapApplicationStatus(source.Value) : null;

  private ApplicationStatusReasonDetail? MapApplicationStatusReasonDetail(ContractProgramApplications.ApplicationStatusReasonDetail? source) => source.HasValue ? MapApplicationStatusReasonDetail(source.Value) : null;

  private ContractProgramApplications.ApplicationStatusReasonDetail? MapApplicationStatusReasonDetail(ApplicationStatusReasonDetail? source) => source.HasValue ? MapApplicationStatusReasonDetail(source.Value) : null;

  private ApplicationType? MapApplicationType(ContractProgramApplications.ApplicationType? source) => source.HasValue ? MapApplicationType(source.Value) : null;

  private ContractProgramApplications.ApplicationType? MapApplicationType(ApplicationType? source) => source.HasValue ? MapApplicationType(source.Value) : null;

  private DeliveryType? MapDeliveryType(ContractProgramApplications.DeliveryType? source) => source.HasValue ? MapDeliveryType(source.Value) : null;

  private ContractProgramApplications.DeliveryType? MapDeliveryType(DeliveryType? source) => source.HasValue ? MapDeliveryType(source.Value) : null;

  private ProgramCertificationType? MapProgramCertificationType(ContractProgramApplications.ProgramCertificationType? source) => source.HasValue ? MapProgramCertificationType(source.Value) : null;

  private ContractProgramApplications.ProgramCertificationType? MapProgramCertificationType(ProgramCertificationType? source) => source.HasValue ? MapProgramCertificationType(source.Value) : null;
}
