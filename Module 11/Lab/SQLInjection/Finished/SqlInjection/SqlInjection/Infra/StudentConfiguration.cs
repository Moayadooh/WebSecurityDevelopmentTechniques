using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlInjection.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInjection.Infra
{
  public class StudentConfiguration : IEntityTypeConfiguration<Student>
  {
    public void Configure(EntityTypeBuilder<Student> builder)
    {
      builder.HasKey(student => student.StudentId);

      builder.Property(student => student.FirstName)
        .IsRequired()
        .HasMaxLength(128);

      builder.Property(student => student.LastName)
        .IsRequired()
        .HasMaxLength(128);

      builder.HasData(
        new Student { StudentId = 1, FirstName = "Carson", LastName = "Alexander", CourseId = 1 },
        new Student { StudentId = 2, FirstName = "Meredith", LastName = "Alonso", CourseId = 1 },
        new Student { StudentId = 3, FirstName = "Arturo", LastName = "Anand", CourseId = 2 },
        new Student { StudentId = 4, FirstName = "Gytis", LastName = "Barzdukas", CourseId = 2 },
        new Student { StudentId = 5, FirstName = "Yan", LastName = "Li", CourseId = 2 },
        new Student { StudentId = 6, FirstName = "Peggy", LastName = "Justice", CourseId = 3 },
        new Student { StudentId = 7, FirstName = "Laura", LastName = "Norman", CourseId = 3 },
        new Student { StudentId = 8, FirstName = "Nino", LastName = "Olivetto", CourseId = 3 }
      );
    }
  }
}
