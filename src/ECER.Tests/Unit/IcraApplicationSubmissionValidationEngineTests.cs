using Bogus;
using ECER.Engines.Validation.Applications;
using ECER.Managers.Registry.Contract.Applications;

namespace ECER.Tests.Unit;

public class IcraApplicationSubmissionValidationEngineTests
{
  private readonly IcraApplicationSubmissionValidationEngine _validator;

  public IcraApplicationSubmissionValidationEngineTests()
  {
    _validator = new IcraApplicationSubmissionValidationEngine();
  }

  [Fact]
  public async Task Validate_WithoutInformation_ReturnsEducationErrors()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
    };

    var result = await _validator.Validate(application);
    Assert.Contains("the application does not have any education", result.ValidationErrors);
    Assert.Contains("application is not associated with a five year certification type", result.ValidationErrors);
    Assert.Contains("the application does not have any character references", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_ReturnsSuccess()
  {
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      Transcripts = new List<Transcript>
      {
        CreateMockTranscript(false)
      },
      CharacterReferences = new List<CharacterReference>
      {
        CreateMockCharacterReference(),
      }
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
    var transcript = new Transcript(null, faker.Company.CompanyName(), $"{faker.Hacker.Adjective()} Program", faker.Random.Number(10000000, 99999999).ToString(), startDate, endDate, faker.Random.Bool(), faker.Name.FirstName(), faker.Name.LastName(), faker.Random.Bool(), EducationRecognition.Recognized, EducationOrigin.InsideBC)
    {
      TranscriptStatusOption = TranscriptStatusOptions.OfficialTranscriptRequested
    };

    return transcript;
  }

  private static CharacterReference CreateMockCharacterReference()
  {
    var faker = new Faker("en_CA");

    return new CharacterReference(
      "faketest", "faketest", "fake@test.com", faker.Phone.PhoneNumber()
    );
  }
}
