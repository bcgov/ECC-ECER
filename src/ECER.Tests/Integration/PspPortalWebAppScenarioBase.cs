using ECER.Utilities.DataverseSdk.Model;
using ECER.Utilities.Security;
using ECER.Clients.PSPPortal.Server.Programs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xrm.Sdk;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

[CollectionDefinition("PspPortalWebAppScenario")]
public class WebAppScenarioCollectionFixture : ICollectionFixture<PspPortalWebAppFixture>;

[Collection("PspPortalWebAppScenario")]
public abstract class PspPortalWebAppScenarioBase : WebAppScenarioBase
{
  protected new PspPortalWebAppFixture Fixture => (PspPortalWebAppFixture)base.Fixture;

  protected PspPortalWebAppScenarioBase(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }
}

public class PspPortalWebAppFixture : WebAppFixtureBase
{
  private IServiceScope serviceScope = null!;
  private ecer_PostSecondaryInstitute testPostSecondaryInstitute = null!;
  private ecer_ECEProgramRepresentative testProgramRepresentative = null!;
  private ecer_ECEProgramRepresentative secondaryProgramRepresentative = null!;
  private ecer_ECEProgramRepresentative tertiaryProgramRepresentative = null!;
  private ecer_ECEProgramRepresentative inactiveProgramRepresentative = null!;
  private ecer_PostSecondaryInstitute otherPostSecondaryInstitute = null!;
  private ecer_ECEProgramRepresentative otherInstituteRepresentative = null!;
  private ecer_PortalInvitation testPortalInvitationOne = null!;
  private ecer_ProvincialRequirement testAreaOfInstruction = null!;
  private UserIdentity testPspIdentity = null!;
  private ecer_Communication testCommunication1 = null!;
  private ecer_Communication testCommunication2 = null!;
  private ecer_Program testProgram = null!;
  private ecer_Course testCourse = null!;
  
  private static readonly ecer_CertificateLevel[] AreaOfInstructionCertificateLevels = { ecer_CertificateLevel.ITE, ecer_CertificateLevel.SNE };
  private static readonly ProgramTypes[] AreaOfInstructionProgramTypeValues = { ProgramTypes.ITE, ProgramTypes.SNE };
  private const int DefaultAreaOfInstructionMinimumHours = 40;

  public IServiceProvider Services => serviceScope.ServiceProvider;
  public UserIdentity AuthenticatedPspUserIdentity => testPspIdentity;
  public string AuthenticatedPspUserId => testProgramRepresentative.Id.ToString();
  public string PostSecondaryInstituteId => testPostSecondaryInstitute.ecer_PostSecondaryInstituteId?.ToString() ?? string.Empty;
  public string SecondaryPspUserId => secondaryProgramRepresentative.Id.ToString();
  public string TertiaryPspUserId => tertiaryProgramRepresentative.Id.ToString();
  public string InactivePspUserId => inactiveProgramRepresentative.Id.ToString();
  public string OtherInstitutePspUserId => otherInstituteRepresentative.Id.ToString();
  public Guid portalInvitationOneId => testPortalInvitationOne.ecer_PortalInvitationId ?? Guid.Empty;
  public string communicationOneId => testCommunication1.Id.ToString();
  public string communicationTwoId => testCommunication2.Id.ToString();
  
  public string programId => testProgram.Id.ToString();
  public string AreaOfInstructionId => testAreaOfInstruction.ecer_ProvincialRequirementId?.ToString() ?? string.Empty;
  public string AreaOfInstructionName => testAreaOfInstruction.ecer_Name ?? string.Empty;
  public int? AreaOfInstructionMinimumHours => testAreaOfInstruction?.ecer_MinimumHours;
  public IEnumerable<ProgramTypes> AreaOfInstructionProgramTypes => AreaOfInstructionProgramTypeValues;

