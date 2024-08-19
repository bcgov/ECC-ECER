using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace ECER.Infrastructure.Common;

/// <summary>
/// Implementers of this interface declare they have configuration to run when a deployment unit starts
/// </summary>
public interface IConfigureComponents
{
  /// <summary>
  /// Configure components in a particular context
  /// </summary>
  /// <param name="configurationContext"></param>
  void Configure([NotNull] ConfigurationContext configurationContext);
}

/// <summary>
/// A configuration context
/// </summary>
/// <param name="Services">Service collection</param>
/// <param name="Configuration">Configuration</param>
/// <param name="logger">Logger</param>
public record ConfigurationContext(IServiceCollection Services, IConfiguration Configuration, ILogger logger);
