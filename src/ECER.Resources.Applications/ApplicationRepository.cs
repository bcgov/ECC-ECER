using System.Globalization;
using ECER.Utilities.DataverseSdk;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Applications;

internal sealed class ApplicationRepository : IApplicationRepository
{
    private readonly EcerContext context;

    public ApplicationRepository(EcerContext context)
    {
        this.context = context;
    }

    public async Task<ApplicationQueryResponse> Query(ApplicationQueryRequest query)
    {
        await Task.CompletedTask;
        var applications = (from a in context.ECER_ApplicationSet
                            join c in context.ContactSet on a.ECER_ApplicantId.Id equals c.ContactId
                            where a.StateCode == ECER_Application_StateCode.Active && a.ECER_ApplicantId != null
                            select a).ToList();

        return new ApplicationQueryResponse(applications.Select(a => new Application
        {
            ApplicantId = a.ECER_ApplicantId.Id.ToString(),
            ApplicationId = a.Id.ToString(),
            SubmissionDate = a.CreatedOn!.Value
        }));
    }

    public async Task<string> Save(SaveApplicationRequest request)
    {
        await Task.CompletedTask;

        var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(request.Application.ApplicantId));
        if (applicant == null) throw new InvalidOperationException($"Applicant '{request.Application.ApplicantId}' not found");

        var ecerApplication = new ECER_Application(Guid.NewGuid())
        {
            ECER_LegalFirstName = "First",
            ECER_LegalLastName = "Last",
            ECER_DateOfBirth = DateTime.Parse("2000-01-01", CultureInfo.InvariantCulture),
        };

        context.AddObject(ecerApplication);

        context.AddLink(ecerApplication, ECER_Application.Fields.ECER_Application_ApplicantId_Contact, applicant);

        context.SaveChanges();

        return ecerApplication.Id.ToString();
    }
}

public record ApplicationQueryRequest();

public record ApplicationQueryResponse(IEnumerable<Application> Items);

public record Application
{
    public string ApplicantId { get; set; } = null!;
    public string? ApplicationId { get; set; }
    public DateTime SubmissionDate { get; set; }
}

public record SaveApplicationRequest(Application Application);
