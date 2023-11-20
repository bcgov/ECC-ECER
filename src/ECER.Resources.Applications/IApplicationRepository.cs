
namespace ECER.Resources.Applications;

public interface IApplicationRepository
{
    Task<ApplicationQueryResponse> Query(ApplicationQueryRequest query);
    Task<string> Save(SaveApplicationRequest request);
}