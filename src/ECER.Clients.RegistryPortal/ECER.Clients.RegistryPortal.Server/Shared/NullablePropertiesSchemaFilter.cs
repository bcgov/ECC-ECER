using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;

namespace ECER.Clients.RegistryPortal.Server.Shared
{
  public class NullablePropertiesSchemaFilter : ISchemaFilter
  {
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
      if (schema?.Properties == null)
        return;

      foreach (var property in schema.Properties)
      {
        var clrProperty = context?.Type.GetProperty(property.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (clrProperty != null)
        {
          // Determine if the property is nullable
          var isNullable = Nullable.GetUnderlyingType(clrProperty.PropertyType) != null
                           || (clrProperty.PropertyType.IsClass && !IsNonNullableReferenceType(clrProperty));

          // If the property is not nullable, add it to the required list
          if (!isNullable && !schema.Required.Contains(property.Key))
          {
            schema.Required.Add(property.Key);
          }
        }
      }
    }

    private bool IsNonNullableReferenceType(PropertyInfo propertyInfo)
    {
      // Check if the property has a Nullable attribute or is explicitly marked as nullable
      var nullableAttribute = propertyInfo.GetCustomAttribute(typeof(System.Runtime.CompilerServices.NullableAttribute));
      return nullableAttribute == null && propertyInfo.PropertyType == typeof(string);
    }
  }
}
