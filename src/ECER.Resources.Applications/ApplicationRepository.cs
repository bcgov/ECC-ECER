using AutoMapper;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Applications;

internal sealed class ApplicationRepository : IApplicationRepository
{
    private readonly EcerContext context;
    private readonly IMapper mapper;

    public ApplicationRepository(EcerContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<Application>> Query(ApplicationQuery query)
    {
        return query switch
        {
            CertificationApplicationQuery qry => await Query(qry),
            _ => throw new NotImplementedException()
        };
    }

    public async Task<string> SaveDraft(Application application)
    {
        return application switch
        {
            CertificationApplication app => await Save(app),
            _ => throw new NotImplementedException()
        };
    }

    private async Task<string> Save(CertificationApplication application)
    {
        await Task.CompletedTask;

        var applicant = context.ContactSet.SingleOrDefault(c => c.ContactId == Guid.Parse(application.ApplicantId));
        if (applicant == null) throw new InvalidOperationException($"Applicant '{application.ApplicantId}' not found");

        var ecerApplication = mapper.Map<ecer_Application>(application);
        if (!ecerApplication.ecer_ApplicationId.HasValue)
        {
            ecerApplication.ecer_ApplicationId = Guid.NewGuid();
            context.AddObject(ecerApplication);
            context.AddLink(ecerApplication, ecer_Application.Fields.ecer_application_Applicantid_contact, applicant);
        }
        else
        {
            context.Attach(ecerApplication);
            context.UpdateObject(ecerApplication);
        }

        context.SaveChanges();

        return ecerApplication.ecer_ApplicationId.Value.ToString();
    }

    public Task<string> Submit(string applicationId) => throw new NotImplementedException();

    private async Task<IEnumerable<CertificationApplication>> Query(CertificationApplicationQuery query)
    {
        await Task.CompletedTask;
        var applications = from a in context.ecer_ApplicationSet
                           join c in context.ContactSet on a.ecer_Applicantid.Id equals c.ContactId
                           select new { a, c };

        if (query.ByStatus != null)
        {
            var statuses = mapper.Map<IEnumerable<ecer_Application_StatusCode>>(query.ByStatus).ToList();
            applications = applications.Where(a => statuses.Contains(a.a.StatusCode!.Value));
        }

        if (query.ById != null) applications = applications.Where(r => r.a.ecer_ApplicationId == Guid.Parse(query.ById));
        if (query.ByApplicantId != null) applications = applications.Where(r => r.a.ecer_Applicantid.Id == Guid.Parse(query.ByApplicantId));

        return mapper.Map<IEnumerable<CertificationApplication>>(applications.Select(r => r.a).ToList());
    }
}