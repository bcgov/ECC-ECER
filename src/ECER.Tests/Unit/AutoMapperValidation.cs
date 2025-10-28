using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace ECER.Tests.Unit;

public class AutoMapperValidation
{
  [Fact]
  public void Validate()
  {
    var loggerFactory = NullLoggerFactory.Instance;

    var mapperConfig = new MapperConfiguration(cfg =>
     {
       cfg.AddMaps(ReflectionExtensions.DiscoverLocalAessemblies(prefix: "ECER"));
       cfg.EnableEnumMappingValidation();
     }, loggerFactory: loggerFactory);  // v15 requires this overload

    mapperConfig.AssertConfigurationIsValid();
  }
}
