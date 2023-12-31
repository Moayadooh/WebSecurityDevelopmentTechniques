using System;
using System.Collections.Generic;

namespace SqlInjection.Entities
{
  public class Course
  {
    public int CourseId { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public List<Student> Students { get; set; }
  }
}
