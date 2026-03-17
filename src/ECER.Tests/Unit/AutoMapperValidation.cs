using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using AutoMapper.Internal;
using ECER.Infrastructure.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace ECER.Tests.Unit;

public class AutoMapperValidation
{
  class Circular { public Circular? Self { get; set; } }
  
  [Fact]
  public void Validate()
  {
    var loggerFactory = NullLoggerFactory.Instance;

    var mapperConfig = new MapperConfiguration(cfg =>
     {
       cfg.AddMaps(ReflectionExtensions.DiscoverLocalAessemblies(prefix: "ECER"));
       cfg.EnableEnumMappingValidation();
     });  // v15 requires this overload

    mapperConfig.AssertConfigurationIsValid();
  }
  
  [Fact]
  public void MaxDepth_IsActuallySet()
  {
    var config = new MapperConfiguration(cfg => 
      cfg.CreateMap<Circular, Circular>()
        .MaxDepth(32)          
        .PreserveReferences());

    var globalConfig = (IGlobalConfiguration)config;
    var typeMaps = globalConfig.GetAllTypeMaps();
    
    foreach (var typeMap in typeMaps)
    {
      typeMap.MaxDepth.ShouldBe(32);
    }
  }
  
  [Fact]
  public void Mapper_ShouldNotThrow_StackOverflow_WithDeepNesting()
  {
    var config = new MapperConfiguration(cfg => cfg.CreateMap<Circular, Circular>()
        .MaxDepth(32)          
        .PreserveReferences()
        );
    var mapper = config.CreateMapper();
    
    var root = new Circular();
    var current = root;

    for (int i = 0; i < 100; i++)
    {
      current.Self = new Circular();
      current = current.Self;
    }
    
    var exception = Record.Exception(() => mapper.Map<Circular>(root));
    Assert.Null(exception);
    
    var result = mapper.Map<Circular>(root);
    result.ShouldNotBeNull();
    
    int depth = 0;
    current = result;
    while (current.Self != null)
    {
      depth++;
      current = current.Self;
    }
    depth.ShouldBeLessThanOrEqualTo(32);
  }
}
