using System.ComponentModel.DataAnnotations.Schema;

namespace SqlInjection.Entities
{
  public class Student
  {
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; }
  }
}
