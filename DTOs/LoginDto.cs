using System.ComponentModel.DataAnnotations;
using PolicyManagement.Constants;

namespace PolicyManagement.DTOs;

public class LoginDto : IValidatableObject
{
    [Required(ErrorMessage = "Login type is required.")]
    [RegularExpression("^(EMAIL|PHONE)$", ErrorMessage = "Login type must be either 'EMAIL' or 'PHONE'.")]
    public string Type { get; set; } = LoginTypes.EMAIL;

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters.")]
    public string? Email { get; set; }

    [StringLength(10, ErrorMessage = "Country code cannot exceed 10 characters.")]
    public string? CountryCode { get; set; }

    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        var loginType = Type?.ToUpper();

        if (loginType == LoginTypes.EMAIL)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                results.Add(new ValidationResult("Email is required when login type is EMAIL.", new[] { nameof(Email) }));
            }
            else if (!new EmailAddressAttribute().IsValid(Email))
            {
                results.Add(new ValidationResult("Invalid email format.", new[] { nameof(Email) }));
            }
        }
        else if (Type == LoginTypes.PHONE)
        {
            if (string.IsNullOrWhiteSpace(CountryCode))
            {
                results.Add(new ValidationResult("Country code is required when login type is PHONE.", new[] { nameof(CountryCode) }));
            }
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                results.Add(new ValidationResult("Phone number is required when login type is PHONE.", new[] { nameof(PhoneNumber) }));
            }
        }
        else
        {
            results.Add(new ValidationResult("Invalid login type.", new[] { nameof(Type) }));
        }

        return results;
    }
}
