using Bogus;
using ECER.Engines.Validation.Applications;
using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Tests.Unit;

public class ApplicationSubmissionValidationEngineTests
{
  private readonly ApplicationSubmissionValidationEngine _validator;

  public ApplicationSubmissionValidationEngineTests()
  {
    _validator = new ApplicationSubmissionValidationEngine();
  }

  [Fact]
  public async Task Validate_WithoutEducation_ReturnsEducationError()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      Transcripts = new List<Transcript>(),
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      CertificationTypes = new List<CertificationType> { CertificationType.EceAssistant },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(500) }
    };

    var result = await _validator.Validate(application);
    Assert.Contains("the application does not have any education", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithoutCharacterReferences_ReturnsCharacterReferenceError()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      Transcripts = new List<Transcript> { CreateMockTranscript(false) },
      CharacterReferences = new List<CharacterReference>(),
      CertificationTypes = new List<CertificationType> { CertificationType.EceAssistant },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(500) }
    };

    var result = await _validator.Validate(application);
    Assert.Contains("the application does not have any character references", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithOutdatedEceAssistantEducation_ReturnsOutdatedEducationError()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      Transcripts = new List<Transcript> { CreateMockTranscript(true) },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      CertificationTypes = new List<CertificationType> { CertificationType.EceAssistant },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(500) }
    };

    var result = await _validator.Validate(application);
    Assert.Contains("Education was completed more than 5 years ago", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithInadequateWorkExperienceForFiveYears_ReturnsWorkExperienceError()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      Transcripts = new List<Transcript> { CreateMockTranscript(false) },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(499) }
    };

    var result = await _validator.Validate(application);
    Assert.Contains("Work experience does not meet 500 hours", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_WithSneAndIteWithoutFiveYears_ReturnsCertificationError()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      Transcripts = new List<Transcript> { CreateMockTranscript(false) },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      CertificationTypes = new List<CertificationType> { CertificationType.Sne, CertificationType.Ite },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(500) }
    };

    var result = await _validator.Validate(application);
    Assert.Contains("Sub five year certification type selected but without five year certification", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_ValidApplication_ReturnsNoValidationError()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      Transcripts = new List<Transcript> { CreateMockTranscript(false), CreateMockTranscript(false), CreateMockTranscript(false) },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(500) }
    };

    var result = await _validator.Validate(application);
    Assert.Empty(result.ValidationErrors);
  }

  private static Transcript CreateMockTranscript(bool invalid)
  {
    DateTime startDate = DateTime.Now.AddYears(-2);
    DateTime endDate = DateTime.Now.AddYears(-1);
    if (invalid)
    {
      startDate = DateTime.Now.AddYears(-20);
      endDate = DateTime.Now.AddYears(-19);
    }

    var faker = new Faker("en_CA");
    var transcript = new Transcript(null, faker.Company.CompanyName(), $"{faker.Hacker.Adjective()} Program", faker.Random.Number(10000000, 99999999).ToString(), startDate, endDate, faker.Random.Bool(), faker.Random.Bool(), faker.Random.Bool(), faker.Name.FirstName(), faker.Name.LastName(), faker.Random.Bool());

    return transcript;
  }

  private static CharacterReference CreateMockCharacterReference()
  {
    var faker = new Faker("en_CA");

    return new CharacterReference(
      faker.Name.FirstName(), faker.Name.LastName(), faker.Internet.Email(), faker.Phone.PhoneNumber()
    );
  }

  private WorkExperienceReference CreateMockWorkExperienceReference(int hours)
  {
    var faker = new Faker("en_CA");

    return new WorkExperienceReference(
       faker.Name.FirstName(), faker.Name.FirstName(), faker.Internet.Email(), hours
    )
    {
      PhoneNumber = faker.Phone.PhoneNumber()
    };
  }
}
