#pragma warning disable CA2000 // test doubles live for the duration of the test
using ECER.Managers.Registry;
using ECER.Managers.Registry.Contract.Captcha;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using System.Net;
using System.Text;

namespace ECER.Tests.Unit.RegistryPortal;

public class CaptchaHandlersTests
{
  private sealed class StubHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) : HttpMessageHandler
  {
    public string? RequestBody { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      RequestBody = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
      return await respond(request);
    }
  }

  private static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK) =>
    new(status) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

  private static CaptchaHandlers CreateHandler(StubHandler stub, params string[] hostnames)
  {
    var factory = new Mock<IHttpClientFactory>();
    factory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient(stub));
    var settings = new CaptchaAppSettings { Url = "https://captcha.test/siteverify", Secret = "secret" };
    foreach (var hostname in hostnames) settings.ExpectedHostnames.Add(hostname);
    return new CaptchaHandlers(factory.Object, Options.Create(settings), NullLogger<CaptchaHandlers>.Instance);
  }

  [Fact]
  public async Task Handle_SendsRemoteIpAndAcceptsMatchingHostnameAndAction()
  {
    var stub = new StubHandler(_ => Task.FromResult(Json("""{"success":true,"hostname":"ecer.test","action":"reference-submission"}""")));
    var handler = CreateHandler(stub, "ecer.test");

    var result = await handler.Handle(new VerifyCaptchaCommand("token", "203.0.113.5", "reference-submission"), CancellationToken.None);

    result.Success.ShouldBeTrue();
    stub.RequestBody!.ShouldContain("remoteip=203.0.113.5");
  }

  [Fact]
  public async Task Handle_RejectsUnexpectedHostname()
  {
    var stub = new StubHandler(_ => Task.FromResult(Json("""{"success":true,"hostname":"evil.test","action":"a"}""")));
    var result = await CreateHandler(stub, "ecer.test").Handle(new VerifyCaptchaCommand("token", null, "a"), CancellationToken.None);

    result.Success.ShouldBeFalse();
    result.ErrorCodes.ShouldContain(CaptchaHandlers.HostnameMismatchErrorCode);
  }

  [Fact]
  public async Task Handle_RejectsUnexpectedAction()
  {
    var stub = new StubHandler(_ => Task.FromResult(Json("""{"success":true,"hostname":"ecer.test","action":"other"}""")));
    var result = await CreateHandler(stub, "ecer.test").Handle(new VerifyCaptchaCommand("token", null, "a"), CancellationToken.None);

    result.Success.ShouldBeFalse();
    result.ErrorCodes.ShouldContain(CaptchaHandlers.ActionMismatchErrorCode);
  }

  [Fact]
  public async Task Handle_FailsClosedWhenServiceReturnsNonSuccess()
  {
    var stub = new StubHandler(_ => Task.FromResult(Json("{}", HttpStatusCode.InternalServerError)));
    var result = await CreateHandler(stub).Handle(new VerifyCaptchaCommand("token"), CancellationToken.None);

    result.Success.ShouldBeFalse();
    result.ErrorCodes.ShouldContain(CaptchaHandlers.ServiceUnavailableErrorCode);
  }

  [Fact]
  public async Task Handle_FailsClosedWhenServiceUnreachable()
  {
    var stub = new StubHandler(_ => throw new HttpRequestException("unreachable"));
    var result = await CreateHandler(stub).Handle(new VerifyCaptchaCommand("token"), CancellationToken.None);

    result.Success.ShouldBeFalse();
    result.ErrorCodes.ShouldContain(CaptchaHandlers.ServiceUnavailableErrorCode);
  }
}
