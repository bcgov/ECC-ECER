using AutoMapper;
using ECER.Managers.Admin.Contract.Certifications;
using ECER.Managers.Admin.Contract.Files;
using ECER.Resources.Documents.Certifications;
using ECER.Utilities.ObjectStorage.Providers;
using ECER.Utilities.ObjectStorage.Providers.S3;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace ECER.Managers.Admin;

public class CertificationHandlers(IObjectStorageProviderResolver objectStorageProviderResolver, IConfiguration configuration, ICertificationRepository certificationRepository,
    IMapper mapper)
  : IRequestHandler<GetCertificationsCommand, GetCertificationsCommandResponse>,
    IRequestHandler<GetCertificationFileCommand, FileQueryResults>

{
  public async Task<GetCertificationsCommandResponse> Handle(GetCertificationsCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var certifications = await certificationRepository.QueryCertificateSummary(new UserCertificationSummaryQuery() { ById = request.Id });
    return new GetCertificationsCommandResponse(mapper.Map<IEnumerable<ECER.Managers.Admin.Contract.Certifications.CertificationSummary>>(certifications)!);
  }

  public async Task<FileQueryResults> Handle(GetCertificationFileCommand request, CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(request);

    var certifications = await certificationRepository.QueryCertificateSummary(new UserCertificationSummaryQuery() { ById = request.Id });
    var mappedCertifications = mapper.Map<IEnumerable<ECER.Managers.Admin.Contract.Certifications.CertificationSummary>>(certifications)!;

    var fileLocations = new List<FileLocation>();
    foreach (var certification in mappedCertifications)
    {
      fileLocations.Add(new FileLocation(certification!.FileId!, certification.FilePath ?? string.Empty));
    }
    //TODO change this away from always PSP we should pass in the application type
    var objectStorageProvider = objectStorageProviderResolver.resolve(EcerWebApplicationType.Psp);

    var bucket = GetBucketName(configuration, "psp");
    var files = new ConcurrentBag<FileData>();
    await Parallel.ForEachAsync(fileLocations, cancellationToken, async (fileLocation, ct) =>
    {
      var file = await objectStorageProvider.GetAsync(new S3Descriptor(bucket, fileLocation.Id, fileLocation.Folder), ct);
      var classification = file?.Tags?.SingleOrDefault(t => t.Key == "classification");
      var fileProperties = new FileProperties
      {
        Classification = classification?.Value ?? string.Empty,
        TagsList = file?.Tags?.Where(t => t.Key != "classification")
      };

      if (file != null) files.Add(new FileData(fileLocation, fileProperties, file.FileName, "application/json", file.Content));
    });

    return new FileQueryResults(files.ToList());
  }

  private static string GetBucketName(IConfiguration configuration, string key) =>
  configuration.GetValue<string>($"objectStorage:{key}:bucketName") ?? throw new InvalidOperationException($"objectStorage:{key}:bucketName is not set");
}
