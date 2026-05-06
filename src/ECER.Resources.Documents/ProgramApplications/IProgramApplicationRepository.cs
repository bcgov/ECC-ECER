using ECER.Utilities.ObjectStorage.Providers;

namespace ECER.Resources.Documents.ProgramApplications;

public interface IProgramApplicationRepository
{
  Task<string> Create(ProgramApplication programApplication, CancellationToken cancellationToken);

  Task<ProgramApplicationQueryResults> Query(ProgramApplicationQuery query, CancellationToken cancellationToken);

  Task<string> UpdateProgramApplication(ProgramApplication application, CancellationToken cancellationToken);

  Task<IEnumerable<NavigationMetadata>> QueryComponentGroups(ComponentGroupQuery query, CancellationToken cancellationToken);

  Task<IEnumerable<ComponentGroupWithComponents>> QueryComponentGroupWithComponents(ComponentGroupWithComponentsQuery query, CancellationToken cancellationToken);

  Task<string> UpdateComponentGroup(ComponentGroupWithComponents componentGroupToUpdate, string applicationId, CancellationToken cancellationToken);

  Task<string> Submit(string applicationId, string programRepresentativeId, bool declaration, CancellationToken cancellationToken);

  Task UpdateCourseProgress(string applicationId, string? basicProgress, string? iteProgress, string? sneProgress, CancellationToken cancellationToken);

  Task<ApplicationFileInfo> CreateDocumentUrlAndShare(string fileId, string fileName, string fileSize, string folder, string programApplicationId, string componentGroupId, string componentId, string instituteId, CancellationToken cancellationToken);

  Task<ApplicationFileInfo> CreateShareOnly(string documentId, string programApplicationId, string componentGroupId, string componentId, CancellationToken cancellationToken);

  Task<IEnumerable<ApplicationFileInfo>> GetApplicationFiles(string programApplicationId, CancellationToken cancellationToken);

  Task<IEnumerable<ApplicationFileInfo>> GetApplicationDocumentUrls(string programApplicationId, CancellationToken cancellationToken);

  Task<ShareDocumentUrlDetails> GetShareDocumentUrlDetails(string shareDocumentId, CancellationToken cancellationToken);

  Task DeleteShareDocumentUrlById(string shareDocumentId, CancellationToken cancellationToken);
}

public record ApplicationFileInfo(string DocumentId, string ShareDocumentId, string FileName, string FileSize, string StorageFolder, string? Extension, EcerWebApplicationType EcerWebApplicationType);

public record ShareDocumentUrlDetails(string DocumentId, string Folder, EcerWebApplicationType EcerWebApplicationType, int RemainingShareCount);

public record ComponentGroupWithComponents(string Id, string Name, string? Instruction, string Status, string CategoryName, int DisplayOrder, IEnumerable<ProgramApplicationComponent> Components);

public record NavigationMetadata(string Id, string Name, string Status, string CategoryName, int DisplayOrder, NavigationType NavigationType, bool? RfaiRequired);
public record ComponentGroupQuery
{
  public string? ByProgramApplicationId { get; set; }
}

public record ComponentGroupWithComponentsQuery
{
  public string? ByProgramApplicationId { get; set; }
  public string? ByComponentGroupId { get; set; }
}

public record ProgramApplicationComponent(string Id, string Name, string? Question, int DisplayOrder, string? Answer, IEnumerable<FileInfo>? Files, bool? RfaiRequired)
{
  public IEnumerable<FileInfo> NewFiles { get; set; } = Array.Empty<FileInfo>();
  public IEnumerable<FileInfo> DeletedFiles { get; set; } = Array.Empty<FileInfo>();
}

public record FileInfo(string Id)
{
  public string? ShareDocumentUrlId { get; set; }
  public string? Name { get; set; }
  public string? Url { get; set; }
  public string? Size { get; set; }
  public string? Extension { get; set; }
  public EcerWebApplicationType EcerWebApplicationType { get; set; }
}

public record ProgramApplicationQuery
{
  public string? ById { get; set; }
  public string? ByPostSecondaryInstituteId { get; set; }
  public IEnumerable<ApplicationStatus>? ByStatus { get; set; }
  public string? ByCampusId { get; set; }
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
}

public record ProgramApplicationQueryResults(IEnumerable<ProgramApplication> Items, int Count);

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
  public float? ProgramLength { get; set; }
  public IEnumerable<MethodofInstruction>? OnlineMethodOfInstruction { get; set; }
  public IEnumerable<DeliveryMethodforInstructor>? DeliveryMethod { get; set; }
  public IEnumerable<WorkHoursType>? EnrollmentOptions { get; set; }
  public IEnumerable<AdmissionOptions>? AdmissionOptions { get; set; }
  public float? MinimumEnrollment { get; set; }
  public float? MaximumEnrollment { get; set; }
  public float? InPersonHoursPercentage { get; set; }
  public float? OnlineDeliveryHoursPercentage { get; set; }
  public IEnumerable<ProgramCampus>? ProgramCampuses { get; set; }
  public string? OtherAdmissionOptions { get; set; }
  public string? InstituteInfoEntryProgress { get; set; }
  public DateTime? DeclarationDate { get; set; }
  public bool? DeclarationAccepted { get; set; }
  public string? DeclarantName { get; set; }
  public string? DeclarantId { get; set; }
  public string? ProgramProfileId { get; set; }
  public string? ProgramProfileName { get; set; }
  public string? DeclarationText { get; set; }
  public string? BasicProgress { get; set; }
  public string? IteProgress { get; set; }
  public string? SneProgress { get; set; }
}

public record ProgramCampus
{
  public string? Id { get; set; }
  public string? CampusId { get; set; }
  public string? Name { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
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

public enum NavigationType
{
  Component,
  Other,
}
