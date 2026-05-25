using Microsoft.Extensions.DependencyInjection;

namespace ECER.Utilities.ObjectStorage.Providers;

public interface IObjectStorageProviderResolver
{
  IObjecStorageProvider resolve(EcerWebApplicationType ecerWebApplicationType);
}

public class ObjectStorageProviderResolver : IObjectStorageProviderResolver
{
  private readonly IServiceProvider _serviceProvider;

  public ObjectStorageProviderResolver(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public IObjecStorageProvider resolve(EcerWebApplicationType ecerWebApplicationType)
  {
    return ecerWebApplicationType switch
    {
      EcerWebApplicationType.PSP => _serviceProvider.GetRequiredKeyedService<IObjecStorageProvider>(EcerWebApplicationType.PSP),
      EcerWebApplicationType.Registry => _serviceProvider.GetRequiredKeyedService<IObjecStorageProvider>(EcerWebApplicationType.Registry),
      _ => throw new ArgumentOutOfRangeException(nameof(ecerWebApplicationType), ecerWebApplicationType, null)
    };
  }
}
