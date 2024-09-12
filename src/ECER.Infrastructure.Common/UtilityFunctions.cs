using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Infrastructure.Common;

public static class UtilityFunctions
{
  public static string HumanFileSize(long bytes, int decimals = 2)
  {
    if (bytes == 0) return "0 Bytes";

    int k = 1024;
    int dm = decimals < 0 ? 0 : decimals;
    string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
    int i = (int)Math.Floor(Math.Log(bytes) / Math.Log(k));
    double size = bytes / Math.Pow(k, i);

    return size.ToString("F" + dm) + " " + sizes[i];
  }
}
