using Microsoft.Extensions.Configuration;

namespace ECER.Infrastructure.Common;

public interface IPostConfigureChecker
{
    Task<bool> Check(CheckContext context, CancellationToken ct);
}

public record CheckContext(IServiceProvider Services, IConfiguration Configuration);