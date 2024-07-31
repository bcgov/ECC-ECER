using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Resources.Documents.Applications.Children;

internal class WorkExperienceReferencesService : IApplicationChildService<ecer_WorkExperienceRef>
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public WorkExperienceReferencesService(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task Update(ecer_Application application, List<ecer_WorkExperienceRef> updatedEntities)
  {
    await Task.CompletedTask;
    var existingWorkExperiences = context.ecer_WorkExperienceRefSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();
    // Remove WorkExperienceReferences that they exist in the dataverse but not in the application
    foreach (var reference in existingWorkExperiences)
    {
      if (!updatedEntities.Any(t => t.Id == reference.Id))
      {
        context.DeleteObject(reference);
      }
    }
    // Update Existing WorkExperienceReferences
    foreach (var reference in updatedEntities.Where(d => d.ecer_WorkExperienceRefId != null))
    {
      var oldReference = existingWorkExperiences.SingleOrDefault(t => t.Id == reference.Id);
      if (oldReference != null)
      {
        context.Detach(oldReference);
      }
      context.Attach(reference);
      context.UpdateObject(reference);
    }
    // Add New WorkExperienceReferences that they exist in the application but not in the dataverse
    foreach (var reference in updatedEntities.Where(d => d.ecer_WorkExperienceRefId == null))
    {
      reference.ecer_WorkExperienceRefId = Guid.NewGuid();
      context.AddObject(reference);
      context.AddLink(application, ecer_Application.Fields.ecer_workexperienceref_Applicationid_ecer, reference);
    }
  }
}
