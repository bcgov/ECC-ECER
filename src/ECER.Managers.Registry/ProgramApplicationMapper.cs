using Riok.Mapperly.Abstractions;
using ContractProgramApplications = ECER.Managers.Registry.Contract.ProgramApplications;
using ResourceProgramApplications = ECER.Resources.Documents.ProgramApplications;

namespace ECER.Managers.Registry;

public interface IProgramApplicationMapper
{
  ResourceProgramApplications.ProgramApplication MapProgramApplication(ContractProgramApplications.ProgramApplication source);
  ContractProgramApplications.ProgramApplication? MapProgramApplication(ResourceProgramApplications.ProgramApplication? source);
  IEnumerable<ContractProgramApplications.ProgramApplication> MapProgramApplications(IEnumerable<ResourceProgramApplications.ProgramApplication> source);
  IEnumerable<ResourceProgramApplications.ApplicationStatus> MapApplicationStatuses(IEnumerable<ContractProgramApplications.ApplicationStatus> source);
  IEnumerable<ContractProgramApplications.NavigationMetadata> MapNavigationMetadata(IEnumerable<ResourceProgramApplications.NavigationMetadata> source);
  IEnumerable<ContractProgramApplications.ComponentGroupWithComponents> MapComponentGroupsWithComponents(IEnumerable<ResourceProgramApplications.ComponentGroupWithComponents> source);
  ResourceProgramApplications.ComponentGroupWithComponents MapComponentGroupWithComponents(ContractProgramApplications.ComponentGroupWithComponents source);
}

[Mapper]
internal partial class ProgramApplicationMapper : IProgramApplicationMapper
{
  public ResourceProgramApplications.ProgramApplication MapProgramApplication(ContractProgramApplications.ProgramApplication source) => new(source.Id, source.PostSecondaryInstituteId)
  {
    ProgramApplicationName = source.ProgramApplicationName,
    ProgramApplicationType = MapApplicationType(source.ProgramApplicationType),
    Status = MapApplicationStatus(source.Status),
    StatusReasonDetail = MapStatusReasonDetail(source.StatusReasonDetail),
    ProgramTypes = source.ProgramTypes?.Select(MapProgramCertificationType).ToList(),
    DeliveryType = MapDeliveryType(source.DeliveryType),
    ComponentsGenerationCompleted = source.ComponentsGenerationCompleted,
    ProgramRepresentativeId = source.ProgramRepresentativeId,
    ProgramLength = MapNumber(source.ProgramLength),
    OnlineMethodOfInstruction = source.OnlineMethodOfInstruction?.Select(MapMethodOfInstruction).ToList(),
    DeliveryMethod = source.DeliveryMethod?.Select(MapDeliveryMethod).ToList(),
    EnrollmentOptions = source.EnrollmentOptions?.Select(MapWorkHoursType).ToList(),
    AdmissionOptions = source.AdmissionOptions?.Select(MapAdmissionOptions).ToList(),
    MinimumEnrollment = MapNumber(source.MinimumEnrollment),
    MaximumEnrollment = MapNumber(source.MaximumEnrollment),
    InPersonHoursPercentage = source.InPersonHoursPercentage,
    OnlineDeliveryHoursPercentage = source.OnlineDeliveryHoursPercentage,
    ProgramCampuses = source.ProgramCampuses?.Select(MapProgramCampus).ToList(),
    OtherAdmissionOptions = source.OtherAdmissionOptions,
    InstituteInfoEntryProgress = source.InstituteInfoEntryProgress,
    DeclarationDate = source.DeclarationDate,
    DeclarationAccepted = source.DeclarationAccepted,
    DeclarantName = source.DeclarantName,
    DeclarantId = source.DeclarantId,
    ProgramProfileId = source.ProgramProfileId,
    ProgramProfileName = source.ProgramProfileName,
    DeclarationText = source.DeclarationText,
    BasicProgress = source.BasicProgress,
    IteProgress = source.IteProgress,
    SneProgress = source.SneProgress,
  };

