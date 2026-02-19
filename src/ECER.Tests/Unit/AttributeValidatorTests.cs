using ECER.Clients.PSPPortal.Server.Programs;
using System.ComponentModel.DataAnnotations;

namespace ECER.Tests.Unit
{
  public class AttributeValidatorTests
  {
    private readonly ValidGuidAttribute _attribute = new();

    [Theory]
    [InlineData("550e8400-e29b-41d4-a716-446655440000", true)] // Standard
    [InlineData("550e8400e29b41d4a716446655440000", true)]   // No hyphens
    [InlineData(null, true)]                                  // Optional is valid
    [InlineData("", false)]                                    // Whitespace is not a valid GUID
    [InlineData(" ", false)]                                    // Whitespace is not a valid GUID
    [InlineData("not-a-guid", false)]                         // Garbage string
    [InlineData("12345", false)]                              // Too short
    public void IsValid_ShouldValidateCorrectValue(string? value, bool expectedSuccess)
    {
      // Act
      var result = _attribute.GetValidationResult(value, new ValidationContext(new object()));

      // Assert
      if (expectedSuccess)
        Assert.Equal(ValidationResult.Success, result);
      else
        Assert.NotEqual(ValidationResult.Success, result);
    }
  }
}
