using ECER.Managers.Admin.Contract.Files;
using MediatR;

namespace ECER.Managers.Admin.Contract.Certifications;

public record GetCertificationsCommand(string? Id) : IRequest<GetCertificationsCommandResponse>;
public record GetCertificationFileCommand(string? Id) : IRequest<FileQueryResults>;
public record GetCertificationsCommandResponse(IEnumerable<CertificationSummary> Items);
public record CertificationSummary(string Id)
{
  public string? FileName { get; set; }
  public string? FilePath { get; set; }
  public string? FileExtention { get; set; }
  public string? FileId { get; set; }
  public DateTime? CreatedOn { get; set; }
}
