using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SqlInjection.Entities;
using System;

namespace SqlInjection.Infra
{
  public class CoursesConfiguration : IEntityTypeConfiguration<Course>
  {
    public void Configure(EntityTypeBuilder<Course> builder)
    {
      builder.HasKey(course => course.CourseId);
      builder.Property(course => course.Title)
        .HasMaxLength(128);

      builder.HasMany(course => course.Students)
        .WithOne(student => student.Course)
        .HasForeignKey(student => student.CourseId);

      builder.HasData(
        new Course { CourseId = 1, Title = "Chemistry", Date = DateTime.Now },
        new Course { CourseId = 2, Title = "Calculus", Date = DateTime.Now },
        new Course { CourseId = 3, Title = "Literature", Date = DateTime.Now }
    );
    }
  }
}
