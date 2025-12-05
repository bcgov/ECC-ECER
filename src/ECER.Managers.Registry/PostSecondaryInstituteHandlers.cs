using AutoMapper;
using ECER.Managers.Registry.Contract.PostSecondaryInstitutes;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using MediatR;

namespace ECER.Managers.Registry;

public class PostSecondaryInstituteHandlers(
    IPostSecondaryInstituteRepository postSecondaryInstituteRepository, 
    IMapper mapper)
  : IRequestHandler<SearchPostSecondaryInstitutionQuery, PostSecondaryInstitutionsQueryResults>,
    IRequestHandler<UpdatePostSecondaryInstitutionCommand, string>
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

    return new PostSecondaryInstitutionsQueryResults(mapper.Map<IEnumerable<Contract.PostSecondaryInstitutes.PostSecondaryInstitute>>(institute)!);
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
    var institute = mapper.Map<Resources.Documents.PostSecondaryInstitutes.PostSecondaryInstitute>(request.Institute)!;
    await postSecondaryInstituteRepository.Save(new Resources.Documents.PostSecondaryInstitutes.PostSecondaryInstitute {
      Id = request.Institute.Id,
      Auspice = institute.Auspice,
      BceidBusinessId = institute.BceidBusinessId,
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
}

