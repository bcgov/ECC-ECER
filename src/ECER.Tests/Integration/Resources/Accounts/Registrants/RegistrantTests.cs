using Shouldly;
using Xunit.Categories;

namespace ECER.Tests.Integration.Resources.Accounts.Registrants;

public class RegistrantTests

{
    [Fact]
    [IntegrationTest]
    public async Task CanQuery()
    {
        await Task.CompletedTask;
        true.ShouldBeTrue();
    }
}