using MediatR;

namespace ECER.Managers.Registry.Contract.ProgramApplications;

public record CreateProgramApplicationCommand(ProgramApplication ProgramApplication) : IRequest<ProgramApplication?>;

public record ProgramApplicationQuery : IRequest<ProgramApplicationQueryResults>
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
  public string? ByCampusId { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record ComponentGroupQuery : IRequest<IEnumerable<NavigationMetadata>>
{
  public string? ByProgramApplicationId { get; set; }
}

public record NavigationMetadata(string Id, string Name, string Status, string CategoryName, int DisplayOrder, NavigationType NavigationType);
public enum NavigationType
{
  Component,
  Other,
}
public record ComponentGroupWithComponents(string Id, string Name, string? Instruction, string Status, string CategoryName, int DisplayOrder, IEnumerable<ProgramApplicationComponent> Components);

public record ComponentGroupWithComponentsQuery : IRequest<IEnumerable<ComponentGroupWithComponents>>
{
  public string? ByProgramApplicationId { get; set; }
  public string? ByComponentGroupId { get; set; }
}

public record ProgramApplicationComponent(string Id, string Name, string? Question, int DisplayOrder, string? Answer, IEnumerable<FileInfo>? Files);

public record FileInfo(string Id)
{
  public string? Name { get; set; }
  public string? Url { get; set; }
  public string? Size { get; set; }
  public string? Extension { get; set; }
}

public record UpdateComponentGroupCommand(ComponentGroupWithComponents ComponentGroup, string ProgramApplicationId) : IRequest<string>;

public record UpdateProgramApplicationCommand(ProgramApplication ProgramApplication) : IRequest<string>;

public record ProgramApplication(string? Id, string PostSecondaryInstituteId)
{
  public string? ProgramApplicationName { get; set; }
  public ApplicationType? ProgramApplicationType { get; set; }
  public ApplicationStatus? Status { get; set; }
  public ApplicationStatusReasonDetail? StatusReasonDetail { get; set; }
  public IEnumerable<ProgramCertificationType>? ProgramTypes { get; set; }
  public DeliveryType? DeliveryType { get; set; }
  public bool? ComponentsGenerationCompleted { get; set; }
  public string? ProgramRepresentativeId { get; set; }
  public string? ProgramLength { get; set; }
  public IEnumerable<MethodofInstruction>? OnlineMethodOfInstruction { get; set; }
  public IEnumerable<DeliveryMethodforInstructor>? DeliveryMethod { get; set; }
  public IEnumerable<WorkHoursType>? EnrollmentOptions { get; set; }
  public IEnumerable<AdmissionOptions>? AdmissionOptions { get; set; }
  public string? MinimumEnrollment { get; set; }
  public string? MaximumEnrollment { get; set; }
  public IEnumerable<ProgramCampus>? ProgramCampuses { get; set; }
  public string? OtherAdmissionOptions  { get; set; }
  public string? InstituteInfoEntryProgress { get; set; }
}

public record ProgramCampus
{ 
  public string? Id { get; set; }
  public string? CampusId { get; set; }
}
public enum AdmissionOptions
{
  Allcoursesrestrictedtoearlychildhoodeducationstudents,
  Cohortenrollmentstudentsstarttogetherandgraduatetogether,
  Continuousenrollmentstudentscanenrolatanytime,
  Oneormorecoursesopentoanystudentsintheinstitution,
  Other,
}

public enum WorkHoursType
{
  FullTime,
  PartTime,
}

public enum MethodofInstruction
{
  Asynchronous,
  Synchronous,
}

public enum DeliveryMethodforInstructor
{
  Inpersonsitevisits,
  Virtualsitevisits,
}


public record ProgramApplicationQueryResults(IEnumerable<ProgramApplication> Items, int Count);
public enum ApplicationStatus
{
  Approved,
  Archived,
  Denied,
  Draft,
  Inactive,
  InterimRecognition,
  OnGoingRecognition,
  RefusetoApprove,
  ReviewAnalysis,
  Submitted,
  Withdrawn
}

public enum ApplicationStatusReasonDetail
{
  Pendingdecision,
  Recognitionevaluationmeeting,
  RFAIreceived,
  RFAIrequested,
}

public enum ApplicationType
{
  AddOnlineorHybridDeliveryMethod,
  CurriculumRevisionsatRecognizedInstitution,
  NewBasicECEPostBasicProgram,
  NewCampusatRecognizedPrivateInstitution,
  SatelliteProgram,
  WorkIntegratedLearningProgram,
}

public enum DeliveryType
{
  Hybrid,
  Inperson,
  Online,
}

public enum ProvincialCertificationTypeOffered
{
  ECEBasic,
  ITE,
  ITESNE,
  SNE
}

public enum ProgramCertificationType
{
  Basic,
  ITE,
  SNE
}
