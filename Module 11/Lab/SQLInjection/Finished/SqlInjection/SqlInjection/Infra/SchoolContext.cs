using Microsoft.EntityFrameworkCore;
using SqlInjection.Entities;

namespace SqlInjection.Infra
{
  public class SchoolContext : DbContext
  {
    public SchoolContext(DbContextOptions<SchoolContext> options) : base(options)
    { }

    public DbSet<Course> Courses { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new CoursesConfiguration());
      modelBuilder.ApplyConfiguration(new StudentConfiguration());
    }

  }
}
