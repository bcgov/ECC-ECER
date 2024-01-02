using AutoMapper;
using ECER.Infrastructure.Common;

namespace ECER.Tests.Unit;

public class AutoMapperValidation
{
    [Fact]
    public void Validate()
    {
        var mapperConfig = new MapperConfiguration(cfg =>
         {
             cfg.AddMaps(ReflectionExtensions.DiscoverLocalAessemblies(prefix: "ECER"));
         });

        mapperConfig.AssertConfigurationIsValid();
    }
}