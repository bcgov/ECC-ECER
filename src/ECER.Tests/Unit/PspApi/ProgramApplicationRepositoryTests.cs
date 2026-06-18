using ECER.Resources.Documents.ProgramApplications; // Make sure this matches your repo namespace
using ECER.Utilities.DataverseSdk.Model;
using Shouldly;

namespace ECER.Tests.Unit.PspApi
{
  public class ProgramApplicationRepositoryTests
  {
    private readonly ProgramApplicationRepositoryMapper mapper = new ProgramApplicationRepositoryMapper();

    private (ProgramApplication incomingApplication, ecer_PostSecondaryInstituteProgramApplicaiton entityApplication) CreateBaselineIncomingApplication(ApplicationType Type, DeliveryType DeliveryType)
    {
      var baseDate = DateTime.UtcNow;
      var incomingApplication = new ProgramApplication(
          Id: Guid.NewGuid().ToString(),
          PostSecondaryInstituteId: Guid.NewGuid().ToString()
      )
      {
        // Identification & Type Metadata
        ProgramApplicationName = "Fully Populated ECE Program",
        ProgramApplicationType = Type,
        Status = ApplicationStatus.Draft,
        ProgramTypes = new List<ProgramCertificationType>
        {
            ProgramCertificationType.Basic,
            ProgramCertificationType.ITE,
            ProgramCertificationType.SNE
        },
        DeliveryType = DeliveryType,
        ComponentsGenerationCompleted = true,
        ProgramRepresentativeId = Guid.NewGuid().ToString(),

        // Program Length Metrics
        BasicProgramLength = 45.0f,
        IteProgramLength = 15.5f,
        SneProgramLength = 20.0f,

        // Strategy & Delivery Configuration
        OnlineMethodOfInstruction = new List<MethodofInstruction>
        {
            MethodofInstruction.Asynchronous,
        },
        DeliveryMethod = new List<DeliveryMethodforInstructor>
        {
            DeliveryMethodforInstructor.Inpersonsitevisits,
        },
        EnrollmentOptions = new List<WorkHoursType>
        {
            WorkHoursType.FullTime,
        },
        AdmissionOptions = new List<AdmissionOptions>
        {
            AdmissionOptions.Other
        },
        OtherAdmissionOptions = "Other answer",

        // Capacity and Metrics
        MinimumEnrollment = 12.0f,
        MaximumEnrollment = 40.0f,
        InPersonHoursPercentage = 60.0f,
        OnlineDeliveryHoursPercentage = 40.0f,

        // Nested Collection Entities
        ProgramCampuses = new List<ProgramCampus>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                CampusId = Guid.NewGuid().ToString(),
                StartDate = baseDate.AddDays(14),
                EndDate = baseDate.AddYears(1)
            }
        },
      };

      var entityApplication = mapper.MapProgramApplication(incomingApplication);
      entityApplication.ecer_postsecondaryinstituteprogramapplicaiton_PSIProgramRepresentative_ecer_eceprogramrepresentativ = new ecer_ECEProgramRepresentative
      {
        Id = Guid.NewGuid()
      };

