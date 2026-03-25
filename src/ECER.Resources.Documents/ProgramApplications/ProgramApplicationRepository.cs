using AutoMapper;
using ECER.Resources.Documents.Shared;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.ProgramApplications;

internal sealed class ProgramApplicationRepository : IProgramApplicationRepository
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public ProgramApplicationRepository(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task<string> Create(ProgramApplication programApplication, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    if (string.IsNullOrWhiteSpace(programApplication.PostSecondaryInstituteId))
    {
      throw new InvalidOperationException("Post secondary institute id is required");
    }

    var instituteId = Guid.Parse(programApplication.PostSecondaryInstituteId);
    var institute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(i => i.ecer_PostSecondaryInstituteId == instituteId);
    if (institute == null)
    {
      throw new InvalidOperationException($"Post secondary institute '{programApplication.PostSecondaryInstituteId}' not found");
    }

    if (programApplication.ProgramTypes == null)
    {
      throw new InvalidOperationException("Program types are required");
    }

    if (!programApplication.DeliveryType.HasValue)
    {
      throw new InvalidOperationException("Delivery type is required");
    }

    var entity = mapper.Map<ecer_PostSecondaryInstituteProgramApplicaiton>(programApplication)!;
    entity.ecer_PostSecondaryInstituteProgramApplicaitonId = Guid.NewGuid();
    entity.StatusCode = ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode.Draft;
    entity.StateCode = ecer_postsecondaryinstituteprogramapplicaiton_statecode.Active;
    entity.ecer_ProgramType = programApplication.ProgramTypes.Select(t => Enum.Parse<ecer_PSIProgramType>(t.ToString()));
    entity.ecer_DeliveryType = Enum.Parse<ecer_PSIDeliveryType>(programApplication.DeliveryType.Value.ToString());
    context.AddObject(entity);

    var programProfileId = Guid.Empty;
    if (programApplication.ProgramProfileId != null)
    {
      programProfileId = Guid.Parse(programApplication.ProgramProfileId);
      var programProfile = context.ecer_ProgramSet.SingleOrDefault(program => program.Id == programProfileId);
      
      context.UpdateObject(entity);
      context.AddLink(entity, ecer_Program.Fields.ecer_postsecondaryinstituteprogramapplicaiton_FromProgramProfileId_ecer_program, programProfile!);
    }

    if (programApplication.ProgramCampuses != null && programApplication.ProgramCampuses.Any())
    {
      CreateProgramCampus(programApplication.ProgramCampuses, programProfileId, instituteId, entity);
    }
    if (string.IsNullOrWhiteSpace(entity.ecer_Name))
    {
      entity.ecer_Name = "Draft Program Application";
    }

    context.UpdateObject(entity);
    context.AddLink(entity, ecer_PostSecondaryInstituteProgramApplicaiton.Fields.ecer_postsecondaryinstituteprogramapplicaiton_, institute);

    context.SaveChanges();
    return entity.ecer_PostSecondaryInstituteProgramApplicaitonId!.Value.ToString();
  }

  public void CreateProgramCampus(IEnumerable<ProgramCampus> programCampuses, Guid programProfileId, Guid instituteId, ecer_PostSecondaryInstituteProgramApplicaiton entity)
  {
    foreach (var campus in programCampuses)
    {
      if (!Guid.TryParse(campus.CampusId, out Guid campusGuid))
      {
        throw new InvalidOperationException("Campus id cannot be null");
      }
      var psiCampus =
        context.ecer_PostSecondaryInstituteCampusSet.SingleOrDefault(c => c.Id == campusGuid);
      if (psiCampus != null && campus.Id == null)
      {
        var programCampus = new ecer_ProgramCampus
        {
          Id = Guid.NewGuid(),
          ecer_EducationalInstitutionId = new EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, instituteId),
          ecer_ProgramApplicationId =
            new EntityReference(ecer_PostSecondaryInstituteProgramApplicaiton.EntityLogicalName, entity.Id),
          ecer_CampusId = new EntityReference(ecer_PostSecondaryInstituteCampus.EntityLogicalName, psiCampus.Id)
        };
        if (programProfileId != Guid.Empty)
        {
          programCampus.ecer_ProgramProfileId =
            new EntityReference(ecer_Program.EntityLogicalName, programProfileId);
        }
        context.AddObject(programCampus);
      }
    }
  }

  public async Task<string> UpdateProgramApplication(ProgramApplication application, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var existingApplication = context.ecer_PostSecondaryInstituteProgramApplicaitonSet.SingleOrDefault(p => p.ecer_PostSecondaryInstituteProgramApplicaitonId == Guid.Parse(application.Id!));
    if (existingApplication == null) throw new InvalidOperationException($"ecer_Program '{application.Id}' not found");

    if (application.Status == ApplicationStatus.Withdrawn)
    {
      existingApplication.StatusCode = ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode.Withdrawn;
      existingApplication.StateCode = ecer_postsecondaryinstituteprogramapplicaiton_statecode.Inactive;
      context.UpdateObject(existingApplication);
    }
    else
    {
      var entity = mapper.Map<ecer_PostSecondaryInstituteProgramApplicaiton>(application)!;
      var instituteId = Guid.Parse(application.PostSecondaryInstituteId);
      var institute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(i => i.ecer_PostSecondaryInstituteId == instituteId);
      if (institute == null)
      {
        throw new InvalidOperationException($"Post secondary institute '{application.PostSecondaryInstituteId}' not found");
      }

      if (existingApplication.ecer_ApplicationType != ecer_PSIApplicationType.NewCampusatRecognizedPrivateInstitution)
      {
        if (application.ProgramCampuses != null && application.ProgramCampuses.Any())
        {
          var listOfExistingProgramCampuses = context.ecer_ProgramCampusSet.Where(c =>
            c.ecer_EducationalInstitutionId.Id == instituteId && c.ecer_ProgramApplicationId.Id == entity.Id).ToList();
          var existingIncomingProgramCampus = application.ProgramCampuses.Where(c => c.Id != null)
            .Select(c => Guid.Parse(c.Id!)).ToList();

          var campusesToDelete = listOfExistingProgramCampuses
            .Where(c => !existingIncomingProgramCampus.Contains(c.Id))
            .ToList();

          foreach (var campus in campusesToDelete)
          {
            context.DeleteObject(campus);
          }

          foreach (var campus in application.ProgramCampuses)
          {
            if (!Guid.TryParse(campus.CampusId, out Guid campusGuid))
            {
              throw new InvalidOperationException("Campus id cannot be null");
            }

            var psiCampus =
              context.ecer_PostSecondaryInstituteCampusSet.SingleOrDefault(c => c.Id == campusGuid);
            if (psiCampus != null && campus.Id == null)
            {
              var programCampus = new ecer_ProgramCampus
              {
                Id = Guid.NewGuid(),
                ecer_EducationalInstitutionId =
                  new EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, instituteId),
                ecer_ProgramApplicationId =
                  new EntityReference(ecer_PostSecondaryInstituteProgramApplicaiton.EntityLogicalName, entity.Id),
                ecer_CampusId = new EntityReference(ecer_PostSecondaryInstituteCampus.EntityLogicalName, psiCampus.Id)
              };
              context.AddObject(programCampus);
            }
          }
        }
        else
        {
          var listOfExistingProgramCampuses = context.ecer_ProgramCampusSet.Where(c =>
            c.ecer_EducationalInstitutionId.Id == instituteId && c.ecer_ProgramApplicationId.Id == entity.Id).ToList();
          foreach (var campus in listOfExistingProgramCampuses)
          {
            context.DeleteObject(campus);
          }
        }
      }

      context.Detach(existingApplication);
      context.Attach(entity);
      context.UpdateObject(entity);
      context.AddLink(entity, ecer_PostSecondaryInstituteProgramApplicaiton.Fields.ecer_postsecondaryinstituteprogramapplicaiton_, institute);
      if (application.ProgramRepresentativeId != null)
      {
        var user = context.ecer_ECEProgramRepresentativeSet
          .SingleOrDefault(r => r.Id == Guid.Parse(application.ProgramRepresentativeId));
        context.AddLink(entity, ecer_PostSecondaryInstituteProgramApplicaiton.Fields.ecer_postsecondaryinstituteprogramapplicaiton_PSIProgramRepresentative_ecer_eceprogramrepresentativ, user!);
      }

      if (ValidateInstituteInfo(entity, institute, application))
      {
        entity.ecer_InstitutionProgramInformationEntryProgress = ecer_PSPComponentProgress.Completed;
      }
      else
      {
        entity.ecer_InstitutionProgramInformationEntryProgress = ecer_PSPComponentProgress.InProgress;
      }
      context.UpdateObject(entity);
    }

    context.SaveChanges();
    return application.Id!;
  }

  private static bool ValidateInstituteInfo(ecer_PostSecondaryInstituteProgramApplicaiton application, ecer_PostSecondaryInstitute institute, ProgramApplication incomingApplication)
  {
    return application.ecer_postsecondaryinstituteprogramapplicaiton_PSIProgramRepresentative_ecer_eceprogramrepresentativ != null
            && application.ecer_postsecondaryinstituteprogramapplicaiton_PSIProgramRepresentative_ecer_eceprogramrepresentativ.Id != Guid.Empty
            && (incomingApplication.DeliveryType == DeliveryType.Hybrid || incomingApplication.DeliveryType == DeliveryType.Online)
              && application.ecer_Onlinemethodsofinstruction.Any() && application.ecer_Deliverymethodforpracticuminstructor.Any()
            && ValidateCampus(application, institute);
  }

  private static bool ValidateCampus(ecer_PostSecondaryInstituteProgramApplicaiton application, ecer_PostSecondaryInstitute institute)
  {
    if (institute.ecer_PSIInstitutionType == ecer_psiinstitutiontype.Private)
    {
      return application.ecer_ProgramApplicationId_ecer_postsecondaryinstituteprogramapplicaiton != null
             && application.ecer_ProgramApplicationId_ecer_postsecondaryinstituteprogramapplicaiton.Any()
             && application.ecer_ProgramApplicationId_ecer_postsecondaryinstituteprogramapplicaiton.Count() != 1;
    }
    return true;
  }

  public async Task<ProgramApplicationQueryResults> Query(ProgramApplicationQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var applications = context.ecer_PostSecondaryInstituteProgramApplicaitonSet.AsQueryable();

    //Filter by Id
    if (query.ById != null) applications = applications.Where(p => p.ecer_PostSecondaryInstituteProgramApplicaitonId == Guid.Parse(query.ById));

    //By status
    if (query.ByStatus != null && query.ByStatus.Any())
    {
      var statuses = mapper.Map<IEnumerable<ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode>>(query.ByStatus)!.ToList();
      applications = applications.WhereIn(p => p.StatusCode!.Value, statuses);
    }

    //By post secondary
    if (query.ByPostSecondaryInstituteId != null)
    {
      var instituteId = Guid.Parse(query.ByPostSecondaryInstituteId);
      applications = applications.Where(p => p.ecer_PostSecondaryInstitute.Id == instituteId);
    }

    //By campus
    if (query.ByCampusId != null)
    {
      var campusId = Guid.Parse(query.ByCampusId);
      var programApplicationIdsForCampus = context.ecer_ProgramCampusSet
        .Where(c => c.ecer_CampusId.Id == campusId && c.ecer_ProgramApplicationId != null)
        .Select(c => c.ecer_ProgramApplicationId.Id)
        .ToList();
      if (programApplicationIdsForCampus.Count == 0) return new ProgramApplicationQueryResults([], 0);
      applications = applications.WhereIn(p => p.ecer_PostSecondaryInstituteProgramApplicaitonId!.Value, programApplicationIdsForCampus);
    }

    int paginatedTotalProgramCount = 0;
    if (query.PageNumber > 0)
    {
      paginatedTotalProgramCount = context.From(applications).Aggregate().Count();
      applications = applications.OrderByDescending(item => item.CreatedOn).Skip(query.PageNumber).Take(query.PageSize);
    }
    else
    {
      applications = applications.OrderByDescending(item => item.CreatedOn);
    }

    var results = context.From(applications)
      .Join()
      .Include(p => p.ecer_ProgramApplicationId_ecer_postsecondaryinstituteprogramapplicaiton)
      .Execute()
      .ToList();

    return new ProgramApplicationQueryResults(mapper.Map<IEnumerable<ProgramApplication>>(results)!, query.PageNumber > 0 ? paginatedTotalProgramCount : results.Count);
  }
  public async Task<IEnumerable<NavigationMetadata>> QueryComponentGroups(ComponentGroupQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var categoryGroups = context.ecer_ProgramApplicationComponentGroupSet.AsQueryable();

    //Filter by Id
    if (query.ByProgramApplicationId != null) categoryGroups = categoryGroups.Where(p => p.ecer_ProgramApplication.Id == Guid.Parse(query.ByProgramApplicationId));
    var results = context.From(categoryGroups)
      .Execute();

    return mapper.Map<IEnumerable<NavigationMetadata>>(results)!.ToList();
  }

  public async Task<IEnumerable<ComponentGroupWithComponents>> QueryComponentGroupWithComponents(ComponentGroupWithComponentsQuery query, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var categoryGroups = context.ecer_ProgramApplicationComponentGroupSet.AsQueryable();

    if (query.ByComponentGroupId != null) categoryGroups = categoryGroups.Where(p => p.ecer_ProgramApplicationComponentGroupId == Guid.Parse(query.ByComponentGroupId));

    if (query.ByProgramApplicationId != null) categoryGroups = categoryGroups.Where(p => p.ecer_ProgramApplication.Id == Guid.Parse(query.ByProgramApplicationId));

    var results = context.From(categoryGroups)
      .Join()
      .Include(e => e.ecer_programapplicationcomponentgroup_ComponentGroup)
      .Include(e => e.ecer_programapplicationcomponent_ComponentGroup)
      .IncludeNested(e => e.ecer_documenturl_ProgramApplicationComponentId)
      .Execute();
    return mapper.Map<IEnumerable<ComponentGroupWithComponents>>(results)!.ToList();
  }

  public async Task<string> UpdateComponentGroup(ComponentGroupWithComponents componentGroupToUpdate, string applicationId, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;

    var groupId = Guid.Parse(componentGroupToUpdate.Id);
    var appId = Guid.Parse(applicationId);

    var componentGroup = context.ecer_ProgramApplicationComponentGroupSet
      .SingleOrDefault(g => g.ecer_ProgramApplicationComponentGroupId == groupId && g.ecer_ProgramApplication.Id == appId);
    if (componentGroup == null) throw new InvalidOperationException($"Component group '{componentGroupToUpdate.Id}' not found");

    var existingComponentsQuery = context.ecer_ProgramApplicationComponentSet
      .Where(c => c.ecer_ComponentGroup.Id == groupId);
    var existingComponents = context.From(existingComponentsQuery)
      .Execute()
      .Where(c => c.ecer_ProgramApplicationComponentId.HasValue)
      .ToDictionary(c => c.ecer_ProgramApplicationComponentId!.Value);

    foreach (var component in componentGroupToUpdate.Components)
    {
      if (!Guid.TryParse(component.Id, out var componentId)) continue;
      if (!existingComponents.TryGetValue(componentId, out var existingComponent)) continue;

      var ecerComponent = mapper.Map<ecer_ProgramApplicationComponent>(component)!;
      context.Detach(existingComponent);
      context.Attach(ecerComponent);
      context.UpdateObject(ecerComponent);
      existingComponents[componentId] = ecerComponent;
    }

    var allAnswers = existingComponents.Values.Select(c => c.ecer_Componentanswer).ToList();
    ecer_PSPComponentProgress progress;
    if (allAnswers.All(string.IsNullOrWhiteSpace))
    {
      progress = ecer_PSPComponentProgress.ToDo;
    }
    else if (allAnswers.All(a => !string.IsNullOrWhiteSpace(a)))
    {
      progress = ecer_PSPComponentProgress.Completed;
    }
    else
    {
      progress = ecer_PSPComponentProgress.InProgress;
    }

    componentGroup.ecer_EntryProgress = progress;
    context.UpdateObject(componentGroup);

    context.SaveChanges();
    return componentGroupToUpdate.Id;
  }

  public async Task<string> Submit(string applicationId, string programRepresentativeId, bool declaration, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
    var appGuid = Guid.Parse(applicationId);
    var existing = context.ecer_PostSecondaryInstituteProgramApplicaitonSet
      .SingleOrDefault(p => p.ecer_PostSecondaryInstituteProgramApplicaitonId == appGuid);
    if (existing == null) throw new InvalidOperationException($"Program application '{applicationId}' not found");

    if (existing.StatusCode == ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode.Draft)
    {
      existing.StatusCode = ecer_PostSecondaryInstituteProgramApplicaiton_StatusCode.Submitted;
      existing.ecer_DateofApplicationShort = DateTime.UtcNow;
      existing.ecer_AgreeNotifyofChanges = declaration ? ecer_YesNoNull.Yes : ecer_YesNoNull.No; 
      existing.ecer_SubmittedByProgramRepresentativeId = new EntityReference(ecer_ECEProgramRepresentative.EntityLogicalName, Guid.Parse(programRepresentativeId));
    }
    else
    {
      existing.ecer_statusreasondetail = ecer_Statusreasondetail.RFAIreceived;
      existing.ecer_ModifiedbyProgramRepresentative = new EntityReference(ecer_ECEProgramRepresentative.EntityLogicalName, Guid.Parse(programRepresentativeId));
      existing.ecer_ProgramRepModifiedDate = DateTime.UtcNow;
    }


    context.UpdateObject(existing);
    context.SaveChanges();
    return applicationId;
  }
}
