using ECER.Infrastructure.Common;
using ECER.Resources.Accounts.Communications;
using ECER.Resources.Accounts.Documents;
using ECER.Resources.Accounts.Registrants;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Resources.Accounts;

public class Configurer : IConfigureComponents
{
  public void Configure([NotNull] ConfigurationContext configurationContext)
  {
    configurationContext.Services.AddTransient<IRegistrantRepository, RegistrantRepository>();
    configurationContext.Services.AddTransient<ICommunicationRepository, CommunicationRepository>();
    configurationContext.Services.AddTransient<IDocumentRepository, DocumentRepository>();
  }
}
