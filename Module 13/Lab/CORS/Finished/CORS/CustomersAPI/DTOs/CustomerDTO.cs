using System.ComponentModel.DataAnnotations;

namespace BestPractices.DTOs;
public class CustomerDTO : IValidatableObject
{
  public int Id { get; init; }

  [MinLength(1)]
  [MaxLength(50)]
  public string FirstName { get; init; } = default!;

  [MaxLength(50)]
  public string? LastName { get; init; }

  [MinLength(1)]
  [MaxLength(100)]
  public string Street { get; init; } = default!;

  [MinLength(1)]
  [MaxLength(100)]
  public string City { get; init; } = default!;

  [MaxLength(125)]
  [RegularExpression(RegularExpressions.EmailRegex)]
  public string Email { get; init; } = default!;

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (FirstName == LastName)
    {
      yield return new ValidationResult("First name and last name must be different", new[] { nameof(FirstName), nameof(LastName) });
    }
  }
}
