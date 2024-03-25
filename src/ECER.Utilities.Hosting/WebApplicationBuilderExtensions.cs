using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ECER.Utilities.Hosting;

public static class WebApplicationBuilderExtensions
{
  public static void ConfigureDataProtection(this WebApplicationBuilder builder)
  {
    var dpbuilder = builder.Services.AddDataProtection();
    var dpFolder = builder.Configuration.GetValue("DATAPROTECTION_FOLDER", "/dpkeys");
    if (!string.IsNullOrEmpty(dpFolder) && Directory.Exists(dpFolder))
    {
      dpbuilder.PersistKeysToFileSystem(new DirectoryInfo(dpFolder));
    }
    else if (builder.Environment.IsDevelopment())
    {
      dpbuilder.UseEphemeralDataProtectionProvider();
    }
    else
    {
      throw new InvalidOperationException($"DATAPROTECTION_FOLDER is not configured or folder doesn't exist: {dpFolder}");
    }
  }
}
