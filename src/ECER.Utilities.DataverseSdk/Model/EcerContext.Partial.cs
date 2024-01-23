using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using XrmToolkit.Linq;

namespace ECER.Utilities.DataverseSdk.Model;

public partial class EcerContext
{
    private IOrganizationServiceAsync organizationService;

    public EcerContext(IOrganizationServiceAsync service) : this((IOrganizationService)service)
    {
        this.organizationService = service;
    }

    public IQueryable<TEntity> Queryable<TEntity>() where TEntity : Entity => organizationService.Queryable<TEntity>();
}