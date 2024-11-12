using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ECER.Tests.Integration.Api;

public class CertificationTests : ApiWebAppScenarioBase
{
  public CertificationTests(ITestOutputHelper output, ApiWebAppFixture fixture) : base(output, fixture)
  {
  }
}
