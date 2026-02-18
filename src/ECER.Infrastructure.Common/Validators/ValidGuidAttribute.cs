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