  protected override void AddAuthorizationOptions(AuthorizationOptions opts)
  {
    ArgumentNullException.ThrowIfNull(opts);
    opts.AddPolicy("psp_user", new AuthorizationPolicyBuilder(opts.GetPolicy("psp_user")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.AddPolicy("psp_user_has_accepted_terms", new AuthorizationPolicyBuilder(opts.GetPolicy("psp_user_has_accepted_terms")!).AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme).Build());
    opts.DefaultPolicy = opts.GetPolicy("psp_user")!;
  }

  public override async Task InitializeAsync()
  {
    Host = await CreateHost<Clients.PSPPortal.Server.Program>();
    serviceScope = Host.Services.CreateScope();
    var context = serviceScope.ServiceProvider.GetRequiredService<EcerContext>();
    await InitializeDataverseTestData(context);
  }

  public override async Task DisposeAsync()
  {
    await Task.CompletedTask;
    serviceScope?.Dispose();
  }

  private async Task InitializeDataverseTestData(EcerContext context)
  {
    await Task.CompletedTask;

    testPostSecondaryInstitute = GetOrAddPostSecondaryInstitute(context);
    testProgramRepresentative = GetOrAddProgramRepresentative(context, testPostSecondaryInstitute);
    testPspIdentity = GetOrAddProgramRepresentativeIdentity(context, testProgramRepresentative);
    secondaryProgramRepresentative = GetOrAddProgramRepresentative(context, testPostSecondaryInstitute, $"{TestRunId}psp_rep_secondary", ecer_RepresentativeRole.Secondary, ecer_AccessToPortal.Invited);
    tertiaryProgramRepresentative = GetOrAddProgramRepresentative(context, testPostSecondaryInstitute, $"{TestRunId}psp_rep_tertiary", ecer_RepresentativeRole.Secondary, ecer_AccessToPortal.Active);
    inactiveProgramRepresentative = GetOrAddProgramRepresentative(context, testPostSecondaryInstitute, $"{TestRunId}psp_rep_inactive", ecer_RepresentativeRole.Secondary, ecer_AccessToPortal.Disabled);
    otherPostSecondaryInstitute = GetOrAddPostSecondaryInstitute(context, $"{TestRunId}psp_institute_other");
    otherInstituteRepresentative = GetOrAddProgramRepresentative(context, otherPostSecondaryInstitute, $"{TestRunId}psp_rep_other", ecer_RepresentativeRole.Secondary, ecer_AccessToPortal.Active);
    testPortalInvitationOne = GetOrAddPortalInvitation_PspProgramRepresentative(context, testProgramRepresentative, $"{TestRunId}psp_invite1");
    testAreaOfInstruction = GetOrAddAreaOfInstruction(context);
    
    testCommunication1 = GetOrAddCommunication(context, "comm1", null);
    testCommunication2 = GetOrAddCommunication(context, "comm2", null);
    
    testProgram = AddProgram(context, testPostSecondaryInstitute);
    testCourse = AddCourse(context, testProgram);
    
    context.SaveChanges();

    //load dependent properties
    context.Attach(testProgramRepresentative);
    context.LoadProperty(testProgramRepresentative, ecer_ECEProgramRepresentative.Fields.ecer_authentication_eceprogramrepresentative);

    context.SaveChanges();
  }

  private ecer_ProvincialRequirement GetOrAddAreaOfInstruction(EcerContext context)
  {
    var instructionName = $"{TestRunId}psp_area_instruction";
    var requirement = context.ecer_ProvincialRequirementSet.FirstOrDefault(r => r.ecer_Name == instructionName);

    if (requirement == null)
    {
      var requirementId = Guid.NewGuid();
      requirement = new ecer_ProvincialRequirement
      {
        Id = requirementId,
        ecer_ProvincialRequirementId = requirementId,
        ecer_Name = instructionName,
        ecer_MinimumHours = DefaultAreaOfInstructionMinimumHours,
        ecer_CertificateLevels = AreaOfInstructionCertificateLevels,
        StateCode = ecer_provincialrequirement_statecode.Active,
        StatusCode = ecer_ProvincialRequirement_StatusCode.Active
      };
      context.AddObject(requirement);
    }
    else
    {
      requirement.ecer_MinimumHours = DefaultAreaOfInstructionMinimumHours;
      requirement.ecer_CertificateLevels = AreaOfInstructionCertificateLevels;
      requirement.StateCode = ecer_provincialrequirement_statecode.Active;
      requirement.StatusCode = ecer_ProvincialRequirement_StatusCode.Active;
      context.UpdateObject(requirement);
    }

    return requirement;
  }
  
  private ecer_Course AddCourse(EcerContext context, ecer_Program program)
  {
    var course = new ecer_Course
    {
      ecer_Code = "101",
      ecer_CourseName = "Course 101",
      ecer_NewCourseHourDecimal = 20.00m,
      ecer_ProgramType = ecer_PSIProgramType.SNE,
      ecer_Programid = new EntityReference(ecer_Program.EntityLogicalName, program.Id)
    };
    context.AddObject(course);

    return course;
  }

  private ecer_Program AddProgram(EcerContext context, ecer_PostSecondaryInstitute institute)
  {
    string[] sneProgramTypes = { "SNE" };
    var program = new ecer_Program
    {
      StatusCode = ecer_Program_StatusCode.RequiresReview,
      ecer_ProgramId = Guid.NewGuid(),
      ecer_Name = "Draft",
      ecer_PortalStage = "stage1",
      ecer_StartDate = DateTime.UtcNow.Date,
      ecer_EndDate = DateTime.UtcNow.Date.AddYears(1),
      ecer_ProgramTypes = sneProgramTypes.Select(t => Enum.Parse<ecer_PSIProgramType>(t)),
      ecer_PostSecondaryInstitution = new EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, institute.Id)
    };
    context.AddObject(program);

    return program;
  }
  
