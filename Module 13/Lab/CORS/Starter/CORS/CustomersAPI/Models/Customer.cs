namespace BestPractices.Models;

public class Customer
{
  public int Id { get; set; }
  
  public string FirstName { get; set; } = default!;

  public string? LastName { get; set; }

  public Address Address { get; set; } = default!;

  public string Email { get; set; } = default!;
}
