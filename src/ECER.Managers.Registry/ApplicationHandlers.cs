using ECER.Resources.Applications;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public static class ApplicationHandlers
{
    /// <summary>
    /// Handles submitting a new application use case
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public static async Task<string> Handle(SubmitNewApplicationCommand cmd)
    {
        await Task.CompletedTask;
        return string.Empty;
    }

    /// <summary>
    /// Handles applications query use case
    /// </summary>
    /// <param name="query"></param>
    /// <param name="applicationRepository">DI service</param>
    /// <returns></returns>
    public static async Task<ApplicationsQueryResults> Handle(ApplicationsQuery query, IApplicationRepository applicationRepository)
    {
        ArgumentNullException.ThrowIfNull(applicationRepository);
        var queryResponse = await applicationRepository.Query(new ApplicationQueryRequest());
        return new ApplicationsQueryResults(queryResponse.Items.Select(i => new Application
        {
            Id = i.ApplicationId!,
            RegistrantId = i.ApplicantId,
            SubmittedOn = i.SubmissionDate
        }));
    }
}

/// <summary>
/// SubmitNewApplicationCommand
/// </summary>
public record SubmitNewApplicationCommand();

public record ApplicationsQuery();

public record ApplicationsQueryResults(IEnumerable<Application> Items);

public record Application()
{
    public string Id { get; set; } = null!;
    public string RegistrantId { get; set; } = null!;
    public DateTime SubmittedOn { get; set; }
}