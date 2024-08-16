using Microsoft.Extensions.Configuration;

namespace ECER.Infrastructure.Common;

public interface IPostConfigureChecker
{
  /// <summary>
  /// Runs startup health check; if throws exception it is considered unhealthy, if completed successfully it is considered healthy
  /// </summary>
  Task Check(CheckContext context, CancellationToken ct);
}

public record CheckContext(IServiceProvider Services, IConfiguration Configuration);
