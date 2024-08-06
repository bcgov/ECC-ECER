using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Utilities.FileScanner.Providers;

internal record ClamAvProviderSettings
{
  public string Url { get; set; } = null!;
  public int Port { get; set; }
}
