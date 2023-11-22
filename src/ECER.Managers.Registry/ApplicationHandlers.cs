using ECER.Engines.Validation.Applications;
using ECER.Resources.Accounts.Registrants;
using ECER.Resources.Applications;
using Microsoft.Extensions.DependencyInjection;

namespace ECER.Managers.Registry;

/// <summary>
/// Message handlers
/// </summary>
public class ApplicationHandlers(IServiceProvider serviceProvider)
{
    /// <summary>
    /// Handles submitting a new application use case
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    public async Task<string> Handle(SubmitNewApplicationCommand cmd)
    {
        var validationEngine = serviceProvider.GetRequiredService<IApplicationValidationEngine>();
        var registrantsRepository = serviceProvider.GetRequiredService<IRegistrantRepository>();
        var applicationRepository = serviceProvider.GetRequiredService<IApplicationRepository>();

        var validationResults = await validationEngine.Validate(new Engines.Validation.Applications.Application());

        if (!validationResults.IsValid)
        {
            throw new InvalidOperationException("invalid application");
        }

        var applicantId = await registrantsRepository.Create(new NewRegistrantRequest());
        var applicationId = await applicationRepository.Save(new SaveApplicationRequest(new Resources.Applications.Application
        {
            ApplicantId = applicantId
        }));

        return applicationId;
    }

    /// <summary>
    /// Handles applications query use case
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<ApplicationsQueryResults> Handle(ApplicationsQuery query)
    {
        var applicationRepository = serviceProvider.GetRequiredService<IApplicationRepository>();
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