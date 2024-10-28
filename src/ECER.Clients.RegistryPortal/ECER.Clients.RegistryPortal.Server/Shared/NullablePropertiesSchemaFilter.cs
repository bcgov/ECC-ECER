using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace ECER.Clients.RegistryPortal.Server.Shared;

public class NullablePropertiesSchemaFilter : ISchemaFilter
{
  public void Apply(OpenApiSchema schema, SchemaFilterContext context)
  {
    if (schema?.Properties == null)
      return;

    var constructor = context?.Type.GetConstructors().FirstOrDefault();
    if (constructor != null)
    {
      foreach (var param in constructor.GetParameters())
      {
        bool isNullableParam = Nullable.GetUnderlyingType(param.ParameterType) != null ||
                               (param.ParameterType.IsClass && !IsNonNullableReferenceType(param));

        if (!isNullableParam && !schema.Required.Contains(param.Name))
        {
          schema.Required.Add(param.Name);
        }
      }
    }

    foreach (var property in schema.Properties)
    {
      var clrProperty = context?.Type.GetProperty(property.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
      if (clrProperty != null)
      {
        bool isNullableProp = Nullable.GetUnderlyingType(clrProperty.PropertyType) != null
                              || (clrProperty.PropertyType.IsClass && !IsNonNullableReferenceType(clrProperty));

        if (!isNullableProp && !schema.Required.Contains(property.Key))
        {
          schema.Required.Add(property.Key);
        }
      }
    }
  }

  private bool IsNonNullableReferenceType(PropertyInfo propertyInfo)
  {
    var nullableAttribute = propertyInfo.GetCustomAttribute(typeof(System.Runtime.CompilerServices.NullableAttribute));
    return nullableAttribute == null && propertyInfo.PropertyType == typeof(string);
  }

  private bool IsNonNullableReferenceType(ParameterInfo parameterInfo)
  {
    var nullableAttribute = parameterInfo.GetCustomAttribute(typeof(System.Runtime.CompilerServices.NullableAttribute));
    return nullableAttribute == null && parameterInfo.ParameterType == typeof(string);
  }
}