  private ecer_Communication GetOrAddCommunication(EcerContext context, string message, Guid? parentCommunicationId)
  {
    var communication = context.ecer_CommunicationSet.FirstOrDefault(c => c.ecer_EducationInstitutionId.Id == testPostSecondaryInstitute.Id && c.ecer_Message == message && c.StatusCode == ecer_Communication_StatusCode.NotifiedRecipient);

    if (communication == null)
    {
      communication = new ecer_Communication
      {
        Id = Guid.NewGuid(),
        ecer_Message = message,
        ecer_Acknowledged = false,
        StatusCode = ecer_Communication_StatusCode.NotifiedRecipient,
      };
      if (parentCommunicationId == null)
      {
        communication.ecer_IsRoot = true;
      }
      context.AddObject(communication);

      if (parentCommunicationId != null)
      {
        var parent = context.ecer_CommunicationSet.SingleOrDefault(d => d.ecer_CommunicationId == parentCommunicationId);
        var Referencingecer_communication_ParentCommunicationid = new Relationship(ecer_Communication.Fields.Referencingecer_communication_ParentCommunicationid)
        {
          PrimaryEntityRole = EntityRole.Referencing
        };
        context.AddLink(communication, Referencingecer_communication_ParentCommunicationid, parent);
      }
      
    }

    return communication;
  }

  private ecer_PostSecondaryInstitute GetOrAddPostSecondaryInstitute(EcerContext context, string? nameOverride = null)
  {
    var instituteName = nameOverride ?? $"{TestRunId}psp_institute";
    var institute = context.ecer_PostSecondaryInstituteSet.FirstOrDefault(i => i.ecer_Name == instituteName);

    if (institute == null)
    {
      var instituteId = Guid.NewGuid();
      institute = new ecer_PostSecondaryInstitute
      {
        Id = instituteId,
        ecer_PostSecondaryInstituteId = instituteId,
        ecer_Name = instituteName,
        ecer_City = "Victoria"
      };

      context.AddObject(institute);
    }

    return institute;
  }

