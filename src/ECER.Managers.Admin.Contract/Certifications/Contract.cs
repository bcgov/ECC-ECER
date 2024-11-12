using ECER.Managers.Admin.Contract.Files;
using MediatR;

namespace ECER.Managers.Admin.Contract.Certifications;

public record GetCertificationsCommand(string? Id) : IRequest<GetCertificationsCommandResponse>;
public record GetCertificationsCommandResponse(IEnumerable<CertificationSummary> Items);
public record CertificationSummary(string Id)
{
  public string? FileName { get; set; }
  public string? FilePath { get; set; }
}
