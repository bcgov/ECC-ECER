using AutoMapper;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Resources.Accounts.PspReps;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using MediatR;

namespace ECER.Managers.Registry;

public class PspUserHandlers(IPspRepRepository pspRepRepository, IPostSecondaryInstituteRepository postSecondaryInstituteRepository, IMapper mapper, IServiceProvider serviceProvider)
  : IRequestHandler<SearchPspRepQuery, PspRepQueryResults>,
    IRequestHandler<UpdatePspRepProfileCommand, string>,
    IRequestHandler<RegisterPspUserCommand, RegisterPspUserResult>
{
  /// <summary>
  /// Handles search psp program rep use case
  /// </summary>
  /// <returns>Query results</returns>
  public async Task<PspRepQueryResults> Handle(SearchPspRepQuery request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var pspUsers = await pspRepRepository.Query(new PspRepQuery
    {
      ByIdentity = request.ByUserIdentity
    }, cancellationToken);

    return new PspRepQueryResults(mapper.Map<IEnumerable<Contract.PspUsers.PspUser>>(pspUsers)!);
  }
  
  /// <summary>
  ///  Handles updating a psp user profile use case
  /// </summary>
  /// <param name="request"></param>
  /// <param name="cancellationToken"></param>
  /// <returns>the user id of the updated user</returns>
  public async Task<string> Handle(UpdatePspRepProfileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    
    var pspUser = (await pspRepRepository.Query(new PspRepQuery
    {
      ById = request.User.Id
    }, cancellationToken)).SingleOrDefault();
    
    if (pspUser == null) throw new InvalidOperationException($"Psp user with id {request.User.Id} not found");
    var profile = mapper.Map<Resources.Accounts.PspReps.PspUserProfile>(request.User.Profile);
    await pspRepRepository.Save(new Resources.Accounts.PspReps.PspUser { Id = request.User.Id, Profile = profile }, cancellationToken); 
    
    return request.User.Id;
  }
  
  /// <summary>
  /// Handles registering a psp user use case
  /// </summary>
  /// <returns>the user id of the newly created user</returns>
  public async Task<RegisterPspUserResult> Handle(RegisterPspUserCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);
    
    var postSecondaryInstitution = (await postSecondaryInstituteRepository.Query(new PostSecondaryInstituteQuery
    {
      ByProgramRepresentativeId = request.Profile.ProgramRepresentativeId
    }, cancellationToken)).SingleOrDefault();
    
    if (postSecondaryInstitution == null) throw new InvalidOperationException($"Post secondary institute of associated program representative id {request.Profile.ProgramRepresentativeId} not found");

    var bceidBusinessId = postSecondaryInstitution.BceidBusinessId;
    if (string.IsNullOrEmpty(bceidBusinessId))
    {
      // Save the bceid business id to the post secondary institute
      postSecondaryInstitution.BceidBusinessId = request.Profile.BceidBusinessId;
      await postSecondaryInstituteRepository.Save(postSecondaryInstitution, cancellationToken);
      
      return RegisterPspUserResult.Success();
    }
    
    // Check that the bceid business id matches the request
    if (request.Profile.BceidBusinessId != bceidBusinessId)
    {
      return RegisterPspUserResult.Failure(RegisterPspUserError.BceidBusinessIdDoesNotMatch);
    }
    
    // Business id matches, proceed with registration
    return RegisterPspUserResult.Success();
  }
}

