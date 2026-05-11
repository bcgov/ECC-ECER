using ECER.Engines.Validation.ProgramApplications;
using ECER.Infrastructure.Common;
using ECER.Managers.Admin.Contract.Files;
using ECER.Managers.Registry.Contract.ProgramApplications;
using ECER.Resources.Documents.Courses;
using ECER.Resources.Documents.MetadataResources;
using ECER.Resources.Documents.ProgramApplications;
using MediatR;
using ApplicationFileInfo = ECER.Managers.Registry.Contract.ProgramApplications.ApplicationFileInfo;
using ComponentGroupQuery = ECER.Managers.Registry.Contract.ProgramApplications.ComponentGroupQuery;
using ComponentGroupWithComponents = ECER.Managers.Registry.Contract.ProgramApplications.ComponentGroupWithComponents;
using ComponentGroupWithComponentsQuery = ECER.Managers.Registry.Contract.ProgramApplications.ComponentGroupWithComponentsQuery;
using CreateProgramApplicationCommand = ECER.Managers.Registry.Contract.ProgramApplications.CreateProgramApplicationCommand;
using NavigationMetadata = ECER.Managers.Registry.Contract.ProgramApplications.NavigationMetadata;
using NavigationType = ECER.Managers.Registry.Contract.ProgramApplications.NavigationType;
using ProgramApplicationQuery = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQuery;
using ProgramApplicationQueryResults = ECER.Managers.Registry.Contract.ProgramApplications.ProgramApplicationQueryResults;

namespace ECER.Managers.Registry;

