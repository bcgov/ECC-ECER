using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Resources.Documents.Applications.ChildrenServices;

internal class ProfessionalDevelopmentsService : IApplicationChildService<ecer_ProfessionalDevelopment>
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public ProfessionalDevelopmentsService(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task Update(ecer_Application application, List<ecer_ProfessionalDevelopment> updatedEntities)
  {
    await Task.CompletedTask;
    var existingProfessionalDevelopments = context.ecer_ProfessionalDevelopmentSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    foreach (var professionalDevelopment in existingProfessionalDevelopments)
    {
      if (!updatedEntities.Any(t => t.Id == professionalDevelopment.Id))
      {
        context.DeleteObject(professionalDevelopment);
      }
    }

    foreach (var professionalDevelopment in updatedEntities.Where(d => d.ecer_ProfessionalDevelopmentId != null))
    {
      var oldProfessionalDevelopment = existingProfessionalDevelopments.SingleOrDefault(t => t.Id == professionalDevelopment.Id);
      if (oldProfessionalDevelopment != null)
      {
        context.Detach(oldProfessionalDevelopment);
      }
      context.Attach(professionalDevelopment);
      context.UpdateObject(professionalDevelopment);
    }

    foreach (var professionalDevelopment in updatedEntities.Where(d => d.ecer_ProfessionalDevelopmentId == null))
    {
      professionalDevelopment.ecer_ProfessionalDevelopmentId = Guid.NewGuid();
      context.AddObject(professionalDevelopment);
      context.AddLink(application, ecer_Application.Fields.ecer_ecer_professionaldevelopment_Applicationi, professionalDevelopment);
    }
  }
}
