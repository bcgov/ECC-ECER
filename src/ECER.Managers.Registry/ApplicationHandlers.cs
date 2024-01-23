using AutoMapper;
using ECER.Resources.Applications;
using ECER.Utilities.Security;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public static class ApplicationHandlers
{
    /// <summary>
    /// Handles submitting a new application use case
    /// </summary>
    /// <param name="cmd">The command</param>
    /// <param name="applicationRepository">DI service</param>
    /// <param name="mapper">DI service</param>
    /// <returns></returns>
    public static async Task<string> Handle(SaveDraftCertificationApplicationCommand cmd, IApplicationRepository applicationRepository, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(applicationRepository);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(cmd);

        var applicationId = await applicationRepository.SaveDraft(mapper.Map<Resources.Applications.CertificationApplication>(cmd.Application));
        return applicationId;
    }

    /// <summary>
    /// Handles applications query use case
    /// </summary>
    /// <param name="query">The query</param>
    /// <param name="applicationRepository">DI service</param>
    /// <param name="mapper">DI service</param>
    /// <returns></returns>
    public static async Task<ApplicationsQueryResults> Handle(CertificationApplicationsQuery query, IApplicationRepository applicationRepository, IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(applicationRepository);
        ArgumentNullException.ThrowIfNull(mapper);
        ArgumentNullException.ThrowIfNull(query);

        var applications = await applicationRepository.Query(new CertificationApplicationQuery
        {
            ById = query.ById
        });
        return new ApplicationsQueryResults(mapper.Map<IEnumerable<CertificationApplication>>(applications));
    }
}

/// <summary>
/// Saves an application in draft state
/// </summary>
public record SaveDraftCertificationApplicationCommand(CertificationApplication Application);

public record SubmitCertificationApplicationCommand(string applicationId);

public record CertificationApplicationsQuery
{
    public string? ById { get; set; }
    public UserIdentity? ByIdentity { get; set; }
}

public record ApplicationsQueryResults(IEnumerable<CertificationApplication> Items);

public record CertificationApplication
{
    public string? Id { get; set; }
    public string RegistrantId { get; set; } = null!;
    public DateTime SubmittedOn { get; set; }
    public IEnumerable<CertificationType> CertificationTypes { get; set; } = Array.Empty<CertificationType>();
}

public enum CertificationType
{
    OneYear,
    FiveYears
}