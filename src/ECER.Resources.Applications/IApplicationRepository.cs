namespace ECER.Resources.Applications;

public interface IApplicationRepository
{
    Task<ApplicationQueryResponse> Query(ApplicationQueryRequest query);
    Task<string> Save(SaveApplicationRequest request);
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
