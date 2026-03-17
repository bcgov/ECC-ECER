using AutoMapper;

namespace ECER.Infrastructure.Common;

public abstract class SecureProfile : Profile
{
  protected new IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
  {
    var mapper = base.CreateMap<TSource, TDestination>()
      .MaxDepth(32); 
    if (!typeof(TSource).IsValueType && !typeof(TDestination).IsValueType)
    {
      mapper = mapper.PreserveReferences();
    }
    return mapper;
  }
}
