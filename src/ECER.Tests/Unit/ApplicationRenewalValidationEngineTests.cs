using Bogus;
using ECER.Engines.Validation.Applications;
using ECER.Managers.Registry.Contract.Applications;
using ECER.Resources.Documents.Certifications;
using Moq;

namespace ECER.Tests.Unit;

public class ApplicationRenewalValidationEngineTests
{
  private readonly ApplicationRenewalValidationEngine _validator;
  private readonly Mock<ICertificationRepository> _certificationRepositoryMock;
  private readonly Faker _faker;

  public ApplicationRenewalValidationEngineTests()
  {
    _certificationRepositoryMock = new Mock<ICertificationRepository>();
    _validator = new ApplicationRenewalValidationEngine(_certificationRepositoryMock.Object);
    _faker = new Faker("en_CA");
  }

  [Fact]
  public async Task Validate_OneYearRenewalWithActiveCertificate_ReturnsNoValidationError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.OneYear },
      ExplanationLetter = "This is an explanation letter",
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
                  CreateMockCertification(DateTime.Now.AddYears(1), CertificateStatusCode.Active) // Certification is active
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_OneYearRenewalWithActiveCertificateWithoutExplanationLetter_ReturnsExplanationLetterError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.OneYear },
      ExplanationLetter = null, // No explanation letter
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
                  CreateMockCertification(DateTime.Now.AddYears(1), CertificateStatusCode.Active) // Certification is active
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("the application does not have explanation letter", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_OneYearRenewalWithActiveCertificateWithoutCharacterReferences_ReturnsCharacterReferenceError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.OneYear },
      ExplanationLetter = "This is an explanation letter",
      CharacterReferences = new List<CharacterReference>() // No character references
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
                  CreateMockCertification(DateTime.Now.AddYears(1), CertificateStatusCode.Active) // Certification is active
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("the application does not have any character references", result.ValidationErrors);
  }

  private Certification CreateMockCertification(DateTime expiryDate, CertificateStatusCode statusCode)
  {
    return new Certification(_faker.Random.Guid().ToString())
    {
      ExpiryDate = expiryDate,
      StatusCode = statusCode,
      Number = _faker.Random.String(10),
      EffectiveDate = expiryDate.AddYears(-1),
      Date = expiryDate.AddYears(-1),
      PrintDate = expiryDate.AddMonths(-1),
      HasConditions = _faker.Random.Bool(),
      LevelName = _faker.Company.CompanyName(),
      IneligibleReference = _faker.Random.Enum<YesNoNull>(),
      Levels = new List<CertificationLevel>
              {
                  new CertificationLevel(_faker.Random.Guid().ToString()) { Type = _faker.Random.Word() }
              },
      Files = new List<CertificationFile>
              {
                  new CertificationFile(_faker.Random.Guid().ToString())
                  {
                      Url = _faker.Internet.Url(),
                      Extention = _faker.System.FileExt(),
                      Size = _faker.Random.Number(1000, 10000).ToString(),
                      Name = _faker.System.FileName()
                  }
              }
    };
  }

  private CharacterReference CreateMockCharacterReference()
  {
    return new CharacterReference(
        _faker.Name.FirstName(),
        _faker.Name.LastName(),
        _faker.Internet.Email(),
        _faker.Phone.PhoneNumber()
    );
  }
}
