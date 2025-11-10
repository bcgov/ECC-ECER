using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;

namespace ECER.Resources.Documents.PostSecondaryInstitutes;

public class PostSecondaryInstituteRepository(EcerContext context, IMapper mapper) : IPostSecondaryInstituteRepository
{
  public async Task<IEnumerable<PostSecondaryInstitute>> Query(PostSecondaryInstituteQuery query, CancellationToken ct)
  {
    await Task.CompletedTask;
    var institutes = context.ecer_PostSecondaryInstituteSet;

    if (query == null) return Array.Empty<PostSecondaryInstitute>();
    
    if (query.ById != null)
    {
      institutes = institutes.Where(r => r.Id.Equals(Guid.Parse(query.ById)));
    }

    if (query.ByBceidBusinessId != null)
    {
      institutes = institutes.Where(r => r.ecer_BusinessBCeID == query.ByBceidBusinessId);
    }

    if (query.ByProgramRepresentativeId != null)
    {
      if (Guid.TryParse(query.ByProgramRepresentativeId, out var repId))
      {
        var programReps = context.ecer_ECEProgramRepresentativeSet;
        programReps = programReps.Where(r => r.ecer_ECEProgramRepresentativeId == repId);
        var rep = programReps.SingleOrDefault();
        if (rep != null)
        {
          institutes = institutes.Where(r => r.ecer_PostSecondaryInstituteId == rep.ecer_PostSecondaryInstitute.Id);
        }
        else
        {
          return Array.Empty<PostSecondaryInstitute>();
        }
      }
    }

    var results = context.From(institutes).Execute();
    
    return mapper.Map<IEnumerable<PostSecondaryInstitute>>(results)!.ToList();
  }

  public async Task Save(PostSecondaryInstitute institute, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(institute);
    if(!Guid.TryParse(institute.Id, out var instituteId)) throw new InvalidOperationException($"Post Secondary Institute id {institute.Id} is not a valid GUID");

    var existingInstitute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(r => r.Id == instituteId);
    
    if (existingInstitute == null) throw new InvalidOperationException($"Post Secondary Institute with id {institute.Id} not found");

    var bceidBusinessId = existingInstitute.ecer_BusinessBCeID;
    
    context.Detach(existingInstitute);

    var updatedInstitute = mapper.Map<ecer_PostSecondaryInstitute>(institute);
    updatedInstitute.Id = instituteId;

    if (!String.IsNullOrEmpty(bceidBusinessId))
    {
      updatedInstitute.ecer_BusinessBCeID = bceidBusinessId;
    }
    
    context.Attach(updatedInstitute);
    context.UpdateObject(updatedInstitute);
    context.SaveChanges();
    await Task.CompletedTask;
  }
}