      return (incomingApplication, entityApplication);
    }

    [Theory]
    // Application type NewBasicECEPostBasicProgram
    [InlineData("Valid", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, true)]
    [InlineData("NameEmpty", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingRepresentative", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("NoMinEnrollment", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("NoMaxEnrollment", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingCampus", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingProgramLengthEntry", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingProgramEnrollment", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingAdmissionOption", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingAdmissionOptionOtherAnswer", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingOnlineMethodOfInstruction", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingOnlineMethodOfInstruction", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Inperson, true)] //should not matter for in-person delivery
    [InlineData("MissingDeliveryMethodForPracticumInstructor", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Online, false)]
    [InlineData("MissingOnlineDeliveryHoursPercentage", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingInPersonHoursPercentage", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingInPersonHoursPercentage", ApplicationType.NewBasicECEPostBasicProgram, DeliveryType.Online, true)] //should not matter for online delivery
    // NewCampusPrivateInstitution
    [InlineData("Valid", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, true)]
    [InlineData("NameEmpty", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("MissingRepresentative", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("NoMinEnrollment", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("NoMaxEnrollment", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("MissingProgramLengthEntry", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("MissingProgramEnrollment", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("MissingAdmissionOption", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("MissingOnlineMethodOfInstruction", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    [InlineData("MissingDeliveryMethodForPracticumInstructor", ApplicationType.NewCampusatRecognizedPrivateInstitution, DeliveryType.Hybrid, false)]
    //SatelliteProgram
    [InlineData("Valid", ApplicationType.SatelliteProgram, DeliveryType.Hybrid, true)]
    [InlineData("NameEmpty", ApplicationType.SatelliteProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingRepresentative", ApplicationType.SatelliteProgram, DeliveryType.Hybrid, false)]
    [InlineData("NoMinEnrollment", ApplicationType.SatelliteProgram, DeliveryType.Hybrid, false)]
    [InlineData("NoMaxEnrollment", ApplicationType.SatelliteProgram, DeliveryType.Hybrid, false)]
    [InlineData("MissingCampus", ApplicationType.SatelliteProgram, DeliveryType.Hybrid, false)]
    //OnlineOrHybridDeliveryMethod
    [InlineData("Valid", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, true)]
    [InlineData("NameEmpty", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingRepresentative", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("NoMinEnrollment", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("NoMaxEnrollment", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingCampus", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingProgramLengthEntry", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingOnlineMethodOfInstruction", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingDeliveryMethodForPracticumInstructor", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Online, false)]
    [InlineData("MissingProgramEnrollment", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingAdmissionOption", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingOnlineDeliveryHoursPercentage", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    [InlineData("MissingInPersonHoursPercentage", ApplicationType.AddOnlineorHybridDeliveryMethod, DeliveryType.Hybrid, false)]
    public void ValidateInstituteInfo_ShouldReturnTrueIfComplete_FalseWhenFieldsMissing(string Scenario, ApplicationType AppType, DeliveryType DeliveryType, bool outcome)
    {
      var (incomingApplication, application) = CreateBaselineIncomingApplication(AppType, DeliveryType);

      switch (Scenario)
      {
        case "Valid":
          // No changes needed, this is our control scenario
          break;

        case "NameEmpty":
          application.ecer_Name = string.Empty;
          break;

        case "MissingRepresentative":
          application.ecer_postsecondaryinstituteprogramapplicaiton_PSIProgramRepresentative_ecer_eceprogramrepresentativ = null;
          break;

        case "NoMinEnrollment":
          application.ecer_MinimumStudentEnrollmentperCourse = null;
          break;

        case "NoMaxEnrollment":
          application.ecer_MaximumStudentEnrollmentperCourse = null;
          break;

        case "MissingCampus":
          incomingApplication.ProgramCampuses = new List<ProgramCampus>();
          break;

        case "MissingProgramLengthEntry":
          incomingApplication.BasicProgramLength = null;
          break;

        case "MissingProgramEnrollment":
          application.ecer_ProgramEnrollment = null;
          break;

        case "MissingAdmissionOption":
          application.ecer_AdmissionOptions = null;
          break;

        case "MissingAdmissionOptionOtherAnswer":
          application.ecer_OtherAdmissionOptions = null;
          break;

        case "MissingOnlineMethodOfInstruction":
          application.ecer_Onlinemethodsofinstruction = null;
          break;

        case "MissingDeliveryMethodForPracticumInstructor":
          application.ecer_Deliverymethodforpracticuminstructor = null;
          break;

        case "MissingOnlineDeliveryHoursPercentage":
          application.ecer_OnlineDeliveryHoursPercentage = null;
          break;

        case "MissingInPersonHoursPercentage":
          application.ecer_InpersonHoursPercentage = null;
          break;

        default:
          throw new ArgumentException($"Test configuration error: The scenario '{Scenario}' was provided in InlineData but has no matching setup handling inside the test switch block.");
      }

      var result = ProgramApplicationRepository.ValidateInstituteInfo(application, incomingApplication);
      result.ShouldBe(outcome, $"Scenario {Scenario} for app type {AppType} for delivery method {DeliveryType} failed expected to be {outcome} but got {result}");
    }
  }
}