  private ecer_ECEProgramRepresentative GetOrAddProgramRepresentative(EcerContext context, ecer_PostSecondaryInstitute institute, string? repNameOverride = null, ecer_RepresentativeRole role = ecer_RepresentativeRole.Primary, ecer_AccessToPortal access = ecer_AccessToPortal.Active)
  {
    var repName = repNameOverride ?? $"{TestRunId}psp_rep";
    var representative = context.ecer_ECEProgramRepresentativeSet.FirstOrDefault(r => r.ecer_FirstName == repName);

    if (representative == null)
    {
      var repId = Guid.NewGuid();
      representative = new ecer_ECEProgramRepresentative
      {
        Id = repId,
        ecer_ECEProgramRepresentativeId = repId,
        ecer_Name = $"{repName}_name",
        ecer_FirstName = repName,
        ecer_LastName = "autotest",
        ecer_PreferredFirstName = repName,
        ecer_EmailAddress = "psp_rep@test.gov.bc.ca",
        ecer_PhoneNumber = "5555555555",
        ecer_PhoneExtension = "123",
        ecer_Role = "Program Representative",
        ecer_RepresentativeRole = ecer_RepresentativeRole.Primary,
        ecer_AccessToPortal = access,
        ecer_HasAcceptedTermsofUse = true,
        StateCode = ecer_eceprogramrepresentative_statecode.Active,
        StatusCode = ecer_ECEProgramRepresentative_StatusCode.Active,
        ecer_PostSecondaryInstitute = new EntityReference(ecer_PostSecondaryInstitute.EntityLogicalName, institute.Id)
      };
      context.AddObject(representative);
      context.AddLink(representative, new Relationship(ecer_ECEProgramRepresentative.Fields.ecer_eceprogramrepresentative_PostSecondaryIns), institute);
    }

    representative.ecer_RepresentativeRole = role;
    representative.ecer_AccessToPortal = access;
    context.UpdateObject(representative);
    return representative;
  }

  private UserIdentity GetOrAddProgramRepresentativeIdentity(EcerContext context, ecer_ECEProgramRepresentative representative)
  {
    var identity = new UserIdentity($"{TestRunId}psp_user1", "bceidbusiness");
    var authentication = context.ecer_AuthenticationSet.FirstOrDefault(a => a.ecer_IdentityProvider == identity.IdentityProvider && a.ecer_ExternalID == identity.UserId);

    if (authentication == null)
    {
      authentication = new ecer_Authentication
      {
        Id = Guid.NewGuid(),
        ecer_IdentityProvider = identity.IdentityProvider,
        ecer_ExternalID = identity.UserId
      };
      context.AddObject(authentication);
      context.AddLink(authentication, new Relationship(ecer_Authentication.Fields.ecer_authentication_eceprogramrepresentative), representative);
    }

    return identity;
  }

  private ecer_PortalInvitation GetOrAddPortalInvitation_PspProgramRepresentative(EcerContext context, ecer_ECEProgramRepresentative representative, string name)
  {
    var portalInvitation = context.ecer_PortalInvitationSet.FirstOrDefault(p => p.ecer_Name == name &&
                                                                                p.StatusCode == ecer_PortalInvitation_StatusCode.Sent &&
                                                                                p.ecer_Type == ecer_PortalInvitationTypes.PSIProgramRepresentative);

    if (portalInvitation == null)
    {
      var guid = Guid.NewGuid();
      portalInvitation = new ecer_PortalInvitation
      {
        Id = guid,
        ecer_PortalInvitationId = guid,
        ecer_Name = name,
        ecer_FirstName = representative.ecer_FirstName,
        ecer_LastName = representative.ecer_LastName,
        ecer_EmailAddress = representative.ecer_EmailAddress,
        StatusCode = ecer_PortalInvitation_StatusCode.Sent,
        ecer_Type = ecer_PortalInvitationTypes.PSIProgramRepresentative,
      };

      context.AddObject(portalInvitation);
      context.AddLink(portalInvitation, new Relationship(ecer_PortalInvitation.Fields.ecer_portalinvitation_psiprogramrepresentativeid), representative);
    }

    return portalInvitation;
  }

}
