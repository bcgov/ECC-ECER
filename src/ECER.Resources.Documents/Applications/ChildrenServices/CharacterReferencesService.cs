using AutoMapper;
using ECER.Resources.Documents.Applications.Children;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Resources.Documents.Applications.ChildrenServices;

internal class CharacterReferencesService : IApplicationChildService<ecer_CharacterReference>
{
  private readonly EcerContext context;
  private readonly IMapper mapper;

  public CharacterReferencesService(EcerContext context, IMapper mapper)
  {
    this.context = context;
    this.mapper = mapper;
  }

  public async Task Update(ecer_Application application, List<ecer_CharacterReference> updatedEntities)
  {
    await Task.CompletedTask;
    var existingCharacterReferences = context.ecer_CharacterReferenceSet.Where(t => t.ecer_Applicationid.Id == application.Id).ToList();

    foreach (var reference in existingCharacterReferences)
    {
      if (!updatedEntities.Any(t => t.Id == reference.Id))
      {
        context.DeleteObject(reference);
      }
    }

    foreach (var reference in updatedEntities.Where(d => d.ecer_CharacterReferenceId != null))
    {
      var oldReference = existingCharacterReferences.SingleOrDefault(t => t.Id == reference.Id);
      if (oldReference != null)
      {
        context.Detach(oldReference);
      }
      context.Attach(reference);
      context.UpdateObject(reference);
    }

    foreach (var reference in updatedEntities.Where(d => d.ecer_CharacterReferenceId == null))
    {
      reference.ecer_CharacterReferenceId = Guid.NewGuid();
      context.AddObject(reference);
      context.AddLink(application, ecer_Application.Fields.ecer_characterreference_Applicationid, reference);
    }
  }
}