  public ContractProgramApplications.ProgramApplication? MapProgramApplication(ResourceProgramApplications.ProgramApplication? source) => source == null ? null : new ContractProgramApplications.ProgramApplication(source.Id, source.PostSecondaryInstituteId)
  {
    ProgramApplicationName = source.ProgramApplicationName,
    ProgramApplicationType = MapApplicationType(source.ProgramApplicationType),
    Status = MapApplicationStatus(source.Status),
    StatusReasonDetail = MapStatusReasonDetail(source.StatusReasonDetail),
    ProgramTypes = source.ProgramTypes?.Select(MapProgramCertificationType).ToList(),
    DeliveryType = MapDeliveryType(source.DeliveryType),
    ComponentsGenerationCompleted = source.ComponentsGenerationCompleted,
    ProgramRepresentativeId = source.ProgramRepresentativeId,
    ProgramLength = MapNumber(source.ProgramLength),
    OnlineMethodOfInstruction = source.OnlineMethodOfInstruction?.Select(MapMethodOfInstruction).ToList(),
    DeliveryMethod = source.DeliveryMethod?.Select(MapDeliveryMethod).ToList(),
    EnrollmentOptions = source.EnrollmentOptions?.Select(MapWorkHoursType).ToList(),
    AdmissionOptions = source.AdmissionOptions?.Select(MapAdmissionOptions).ToList(),
    MinimumEnrollment = MapNumber(source.MinimumEnrollment),
    MaximumEnrollment = MapNumber(source.MaximumEnrollment),
    InPersonHoursPercentage = source.InPersonHoursPercentage,
    OnlineDeliveryHoursPercentage = source.OnlineDeliveryHoursPercentage,
    ProgramCampuses = source.ProgramCampuses?.Select(MapProgramCampus).ToList(),
    OtherAdmissionOptions = source.OtherAdmissionOptions,
    InstituteInfoEntryProgress = source.InstituteInfoEntryProgress,
    DeclarationDate = source.DeclarationDate,
    DeclarationAccepted = source.DeclarationAccepted,
    DeclarantName = source.DeclarantName,
    DeclarantId = source.DeclarantId,
    ProgramProfileId = source.ProgramProfileId,
    ProgramProfileName = source.ProgramProfileName,
    DeclarationText = source.DeclarationText,
    BasicProgress = source.BasicProgress,
    IteProgress = source.IteProgress,
    SneProgress = source.SneProgress,
  };

  public IEnumerable<ContractProgramApplications.ProgramApplication> MapProgramApplications(IEnumerable<ResourceProgramApplications.ProgramApplication> source) => source.Select(programApplication => MapProgramApplication(programApplication)!).ToList();

  public IEnumerable<ResourceProgramApplications.ApplicationStatus> MapApplicationStatuses(IEnumerable<ContractProgramApplications.ApplicationStatus> source) => source.Select(MapApplicationStatus).ToList();

  public IEnumerable<ContractProgramApplications.NavigationMetadata> MapNavigationMetadata(IEnumerable<ResourceProgramApplications.NavigationMetadata> source) => source.Select(MapNavigationMetadata).ToList();

  public IEnumerable<ContractProgramApplications.ComponentGroupWithComponents> MapComponentGroupsWithComponents(IEnumerable<ResourceProgramApplications.ComponentGroupWithComponents> source) => source.Select(MapComponentGroupWithComponents).ToList();

  public ResourceProgramApplications.ComponentGroupWithComponents MapComponentGroupWithComponents(ContractProgramApplications.ComponentGroupWithComponents source) => new(
    source.Id,
    source.Name,
    source.Instruction,
    source.Status,
    source.CategoryName,
    source.DisplayOrder,
    source.Components.Select(MapProgramApplicationComponent).ToList());

  private ContractProgramApplications.NavigationMetadata MapNavigationMetadata(ResourceProgramApplications.NavigationMetadata source) => new(
    source.Id,
    source.Name,
    source.Status,
    source.CategoryName,
    source.DisplayOrder,
    MapNavigationType(source.NavigationType),
    source.RfaiRequired);

  private ContractProgramApplications.ComponentGroupWithComponents MapComponentGroupWithComponents(ResourceProgramApplications.ComponentGroupWithComponents source) => new(
    source.Id,
    source.Name,
    source.Instruction,
    source.Status,
    source.CategoryName,
    source.DisplayOrder,
    source.Components.Select(MapProgramApplicationComponent).ToList());

  private static ResourceProgramApplications.ProgramCampus MapProgramCampus(ContractProgramApplications.ProgramCampus source) => new()
  {
    Id = source.Id,
    CampusId = source.CampusId,
    Name = source.Name,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
  };

  private static ContractProgramApplications.ProgramCampus MapProgramCampus(ResourceProgramApplications.ProgramCampus source) => new()
  {
    Id = source.Id,
    CampusId = source.CampusId,
    Name = source.Name,
    StartDate = source.StartDate,
    EndDate = source.EndDate,
  };

