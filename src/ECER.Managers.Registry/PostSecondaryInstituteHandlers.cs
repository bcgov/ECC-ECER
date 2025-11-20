using AutoMapper;
using ECER.Managers.Registry.Contract.PostSecondaryInstitutes;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using MediatR;


namespace ECER.Managers.Registry;

public class PostSecondaryInstituteHandlers(
    IPostSecondaryInstituteRepository postSecondaryInstituteRepository, 
    IMapper mapper)
  : IRequestHandler<SearchPostSecondaryInstitutionQuery, PostSecondaryInstitutionsQueryResults>
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
}

