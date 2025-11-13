using AutoMapper;
using ECER.Engines.Transformation;
using ECER.Engines.Transformation.PortalInvitations;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Managers.Registry.Contract.PspUsers;
using ECER.Managers.Registry.Contract.Registrants;
using ECER.Managers.Registry.UserRegistrationIdentityService;
using ECER.Resources.Accounts.PspReps;
using ECER.Resources.Documents.PortalInvitations;
using ECER.Resources.Documents.PostSecondaryInstitutes;
using ECER.Utilities.DataverseSdk.Model;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Managers.Registry;

public class PspUserHandlers(
    IPortalInvitationTransformationEngine transformationEngine,
    IPortalInvitationRepository portalInvitationRepository,
    IPspRepRepository pspRepRepository, 
    IPostSecondaryInstituteRepository postSecondaryInstituteRepository, 
    IMapper mapper, 
    EcerContext ecerContext,
    IServiceProvider serviceProvider)
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
      ByProgramRepresentativeId = request.ProgramRepresentativeId
    }, cancellationToken)).SingleOrDefault();
    
    if (postSecondaryInstitution == null) throw new InvalidOperationException($"Post secondary institute of associated program representative id {request.ProgramRepresentativeId} not found");

    ecerContext.BeginTransaction();
    var bceidBusinessId = postSecondaryInstitution.BceidBusinessId; 
    if (string.IsNullOrEmpty(bceidBusinessId))
    {
      // Save the bceid business id to the post secondary institute
      postSecondaryInstitution.BceidBusinessId = request.BceidBusinessId;
      await postSecondaryInstituteRepository.Save(postSecondaryInstitution, cancellationToken);
    }
    
    // Check that the bceid business id matches the request    
    else if (request.BceidBusinessId != bceidBusinessId)
    {
      return RegisterPspUserResult.Failure(RegisterPspUserError.BceidBusinessIdDoesNotMatch);
    }
    
    // We've saved the business id, proceed with registration of the user (attach the identity)
    BceidRegistrationIdentityService resolver = serviceProvider.GetRequiredService<BceidRegistrationIdentityService>();
    await resolver.Resolve(new RegisterNewPspUserCommand(request.ProgramRepresentativeId, request.Profile, request.Identity),
      cancellationToken);
      
    // Get Portal invitation from token
    var transformationResponse = await transformationEngine.Transform(new DecryptInviteTokenRequest(request.Token))! as DecryptInviteTokenResponse ?? throw new InvalidCastException("Invalid response type");
    if (transformationResponse.PortalInvitation == Guid.Empty) return RegisterPspUserResult.Failure(RegisterPspUserError.PortalInvitationTokenInvalid);
      
    // Check that the portal invitation is in 'Sent' status
    var portalInvitation = await portalInvitationRepository.Query(new PortalInvitationQuery(transformationResponse.PortalInvitation), cancellationToken);
    if (portalInvitation.StatusCode != PortalInvitationStatusCode.Sent) return RegisterPspUserResult.Failure(RegisterPspUserError.PortalInvitationWrongStatus);
      
    // Complete the portal invitation
    await portalInvitationRepository.Complete(new CompletePortalInvitationCommand(transformationResponse.PortalInvitation), cancellationToken);
    
    ecerContext.CommitTransaction();
    return RegisterPspUserResult.Success();
  }
}

