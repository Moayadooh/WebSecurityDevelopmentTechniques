namespace BestPractices.Models;

public class CustomersDb : DbContext
{

  public CustomersDb(DbContextOptions<CustomersDb> options)
    : base(options) { }

  public DbSet<Customer> Customers { get; set; } = default!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
    => modelBuilder.ApplyConfiguration(new CustomerConfiguration());

}
