using Alba;
using Bogus;
using ECER.Clients.PSPPortal.Server.ProgramApplications;
using Shouldly;
using System.Net;
using System.Net.Http.Headers;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.PspApi;

public class ProgramApplicationTest : PspPortalWebAppScenarioBase
{
  public ProgramApplicationTest(ITestOutputHelper output, PspPortalWebAppFixture fixture) : base(output, fixture)
  {
  }

  private readonly Faker faker = new Faker("en_CA");

  [Fact]
  public async Task CreateProgramApplication_ReturnsOkAndCreatedApplication()
  {
    var request = new CreateProgramApplicationRequest
    {
      ProgramApplicationName = $"Test program application {Guid.NewGuid():N}",
      ProgramTypes = new[] { ProgramCertificationType.Basic, ProgramCertificationType.ITE },
      DeliveryType = DeliveryType.Hybrid
    };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl("/api/programApplications");
      _.StatusCodeShouldBeOk();
    });

    var createResult = await response.ReadAsJsonAsync<CreateProgramApplicationResponse>();
    createResult.ShouldNotBeNull();
    createResult.ProgramApplication.ShouldNotBeNull();
    createResult.ProgramApplication.Id.ShouldNotBeNull();
    createResult.ProgramApplication.Status.ShouldBe(ApplicationStatus.Draft);
    createResult.ProgramApplication.ProgramApplicationName.ShouldBe(request.ProgramApplicationName);
    createResult.ProgramApplication.DeliveryType.ShouldBe(request.DeliveryType);
    createResult.ProgramApplication.PostSecondaryInstituteId.ShouldBe(Fixture.PostSecondaryInstituteId);

    var getResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{createResult.ProgramApplication.Id}");
      _.StatusCodeShouldBeOk();
    });

    var getResult = await getResponse.ReadAsJsonAsync<GetProgramApplicationResponse>();
    getResult.ShouldNotBeNull();
    var fetched = getResult.Applications!.FirstOrDefault(a => a.Id == createResult.ProgramApplication.Id).ShouldNotBeNull();
    fetched.Status.ShouldBe(ApplicationStatus.Draft);
    fetched.ProgramApplicationName.ShouldBe(request.ProgramApplicationName);
    fetched.DeliveryType.ShouldBe(DeliveryType.Hybrid);
  }

  [Fact]
  public async Task CreateProgramApplication_ForNewCampus_ReturnsBadRequest()
  {
    var request = new CreateProgramApplicationRequest
    {
      ProgramApplicationName = $"Test program application {Guid.NewGuid():N}",
      ProgramTypes = new[] { ProgramCertificationType.Basic, ProgramCertificationType.ITE },
      DeliveryType = DeliveryType.Hybrid,
      ProgramApplicationType = ApplicationType.NewCampusatRecognizedPrivateInstitution
    };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl("/api/programApplications");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task CreateProgramApplication_ForNewCampus_ReturnsOkAndCreatedApplication()
  {
    var request = new CreateProgramApplicationRequest
    {
      ProgramApplicationName = $"Test program application {Guid.NewGuid():N}",
      ProgramTypes = new[] { ProgramCertificationType.Basic, ProgramCertificationType.ITE },
      DeliveryType = DeliveryType.Hybrid,
      ProgramApplicationType = ApplicationType.NewCampusatRecognizedPrivateInstitution,
      ProgramProfileId = Fixture.approvedProgramId,
      CampusId = Guid.NewGuid().ToString()
    };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl("/api/programApplications");
      _.StatusCodeShouldBeOk();
    });

    var createResult = await response.ReadAsJsonAsync<CreateProgramApplicationResponse>();
    createResult.ShouldNotBeNull();
    createResult.ProgramApplication.ShouldNotBeNull();
    createResult.ProgramApplication.Id.ShouldNotBeNull();
    createResult.ProgramApplication.Status.ShouldBe(ApplicationStatus.Draft);
    createResult.ProgramApplication.ProgramApplicationName.ShouldBe(request.ProgramApplicationName);
    createResult.ProgramApplication.DeliveryType.ShouldBe(request.DeliveryType);
    createResult.ProgramApplication.PostSecondaryInstituteId.ShouldBe(Fixture.PostSecondaryInstituteId);
  }

  [Fact]
  public async Task GetAllProgramApplications_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/null");
      _.StatusCodeShouldBeOk();
    });

    var status = await response.ReadAsJsonAsync<GetProgramApplicationResponse>();
    status.ShouldNotBeNull();

    var firstApplication = status.Applications!.Where(app => app.Id == Fixture.programApplicationId).ShouldNotBeNull().First();
    firstApplication.Status.ShouldBe(ApplicationStatus.Submitted);
    firstApplication.StatusReasonDetail.ShouldBe(ApplicationStatusReasonDetail.RFAIrequested);
  }

  [Fact]
  public async Task GetProgramApplication_ById_ReturnsStatusOk()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{this.Fixture.programApplicationId}");
      _.StatusCodeShouldBeOk();
    });

    var status = await response.ReadAsJsonAsync<GetProgramApplicationResponse>();
    status.ShouldNotBeNull();

    var firstApplication = status.Applications!.FirstOrDefault().ShouldNotBeNull();
    firstApplication.Status.ShouldBe(ApplicationStatus.Submitted);
    firstApplication.StatusReasonDetail.ShouldBe(ApplicationStatusReasonDetail.RFAIrequested);
    firstApplication.DeliveryType.ShouldBe(DeliveryType.Hybrid);
  }

  [Fact]
  public async Task UpdateProgramApplication_Type_Draft_ToWithdraw_ReturnsOk()
  {
    var program = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{this.Fixture.draftProgramApplicationId}");
      _.StatusCodeShouldBeOk();
    });

    var status = await program.ReadAsJsonAsync<GetProgramApplicationResponse>();
    status.ShouldNotBeNull();
    var application = status.Applications!.First();
    application.Status = ApplicationStatus.Withdrawn;

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(application).ToUrl($"/api/programApplications/{Fixture.draftProgramApplicationId}");
      _.StatusCodeShouldBeOk();
    });
  }

  [Fact]
  public async Task UpdateProgramApplication_Type_Draft_ReturnsOk()
  {
    var program = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{this.Fixture.draftProgramApplication2Id}");
      _.StatusCodeShouldBeOk();
    });

    var status = await program.ReadAsJsonAsync<GetProgramApplicationResponse>();
    status.ShouldNotBeNull();
    var application = status.Applications!.First();
    application.EnrollmentOptions = new[] { WorkHoursType.FullTime };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(application).ToUrl($"/api/programApplications/{Fixture.draftProgramApplication2Id}");
      _.StatusCodeShouldBeOk();
    });

    var updatedProgram = await Host.Scenario(_ =>
    {
      _.WithPspUser(this.Fixture.AuthenticatedPspUserIdentity, this.Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{this.Fixture.draftProgramApplication2Id}");
      _.StatusCodeShouldBeOk();
    });

    var updatedProgramStatus = await updatedProgram.ReadAsJsonAsync<GetProgramApplicationResponse>();
    updatedProgramStatus.ShouldNotBeNull();
    var updatedApplication = status.Applications!.First();
    updatedApplication.EnrollmentOptions.ShouldNotBeNull();
    updatedApplication.EnrollmentOptions.ShouldNotBeEmpty();
    updatedApplication.EnrollmentOptions.ShouldContain(WorkHoursType.FullTime);
  }

  [Fact]
  public async Task UpdateComponentGroup_WhenComponentGroupIdDoesNotMatchRequestBody_ReturnsBadRequest()
  {
    var appId = Fixture.programApplicationId;
    var routeComponentGroupId = Guid.NewGuid().ToString();
    var differentBodyId = Guid.NewGuid().ToString();
    var request = new ComponentGroupWithComponents(differentBodyId, "Group", null, "Draft", "Category", 1, Array.Empty<ProgramApplicationComponent>());

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(request).ToUrl($"/api/programApplications/{appId}/componentGroups/{routeComponentGroupId}");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task UpdateComponentGroup_WhenApplicationDoesNotExist_ReturnsNotFound()
  {
    var nonExistentAppId = Guid.NewGuid().ToString();
    var componentGroupId = Guid.NewGuid().ToString();
    var request = new ComponentGroupWithComponents(componentGroupId, "Group", null, "Draft", "Category", 1, Array.Empty<ProgramApplicationComponent>());

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(request).ToUrl($"/api/programApplications/{nonExistentAppId}/componentGroups/{componentGroupId}");
      _.StatusCodeShouldBe(HttpStatusCode.NotFound);
    });
  }

  [Fact]
  public async Task UpdateComponentGroup_WithInvalidApplicationIdFormat_ReturnsBadRequest()
  {
    var componentGroupId = Guid.NewGuid().ToString();
    var request = new ComponentGroupWithComponents(componentGroupId, "Group", null, "Draft", "Category", 1, Array.Empty<ProgramApplicationComponent>());

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(request).ToUrl($"/api/programApplications/not-a-valid-guid/componentGroups/{componentGroupId}");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task UpdateComponentGroup_WithInvalidComponentGroupIdFormat_ReturnsBadRequest()
  {
    var appId = Fixture.programApplicationId;
    var request = new ComponentGroupWithComponents("not-a-valid-guid", "Group", null, "Draft", "Category", 1, Array.Empty<ProgramApplicationComponent>());

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Put.Json(request).ToUrl($"/api/programApplications/{appId}/componentGroups/not-a-valid-guid");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task GetProgramApplications_ByCampusId_ReturnsOnlyApplicationsForThatCampus()
  {
    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications?campusId={Fixture.CampusId}");
      _.StatusCodeShouldBeOk();
    });

    var result = await response.ReadAsJsonAsync<GetProgramApplicationResponse>();
    result.ShouldNotBeNull();
    result.Applications.ShouldNotBeNull();
    result.Applications.ShouldContain(a => a.Id == Fixture.campusProgramApplicationId);
    result.Applications.ShouldNotContain(a => a.Id == Fixture.programApplicationId);
  }

  /// <remarks>
  /// PRE-REQUISITES:
  /// 1. Must be connected to the BC Government VPN.
  /// 2. Local ClamAV (clamd) must be running for file scanning middleware.
  /// </remarks>
  [Fact]
  public async Task UploadFileToComponent_FileAppearsInComponentFiles_ThenDelete_RemovesFile()
  {
    var appId = Fixture.componentTestProgramApplicationId;
    var groupId = Fixture.componentTestComponentGroupId;
    var componentId = Fixture.componentTestComponentId;
    var testFileId = Guid.NewGuid().ToString();

    var testFile = await faker.GenerateTestFile(1041);
    using var content = new StreamContent(testFile.Content);
    content.Headers.ContentType = new MediaTypeHeaderValue(testFile.ContentType);

    using var formData = new MultipartFormDataContent
    {
      { content, "file", testFile.FileName }
    };

    // Upload file directly to the component via the immediate upload endpoint
    var uploadResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.MultipartFormData(formData).ToUrl($"/api/programApplications/{appId}/componentGroups/{groupId}/components/{componentId}/files/{testFileId}");
      _.StatusCodeShouldBeOk();
    });

    var uploadedFile = (await uploadResponse.ReadAsJsonAsync<ApplicationFileInfo>()).ShouldNotBeNull();
    uploadedFile.ShareDocumentUrlId.ShouldNotBeNullOrEmpty();

    // Verify file appears in the component's file list
    var getResponse = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{appId}/componentGroups/{groupId}");
      _.StatusCodeShouldBeOk();
    });

    var componentGroup = (await getResponse.ReadAsJsonAsync<IEnumerable<ComponentGroupWithComponents>>()).ShouldNotBeNull().FirstOrDefault().ShouldNotBeNull();
    componentGroup.Components.Any(c => c.Files!.Any(f => f.Name == testFile.FileName)).ShouldBeTrue();

    // Delete the file via the immediate delete endpoint using the share document URL ID
    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Delete.Url($"/api/programApplications/{appId}/files/{uploadedFile.ShareDocumentUrlId}");
      _.StatusCodeShouldBeOk();
    });

    // Verify file is no longer in the component's file list
    var getResponseAfterDelete = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Get.Url($"/api/programApplications/{appId}/componentGroups/{groupId}");
      _.StatusCodeShouldBeOk();
    });

    var componentGroupAfterDelete = (await getResponseAfterDelete.ReadAsJsonAsync<IEnumerable<ComponentGroupWithComponents>>()).ShouldNotBeNull().FirstOrDefault().ShouldNotBeNull();
    componentGroupAfterDelete.Components.FirstOrDefault().ShouldNotBeNull().Files.ShouldBeEmpty();
  }

  [Fact]
  public async Task SubmitProgramApplication_WithInvalidGuidFormat_ReturnsBadRequest()
  {
    var request = new SubmitProgramApplicationRequest { Declaration = true };

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl("/api/programApplications/not-a-valid-guid/submit");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task SubmitProgramApplication_WhenApplicationDoesNotExist_ReturnsNotFound()
  {
    var nonExistentId = Guid.NewGuid().ToString();
    var request = new SubmitProgramApplicationRequest { Declaration = true };

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl($"/api/programApplications/{nonExistentId}/submit");
      _.StatusCodeShouldBe(HttpStatusCode.NotFound);
    });
  }

  [Fact]
  public async Task SubmitProgramApplication_WhenApplicationNotInDraftStatus_ReturnsBadRequest()
  {
    var request = new SubmitProgramApplicationRequest { Declaration = true };

    await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl($"/api/programApplications/{Fixture.programApplicationId}/submit");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });
  }

  [Fact]
  public async Task SubmitProgramApplication_WithIncompleteComponentGroup_ReturnsBadRequestWithValidationErrors()
  {
    var request = new SubmitProgramApplicationRequest { Declaration = true };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl($"/api/programApplications/{Fixture.componentTestProgramApplicationId}/submit");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });

    var error = await response.ReadAsJsonAsync<SubmitProgramApplicationValidationError>();
    error.ShouldNotBeNull();
    error.ValidationErrors.ShouldNotBeEmpty();
    error.Error.ShouldBe("ValidationFailed");
  }

  [Fact]
  public async Task SubmitProgramApplication_WithDeclarationNotAccepted_ReturnsBadRequestWithDeclarationValidationError()
  {
    var request = new SubmitProgramApplicationRequest { Declaration = false };

    var response = await Host.Scenario(_ =>
    {
      _.WithPspUser(Fixture.AuthenticatedPspUserIdentity, Fixture.AuthenticatedPspUserId);
      _.Post.Json(request).ToUrl($"/api/programApplications/{Fixture.componentTestProgramApplicationId}/submit");
      _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
    });

    var error = await response.ReadAsJsonAsync<SubmitProgramApplicationValidationError>();
    error.ShouldNotBeNull();
    error.ValidationErrors.ShouldNotBeEmpty();
    error.ValidationErrors.ShouldContain(e => e.Contains("Declaration must be accepted to submit the program application"));
  }
}
