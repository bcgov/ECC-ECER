/*This is a custom attribute validator for our records objects
 * It checks whether the object property is a valid guid in our endpoint layer records objects
 * works with parameter validation .WithParameterValidation();
 *
 *** How to use ***
 * Create a record in the endpoint layer with the following attribute
 * public record Program
 * {
 *   [ValidGuid]
 *   public string? Id { get; set; }
 * }
 *
 * in endpoint builder use .WithParameterValidation()
 */

using System.ComponentModel.DataAnnotations;

namespace ECER.Clients.PSPPortal.Server.Programs
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
  public sealed class ValidGuidAttribute : ValidationAttribute
  {
    public ValidGuidAttribute()
        : base("The field {0} must be a valid GUID.")
    { }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
      ArgumentNullException.ThrowIfNull(validationContext);

      if (value == null) return ValidationResult.Success;

      var isGuid = Guid.TryParse(value.ToString(), out var _);
      return isGuid ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
  }
}
