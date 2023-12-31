using Microsoft.Data.SqlClient;
using SqlInjection.Entities;
using SqlInjection.Models;
using System.Collections.Generic;

namespace SqlInjection.Infra
{
  public class CoursesRepository
  {
    private ADOConfig config;

    public CoursesRepository(ADOConfig config)
    {
      this.config = config;
    }

    public List<Course> GetAllCourses()
    {
      var result = new List<Course>();
      using (var conn = new SqlConnection(config.ConnectionString))
      {
        conn.Open();

        var command = new SqlCommand() { Connection = conn };
        command.CommandText = $@"SELECT CourseId, Title, Date FROM Courses";
        using (var cmdReader = command.ExecuteReader())
        {
          while (cmdReader.Read())
          {
            var course = new Course();
            course.CourseId = cmdReader.GetInt32(cmdReader.GetOrdinal("CourseId"));
            course.Title = cmdReader.GetString(cmdReader.GetOrdinal("Title"));
            course.Date = cmdReader.GetDateTime(cmdReader.GetOrdinal("Date"));
            result.Add(course);
          }
        }
      }
      return result;
    }

    public List<Course> GetCourse(string id)
    {
      var result = new List<Course>();
      using (var conn = new SqlConnection(config.ConnectionString))
      {
        conn.Open();

        var command = new SqlCommand() { Connection = conn };
        command.CommandText = $@"SELECT CourseId, Title, Date
                                FROM Courses WHERE CourseId = {id}";
        using (var cmdReader = command.ExecuteReader())
        {
          while (cmdReader.Read())
          {
            var course = new Course();
            course.CourseId = cmdReader.GetInt32(cmdReader.GetOrdinal("CourseId"));
            course.Title = cmdReader.GetString(cmdReader.GetOrdinal("Title"));
            course.Date = cmdReader.GetDateTime(cmdReader.GetOrdinal("Date"));
            result.Add(course);
          }
        }
      }
      return result;
    }
  }
}
