using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
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
       cfg.EnableEnumMappingValidation();
     }, loggerFactory: null);  // v15 requires this overload

    mapperConfig.AssertConfigurationIsValid();
  }
}
