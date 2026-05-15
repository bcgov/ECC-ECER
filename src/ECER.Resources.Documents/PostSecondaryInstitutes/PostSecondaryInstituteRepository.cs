using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.DataverseSdk.Queries;
using Microsoft.Xrm.Sdk.Client;

namespace ECER.Resources.Documents.PostSecondaryInstitutes;

internal sealed class PostSecondaryInstituteRepository(EcerContext context, IPostSecondaryInstituteRepositoryMapper mapper) : IPostSecondaryInstituteRepository
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

    if (query.ByProgramRepresentativeId != null && Guid.TryParse(query.ByProgramRepresentativeId, out var repId))
    {
      var programReps = context.ecer_ECEProgramRepresentativeSet.Where(r => r.ecer_ECEProgramRepresentativeId == repId);
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

    var results = context.From(institutes)
      .Join()
      .Include(c => c.ecer_postsecondaryinstitutecampus_postsecondaryinstitute_ecer_postsecondaryinstitute)
      .Execute();

    return mapper.MapPostSecondaryInstitutes(results);
  }

  public async Task Save(PostSecondaryInstitute institute, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(institute);
    if (!Guid.TryParse(institute.Id, out var instituteId)) throw new InvalidOperationException($"Post Secondary Institute id {institute.Id} is not a valid GUID");

    var existingInstitute = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(r => r.Id == instituteId);

    if (existingInstitute == null) throw new InvalidOperationException($"Post Secondary Institute with id {institute.Id} not found");

    var bceidBusinessId = existingInstitute.ecer_BusinessBCeID;

    context.Detach(existingInstitute);

    var updatedInstitute = mapper.MapPostSecondaryInstitute(institute);
    updatedInstitute.Id = instituteId;

    if (!string.IsNullOrEmpty(bceidBusinessId))
    {
      updatedInstitute.ecer_BusinessBCeID = bceidBusinessId;
    }

    context.Attach(updatedInstitute);

    if (!string.IsNullOrWhiteSpace(institute.Province))
    {
      var existingProvince = context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_Name == institute.Province);
      if (existingProvince != null)
      {
        context.AddLink(updatedInstitute, ecer_PostSecondaryInstitute.Fields.ecer_postsecondaryinstitute_ProvinceId, existingProvince);
      }
    }

    context.UpdateObject(updatedInstitute);
    context.SaveChanges();
    await Task.CompletedTask;
  }

  public async Task<string> CreateCampus(string institutionId, Campus campus, CancellationToken ct, IEnumerable<string>? programIds = null)
  {
    ArgumentNullException.ThrowIfNull(campus);
    if (!Guid.TryParse(institutionId, out var institutionGuid))
      throw new InvalidOperationException($"Institution id {institutionId} is not a valid GUID");

    var institution = context.ecer_PostSecondaryInstituteSet.SingleOrDefault(r => r.Id == institutionGuid);
    if (institution == null) throw new InvalidOperationException($"Institution with id {institutionId} not found");

    var newCampus = mapper.MapCampus(campus);
    newCampus.ecer_PostSecondaryInstituteCampusId = Guid.NewGuid();
    newCampus.StatusCode = ecer_PostSecondaryInstituteCampus_StatusCode.Pending;
    newCampus.StateCode = ecer_postsecondaryinstitutecampus_statecode.Active;
    newCampus.ecer_postsecondaryinstitute = new Microsoft.Xrm.Sdk.EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, institutionGuid);

    context.AddObject(newCampus);

    if (!string.IsNullOrWhiteSpace(campus.Province))
    {
      var existingProvince = context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_Name == campus.Province);
      if (existingProvince != null)
        context.AddLink(newCampus, ecer_PostSecondaryInstituteCampus.Fields.ecer_postsecondaryinstitutecampus_province_ecer_province, existingProvince);
    }

    context.SaveChanges();

    foreach (var programId in programIds ?? [])
    {
      if (!Guid.TryParse(programId, out var programGuid)) continue;
      var programCampus = new ecer_ProgramCampus
      {
        ecer_ProgramCampusId = Guid.NewGuid(),
        ecer_CampusId = new Microsoft.Xrm.Sdk.EntityReference(ecer_PostSecondaryInstituteCampus.EntityLogicalName, newCampus.ecer_PostSecondaryInstituteCampusId!.Value),
        ecer_ProgramProfileId = new Microsoft.Xrm.Sdk.EntityReference(ecer_Program.EntityLogicalName, programGuid),
        ecer_EducationalInstitutionId = new Microsoft.Xrm.Sdk.EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, institutionGuid),
        StatusCode = ecer_ProgramCampus_StatusCode.Active,
        StateCode = ecer_programcampus_statecode.Active,
      };
      context.AddObject(programCampus);
    }

    context.SaveChanges();
    await Task.CompletedTask;
    return newCampus.ecer_PostSecondaryInstituteCampusId!.Value.ToString();
  }

  public async Task UpdateCampus(Campus campus, CancellationToken ct)
  {
    ArgumentNullException.ThrowIfNull(campus);
    if (!Guid.TryParse(campus.Id, out var campusGuid))
      throw new InvalidOperationException($"Campus id {campus.Id} is not a valid GUID");

    var existingCampus = context.ecer_PostSecondaryInstituteCampusSet.SingleOrDefault(r => r.Id == campusGuid);
    if (existingCampus == null) throw new InvalidOperationException($"Campus with id {campus.Id} not found");

    context.Detach(existingCampus);

    var updatedCampus = mapper.MapCampus(campus);
    updatedCampus.ecer_PostSecondaryInstituteCampusId = campusGuid;

    context.Attach(updatedCampus);

    if (!string.IsNullOrWhiteSpace(campus.Province))
    {
      var existingProvince = context.ecer_ProvinceSet.SingleOrDefault(p => p.ecer_Name == campus.Province);
      if (existingProvince != null)
        context.AddLink(updatedCampus, ecer_PostSecondaryInstituteCampus.Fields.ecer_postsecondaryinstitutecampus_province_ecer_province, existingProvince);
    }

    context.UpdateObject(updatedCampus);
    context.SaveChanges();
    await Task.CompletedTask;
  }
}