public class ProgramApplicationHandler(
    IProgramApplicationRepository programApplicationRepository,
    IMetadataResourceRepository metadataResourceRepository,
    ICourseRepository courseRepository,
    IProgramApplicationValidationEngine validationEngine,
    IProgramApplicationMapper programApplicationMapper,
    ICoursesMapper coursesMapper,
    IMediator mediator)
  : IRequestHandler<CreateProgramApplicationCommand, Contract.ProgramApplications.ProgramApplication?>,
    IRequestHandler<ProgramApplicationQuery, ProgramApplicationQueryResults>,
    IRequestHandler<UpdateProgramApplicationCommand, string>,
    IRequestHandler<ComponentGroupQuery, IEnumerable<NavigationMetadata>>,
    IRequestHandler<ComponentGroupWithComponentsQuery, IEnumerable<ComponentGroupWithComponents>>,
    IRequestHandler<UpdateComponentGroupCommand, string>,
    IRequestHandler<SubmitProgramApplicationCommand, ProgramApplicationSubmissionResult>,
    IRequestHandler<UploadProgramApplicationFileCommand, ApplicationFileInfo>,
    IRequestHandler<ShareExistingDocumentCommand, ApplicationFileInfo>,
    IRequestHandler<ProgramApplicationFilesQuery, IEnumerable<ApplicationFileInfo>>,
    IRequestHandler<ProgramApplicationDocumentUrlsQuery, IEnumerable<ApplicationFileInfo>>,
    IRequestHandler<DeleteProgramApplicationFileCommand>
{
  public async Task<Contract.ProgramApplications.ProgramApplication?> Handle(CreateProgramApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var resourcesApplication = programApplicationMapper.MapProgramApplication(request.ProgramApplication);
    var id = await programApplicationRepository.Create(resourcesApplication, cancellationToken);

    var result = await programApplicationRepository.Query(new Resources.Documents.ProgramApplications.ProgramApplicationQuery
    {
      ById = id,
      ByPostSecondaryInstituteId = request.ProgramApplication.PostSecondaryInstituteId,
    }, cancellationToken);

    var created = result.Items.FirstOrDefault();
    return programApplicationMapper.MapProgramApplication(created);
  }

  public async Task<string> Handle(UpdateProgramApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    return await programApplicationRepository.UpdateProgramApplication(programApplicationMapper.MapProgramApplication(request.ProgramApplication), cancellationToken);
  }

  public async Task<ProgramApplicationQueryResults> Handle(ProgramApplicationQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var statusFilter = request.ByStatus != null
      ? programApplicationMapper.MapApplicationStatuses(request.ByStatus)
      : null;

    var result = await programApplicationRepository.Query(new Resources.Documents.ProgramApplications.ProgramApplicationQuery
    {
      ById = request.ById,
      ByPostSecondaryInstituteId = request.ByPostSecondaryInstituteId,
      ByStatus = statusFilter,
      ByCampusId = request.ByCampusId,
      PageNumber = request.PageNumber,
      PageSize = request.PageSize,
    }, cancellationToken);

    return new ProgramApplicationQueryResults(programApplicationMapper.MapProgramApplications(result.Items), result.Count);
  }

  public async Task<IEnumerable<NavigationMetadata>> Handle(ComponentGroupQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var result = await programApplicationRepository.QueryComponentGroups(new Resources.Documents.ProgramApplications.ComponentGroupQuery
    {
      ByProgramApplicationId = request.ByProgramApplicationId,
    }, cancellationToken);

    var programApplicationResults = await programApplicationRepository.Query(new Resources.Documents.ProgramApplications.ProgramApplicationQuery
    {
      ById = request.ByProgramApplicationId,
    }, cancellationToken);

    var programApplication = programApplicationResults.Items.FirstOrDefault();
    var mappedComponentResults = programApplicationMapper.MapNavigationMetadata(result).ToList();

    if (programApplication != null)
    {
      var status = programApplication.InstituteInfoEntryProgress ?? "ToDo";
      mappedComponentResults.Add(new NavigationMetadata(Guid.NewGuid().ToString(), "Institution and program info", status, "Institute Info", 0, NavigationType.Other, false));

      if (programApplication.ProgramTypes?.Contains(Resources.Documents.ProgramApplications.ProgramCertificationType.Basic) == true)
        mappedComponentResults.Add(new NavigationMetadata(Guid.NewGuid().ToString(), "Basic", programApplication.BasicProgress ?? "ToDo", "Program Profile", 97, NavigationType.Other, false));
      if (programApplication.ProgramTypes?.Contains(Resources.Documents.ProgramApplications.ProgramCertificationType.ITE) == true)
        mappedComponentResults.Add(new NavigationMetadata(Guid.NewGuid().ToString(), "ITE", programApplication.IteProgress ?? "ToDo", "Program Profile", 98, NavigationType.Other, false));
      if (programApplication.ProgramTypes?.Contains(Resources.Documents.ProgramApplications.ProgramCertificationType.SNE) == true)
        mappedComponentResults.Add(new NavigationMetadata(Guid.NewGuid().ToString(), "SNE", programApplication.SneProgress ?? "ToDo", "Program Profile", 99, NavigationType.Other, false));
    }

    return mappedComponentResults;
  }

  public async Task<IEnumerable<ComponentGroupWithComponents>> Handle(ComponentGroupWithComponentsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var query = new Resources.Documents.ProgramApplications.ComponentGroupWithComponentsQuery
    {
      ByProgramApplicationId = request.ByProgramApplicationId,
      ByComponentGroupId = request.ByComponentGroupId,
    };
    var result = await programApplicationRepository.QueryComponentGroupWithComponents(query, cancellationToken);
    return programApplicationMapper.MapComponentGroupsWithComponents(result);
  }

  public async Task<string> Handle(UpdateComponentGroupCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    return await programApplicationRepository.UpdateComponentGroup(programApplicationMapper.MapComponentGroupWithComponents(request.ComponentGroup), request.ProgramApplicationId, cancellationToken);
  }

  public async Task<ApplicationFileInfo> Handle(UploadProgramApplicationFileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var folder = $"ecer_programapplicationcomponent/{request.ComponentId}";
    var fileProperties = new FileProperties();
    List<FileData> files =
    [
      new FileData(
        new FileLocation(request.FileId, folder, Utilities.ObjectStorage.Providers.EcerWebApplicationType.PSP),
        fileProperties,
        request.FileName,
        request.ContentType,
        request.Content)
    ];

    var saveResponse = await mediator.Send(new SaveFileCommand(files), cancellationToken);
    var saveResult = saveResponse.Items.FirstOrDefault();
    if (saveResult == null || !saveResult.IsSuccessful)
    {
      throw new InvalidOperationException(saveResult?.Message ?? "File upload failed");
    }

    var humanSize = UtilityFunctions.HumanFileSize(request.FileSizeBytes);
    var createRequest = new CreateDocumentUrlRequest(
      request.FileId,
      request.FileName,
      humanSize,
      folder,
      request.ProgramApplicationId,
      request.ComponentGroupId,
      request.ComponentId,
      request.PostSecondaryInstituteId);
    var resourcesResult = await programApplicationRepository.CreateDocumentUrlAndShare(createRequest, cancellationToken);
    return programApplicationMapper.MapApplicationFileInfo(resourcesResult);
  }

  public async Task<ApplicationFileInfo> Handle(ShareExistingDocumentCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var resourcesResult = await programApplicationRepository.CreateShareOnly(
      request.DocumentId,
      request.ProgramApplicationId,
      request.ComponentGroupId,
      request.ComponentId,
      cancellationToken);
    return programApplicationMapper.MapApplicationFileInfo(resourcesResult);
  }

  public async Task<IEnumerable<ApplicationFileInfo>> Handle(ProgramApplicationFilesQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var resourcesResult = await programApplicationRepository.GetApplicationFiles(request.ProgramApplicationId, cancellationToken);
    return programApplicationMapper.MapApplicationFiles(resourcesResult);
  }

  public async Task<IEnumerable<ApplicationFileInfo>> Handle(ProgramApplicationDocumentUrlsQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var resourcesResult = await programApplicationRepository.GetApplicationDocumentUrls(request.ProgramApplicationId, cancellationToken);
    return programApplicationMapper.MapApplicationFiles(resourcesResult);
  }

  public async Task Handle(DeleteProgramApplicationFileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    var details = await programApplicationRepository.GetShareDocumentUrlDetails(request.ShareDocumentId, cancellationToken);
    if (details.RemainingShareCount <= 1)
    {
      var fileData = new FileData(
        new FileLocation(details.DocumentId, details.Folder, details.EcerWebApplicationType),
        new FileProperties(),
        string.Empty,
        string.Empty,
        Stream.Null);
      await mediator.Send(new DeleteFileCommand(fileData), cancellationToken);
    }

    await programApplicationRepository.DeleteShareDocumentUrlById(request.ShareDocumentId, cancellationToken);
  }

  public async Task<ProgramApplicationSubmissionResult> Handle(SubmitProgramApplicationCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var application = request.ProgramApplication;
    var applicationId = application.Id ?? throw new InvalidOperationException("Program application id is required for submission.");

    var componentGroups = await programApplicationRepository.QueryComponentGroups(
      new Resources.Documents.ProgramApplications.ComponentGroupQuery { ByProgramApplicationId = applicationId },
      cancellationToken);

    var resourcesCourses = await courseRepository.GetCourses(new GetCoursesRequest(applicationId, application.PostSecondaryInstituteId, FunctionType.ProgramApplication), cancellationToken);
    var contractCourses = coursesMapper.MapCourses(resourcesCourses);

    var areasOfInstruction = await metadataResourceRepository.QueryAreaOfInstructions(new AreaOfInstructionsQuery { ById = null }, cancellationToken);

    var validationContext = new ProgramApplicationValidationContext(
      ComponentGroupStatuses: componentGroups.Select(group => new ComponentGroupValidationStatus(group.Id, group.Name, group.Status)),
      Courses: contractCourses,
      ProgramApplication: application,
      AreasOfInstruction: areasOfInstruction.ToList().AsReadOnly(),
      DeclarationAccepted: request.Declaration);

    var validationResults = await validationEngine.Validate(validationContext);
    if (!validationResults.IsValid)
    {
      return new ProgramApplicationSubmissionResult
      {
        Error = ProgramApplicationSubmissionError.ValidationFailed,
        ValidationErrors = validationResults.ValidationErrors
      };
    }

    var resultApplicationId = await programApplicationRepository.Submit(
      application.Id,
      request.ProgramRepresentativeId,
      request.Declaration,
      cancellationToken);

    return new ProgramApplicationSubmissionResult { ProgramApplicationId = resultApplicationId };
  }
}
