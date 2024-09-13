using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECER.Infrastructure.Common;

public static class Utility
{
  public static string GetMiddleName(string firstName, string givenNames)
  {
    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(givenNames))
    {
      return string.Empty;
    }

    // Remove the first name from `GIVEN_NAMES` to get the middle name(s)
    string remainingNames = givenNames.Substring(firstName.Length).Trim();

    // Return the middle name(s) or empty if there's no middle name
    return remainingNames.Length > 0 ? remainingNames : string.Empty;
  }
}
