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
      OneYearRenewalExplanationChoice = OneYearRenewalexplanations.IliveandworkinacommunitywithoutothercertifiedECEs,
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
  public async Task Validate_OneYearRenewalWithActiveCertificateOneYearRenewalOtherWithoutExplanationLetter_ReturnsExplanationLetterError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.OneYear },
      OneYearRenewalExplanationChoice = OneYearRenewalexplanations.Other,
      RenewalExplanationOther = null, // No explanation letter
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
    Assert.Contains("renewal explanation other required if one year renewal explanation choice is other", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_OneYearRenewalWithActiveCertificateWithoutCharacterReferences_ReturnsCharacterReferenceError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.OneYear },
      RenewalExplanationOther = "This is an explanation letter",
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

  [Fact]
  public async Task Validate_FiveYearRenewalWithActiveCertificate_ReturnsNoValidationError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      ProfessionalDevelopments = new List<ProfessionalDevelopment> { CreateMockProfessionalDevelopment() },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(400) }
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
  public async Task Validate_ExpiredLessThanFiveYearsWithMissingProfessionalDevelopment_ReturnsProfessionalDevelopmentError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      ProfessionalDevelopments = new List<ProfessionalDevelopment>(), // No professional development
      RenewalExplanationOther = "This is an explanation letter",
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(400) }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-3), CertificateStatusCode.Expired) // Certification expired less than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("the application does not have any professional development", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_CertificateExpiredMoreThanFiveYearsAgo_ReturnsWorkExperienceError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      ProfessionalDevelopments = new List<ProfessionalDevelopment> { CreateMockProfessionalDevelopment() },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(300) } // Less than 500 hours
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-6), CertificateStatusCode.Expired) // Certification expired more than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("You must provide 500 hours of work experience", result.ValidationErrors);
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

  [Fact]
  public async Task Validate_EceAssistantWithExpiredLessThanFiveYearsNoEducation_ReturnsEducationError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.EceAssistant },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(400) },
      Transcripts = new List<Transcript>() // No education transcripts
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-3), CertificateStatusCode.Expired) // Expired less than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("the application does not have one education", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_EceAssistantWithExpiredMoreThanFiveYears_ReturnsNoValidationError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.EceAssistant },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(500) }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-6), CertificateStatusCode.Expired) // Expired more than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_OneYearRenewalWithExpiredLessThanFiveYearsNoProfessionalDevelopment_ReturnsProfessionalDevelopmentError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.OneYear },
      ProfessionalDevelopments = new List<ProfessionalDevelopment>(), // No professional development
      RenewalExplanationOther = "This is an explanation letter",
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-3), CertificateStatusCode.Expired) // Expired less than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("the application does not have any professional development", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_FiveYearsWithExpiredLessThanFiveYearsWithoutFiveYearExplanationChoice_ReturnsError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      ProfessionalDevelopments = new List<ProfessionalDevelopment> { CreateMockProfessionalDevelopment() },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(400) }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-3), CertificateStatusCode.Expired) // Expired less than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("You must provide a reason for late renewal", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_ExpiredMoreThanFiveYearsNoProfessionalDevelopment_ReturnsProfessionalDevelopmentError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      ProfessionalDevelopments = new List<ProfessionalDevelopment>(), // No professional development
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(500) }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-6), CertificateStatusCode.Expired) // Expired more than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("the application does not have any professional development", result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_OneYearRenewalWithExpiredMoreThanFiveYears_ReturnsNoValidationError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.OneYear },
      RenewalExplanationOther = "This is an explanation letter",
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification> {
            CreateMockCertification(DateTime.Now.AddYears(-6), CertificateStatusCode.Expired) // Expired more than 5 years ago
        });

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_FiveYearsRenewalWithNoCertificateFound_ReturnsNoValidationError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.FiveYears },
      ProfessionalDevelopments = new List<ProfessionalDevelopment> { CreateMockProfessionalDevelopment() },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(400) }
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification>()); // No certificate found

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_EceAssistantWithNoCertificateFound_ReturnsNoValidationError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft)
    {
      CertificationTypes = new List<CertificationType> { CertificationType.EceAssistant },
      CharacterReferences = new List<CharacterReference> { CreateMockCharacterReference() },
      WorkExperienceReferences = new List<WorkExperienceReference> { CreateMockWorkExperienceReference(400) },
      Transcripts = new List<Transcript> { CreateMockTranscript() } // With transcripts
    };

    _certificationRepositoryMock
        .Setup(repo => repo.Query(It.IsAny<UserCertificationQuery>()))
        .ReturnsAsync(new List<Certification>()); // No certificate found

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Empty(result.ValidationErrors);
  }

  [Fact]
  public async Task Validate_DraftApplicationWithoutCertificationType_ReturnsError()
  {
    // Arrange
    var application = new Application("id", "registrantId", ApplicationStatus.Draft){};

    // Act
    var result = await _validator.Validate(application);

    // Assert
    Assert.Contains("Application is not associated with a certification type", result.ValidationErrors);
  }

  private Transcript CreateMockTranscript()
  {
    return new Transcript(
        _faker.Random.Guid().ToString(), // Id
        _faker.Company.CompanyName(), // EducationalInstitutionName
        _faker.Name.FullName(), // ProgramName
        _faker.Random.String2(8, "0123456789"), // StudentNumber
        _faker.Date.Past(5), // StartDate (random date within the last 5 years)
        _faker.Date.Recent(365), // EndDate (random date within the last year)
        _faker.Random.Bool(), // IsECEAssistant
        TranscriptStatusOptions.OfficialTranscriptRequested, // Transcript status option
        _faker.Name.FirstName(), // StudentFirstName
        _faker.Name.LastName(), // StudentLastName
        _faker.Random.Bool(), // IsNameUnverified
        _faker.Random.Enum<EducationRecognition>(), //EducationRecognition
        _faker.Random.Enum<EducationOrigin>() //EducationOrigin
    )
    {
      CampusLocation = _faker.Address.City(), // Random city for CampusLocation
      Status = _faker.Random.Enum<TranscriptStage>(), // Random enum value for Status
      StudentMiddleName = _faker.Name.FirstName() // Random middle name
    };
  }

  private CharacterReference CreateMockCharacterReference()
  {
    return new CharacterReference(
        _faker.Name.FirstName(),
        _faker.Name.LastName(),
        "fake@test.com",
        _faker.Phone.PhoneNumber()
    );
  }

  private ProfessionalDevelopment CreateMockProfessionalDevelopment()
  {
    return new ProfessionalDevelopment(
        Guid.NewGuid().ToString(), // Id
        _faker.Company.CatchPhrase(), // CourseName
        _faker.Company.CompanyName(), // OrganizationName
        DateTime.Now.AddMonths(-3), // StartDate
        DateTime.Now.AddMonths(-1) // EndDate
    )
    {
      OrganizationContactInformation = _faker.Phone.PhoneNumber(),
      InstructorName = _faker.Name.FullName(),
      NumberOfHours = _faker.Random.Int(1, 40),
      Status = _faker.Random.Enum<ProfessionalDevelopmentStatusCode>(),
      DeletedFiles = new List<string> { _faker.System.FileName() },
      NewFiles = new List<string> { _faker.System.FileName() },
    };
  }

  private WorkExperienceReference CreateMockWorkExperienceReference(int hours)
  {
    return new WorkExperienceReference(
        _faker.Name.FirstName(), // FirstName
        _faker.Name.LastName(),  // LastName
        "fake@test.com", // EmailAddress
        hours                    // Hours
    )
    {
      Id = Guid.NewGuid().ToString(), // Random GUID as Id
      PhoneNumber = _faker.Phone.PhoneNumber(), // Random phone number
      Status = _faker.Random.Enum<WorkExperienceRefStage>(), // Random enum value for Status
      WillProvideReference = _faker.Random.Bool(), // Random boolean
      TotalNumberofHoursApproved = _faker.Random.Int(0, hours), // Random approved hours up to the total hours
      TotalNumberofHoursObserved = _faker.Random.Int(0, hours)  // Random observed hours up to the total hours
    };
  }
}
