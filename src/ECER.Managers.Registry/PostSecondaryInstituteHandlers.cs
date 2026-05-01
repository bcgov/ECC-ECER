using ECER.Managers.Registry.Contract.PostSecondaryInstitutes;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using MediatR;

namespace ECER.Managers.Registry;

public class PostSecondaryInstituteHandlers(
    IPostSecondaryInstituteRepository postSecondaryInstituteRepository,
    IPostSecondaryInstituteMapper postSecondaryInstituteMapper)
  : IRequestHandler<SearchPostSecondaryInstitutionQuery, PostSecondaryInstitutionsQueryResults>,
    IRequestHandler<UpdatePostSecondaryInstitutionCommand, string>,
    IRequestHandler<CreateCampusCommand, string>,
    IRequestHandler<UpdateCampusCommand, string>
{
  /// <summary>
  /// Handles search post secondary institution by program representative use case
  /// </summary>
  /// <returns>Query results</returns>
  public async Task<PostSecondaryInstitutionsQueryResults> Handle(SearchPostSecondaryInstitutionQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var institute = await postSecondaryInstituteRepository.Query(new PostSecondaryInstituteQuery
    {
      ByProgramRepresentativeId = request.ByProgramRepresentativeId
    }, cancellationToken);

    return new PostSecondaryInstitutionsQueryResults(postSecondaryInstituteMapper.MapPostSecondaryInstitutions(institute));
  }

  /// <summary>
  /// Handles update an existing post secondary institution use case
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns></returns>
  public async Task<string> Handle(UpdatePostSecondaryInstitutionCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var institution = (await postSecondaryInstituteRepository.Query(new PostSecondaryInstituteQuery
    {
      ById = request.Institute.Id
    }, cancellationToken)).SingleOrDefault();

    if (institution == null) throw new InvalidOperationException($"Institute {request.Institute.Id} wasn't found");
    var institute = postSecondaryInstituteMapper.MapPostSecondaryInstitute(request.Institute);
    await postSecondaryInstituteRepository.Save(new Resources.Documents.PostSecondaryInstitutes.PostSecondaryInstitute {
      Id = request.Institute.Id,
      InstitutionType = institute.InstitutionType,
      PrivateAuspiceType = institute.PrivateAuspiceType,
      PtiruInstitutionId = institute.PtiruInstitutionId,
      BceidBusinessId = institute.BceidBusinessId,
      BceidBusinessName = institute.BceidBusinessName,
      City = institute.City,
      PostalCode = institute.PostalCode,
      Name = institute.Name,
      Street1 = institute.Street1,
      Street2 = institute.Street2,
      Street3 = institute.Street3,
      Province = institute.Province,
      WebsiteUrl = institute.WebsiteUrl
    }, cancellationToken);

    return request.Institute.Id;
  }

  /// <summary>
  /// Handles creating a new campus or satellite location for the institution of the given program representative
  /// </summary>
  public async Task<string> Handle(CreateCampusCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var institution = (await postSecondaryInstituteRepository.Query(new PostSecondaryInstituteQuery
    {
      ByProgramRepresentativeId = request.ProgramRepresentativeId
    }, cancellationToken)).SingleOrDefault();

    if (institution == null) throw new InvalidOperationException($"No institution found for program representative {request.ProgramRepresentativeId}");

    var campus = postSecondaryInstituteMapper.MapCampus(request.Campus) with { IsSatelliteOrTemporaryLocation = request.IsSatelliteOrTemporaryLocation };
    return await postSecondaryInstituteRepository.CreateCampus(institution.Id, campus, cancellationToken, request.ProgramIds);
  }

  /// <summary>
  /// Handles updating an existing campus - IsSatelliteOrTemporaryLocation is not changed
  /// </summary>
  public async Task<string> Handle(UpdateCampusCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var institution = (await postSecondaryInstituteRepository.Query(new PostSecondaryInstituteQuery
    {
      ByProgramRepresentativeId = request.ProgramRepresentativeId
    }, cancellationToken)).SingleOrDefault();

    if (institution == null) throw new InvalidOperationException($"No institution found for program representative {request.ProgramRepresentativeId}");

    var existingCampus = institution.Campuses?.SingleOrDefault(c => c.Id == request.Campus.Id);
    if (existingCampus == null) throw new InvalidOperationException($"Campus {request.Campus.Id} does not belong to the user's institution");

    var campusToUpdate = request.Campus with { IsSatelliteOrTemporaryLocation = existingCampus.IsSatelliteOrTemporaryLocation };
    var campus = postSecondaryInstituteMapper.MapCampus(campusToUpdate);
    await postSecondaryInstituteRepository.UpdateCampus(campus, cancellationToken);
    return request.Campus.Id;
  }
}