  private ResourceProgramApplications.ProgramApplicationComponent MapProgramApplicationComponent(ContractProgramApplications.ProgramApplicationComponent source) => new(
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

  private ContractProgramApplications.ProgramApplicationComponent MapProgramApplicationComponent(ResourceProgramApplications.ProgramApplicationComponent source) => new(
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

  private static ResourceProgramApplications.FileInfo MapFileInfo(ContractProgramApplications.FileInfo source) => new(source.Id)
  {
    Name = source.Name,
    Url = source.Url,
    Size = source.Size,
    Extension = source.Extension,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  private static ContractProgramApplications.FileInfo MapFileInfo(ResourceProgramApplications.FileInfo source) => new(source.Id)
  {
    Name = source.Name,
    Url = source.Url,
    Size = source.Size,
    Extension = source.Extension,
    EcerWebApplicationType = source.EcerWebApplicationType,
  };

  private static float? MapNumber(string? source) => string.IsNullOrWhiteSpace(source) ? null : Convert.ToSingle(source);

  private static string? MapNumber(float? source) => source?.ToString();

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.ApplicationStatus MapApplicationStatus(ContractProgramApplications.ApplicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ApplicationStatus MapApplicationStatus(ResourceProgramApplications.ApplicationStatus source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.ApplicationStatusReasonDetail MapStatusReasonDetail(ContractProgramApplications.ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ApplicationStatusReasonDetail MapStatusReasonDetail(ResourceProgramApplications.ApplicationStatusReasonDetail source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.ApplicationType MapApplicationType(ContractProgramApplications.ApplicationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ApplicationType MapApplicationType(ResourceProgramApplications.ApplicationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.DeliveryType MapDeliveryType(ContractProgramApplications.DeliveryType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.DeliveryType MapDeliveryType(ResourceProgramApplications.DeliveryType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.ProgramCertificationType MapProgramCertificationType(ContractProgramApplications.ProgramCertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.ProgramCertificationType MapProgramCertificationType(ResourceProgramApplications.ProgramCertificationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.NavigationType MapNavigationType(ContractProgramApplications.NavigationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.NavigationType MapNavigationType(ResourceProgramApplications.NavigationType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.MethodofInstruction MapMethodOfInstruction(ContractProgramApplications.MethodofInstruction source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.MethodofInstruction MapMethodOfInstruction(ResourceProgramApplications.MethodofInstruction source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.DeliveryMethodforInstructor MapDeliveryMethod(ContractProgramApplications.DeliveryMethodforInstructor source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.DeliveryMethodforInstructor MapDeliveryMethod(ResourceProgramApplications.DeliveryMethodforInstructor source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.WorkHoursType MapWorkHoursType(ContractProgramApplications.WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.WorkHoursType MapWorkHoursType(ResourceProgramApplications.WorkHoursType source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ResourceProgramApplications.AdmissionOptions MapAdmissionOptions(ContractProgramApplications.AdmissionOptions source);

  [MapEnum(EnumMappingStrategy.ByName)]
  private partial ContractProgramApplications.AdmissionOptions MapAdmissionOptions(ResourceProgramApplications.AdmissionOptions source);

  private ResourceProgramApplications.ApplicationStatus? MapApplicationStatus(ContractProgramApplications.ApplicationStatus? source) => source.HasValue ? MapApplicationStatus(source.Value) : null;

  private ContractProgramApplications.ApplicationStatus? MapApplicationStatus(ResourceProgramApplications.ApplicationStatus? source) => source.HasValue ? MapApplicationStatus(source.Value) : null;

  private ResourceProgramApplications.ApplicationStatusReasonDetail? MapStatusReasonDetail(ContractProgramApplications.ApplicationStatusReasonDetail? source) => source.HasValue ? MapStatusReasonDetail(source.Value) : null;

  private ContractProgramApplications.ApplicationStatusReasonDetail? MapStatusReasonDetail(ResourceProgramApplications.ApplicationStatusReasonDetail? source) => source.HasValue ? MapStatusReasonDetail(source.Value) : null;

  private ResourceProgramApplications.ApplicationType? MapApplicationType(ContractProgramApplications.ApplicationType? source) => source.HasValue ? MapApplicationType(source.Value) : null;

  private ContractProgramApplications.ApplicationType? MapApplicationType(ResourceProgramApplications.ApplicationType? source) => source.HasValue ? MapApplicationType(source.Value) : null;

  private ResourceProgramApplications.DeliveryType? MapDeliveryType(ContractProgramApplications.DeliveryType? source) => source.HasValue ? MapDeliveryType(source.Value) : null;

  private ContractProgramApplications.DeliveryType? MapDeliveryType(ResourceProgramApplications.DeliveryType? source) => source.HasValue ? MapDeliveryType(source.Value) : null;
}
