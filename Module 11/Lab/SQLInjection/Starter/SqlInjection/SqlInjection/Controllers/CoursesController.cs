using Microsoft.AspNetCore.Mvc;
using SqlInjection.Entities;
using SqlInjection.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlInjection.Controllers
{
  [Route("api/[controller]")]
  public class CoursesController : Controller
  {
    private CoursesRepository repo;

    public CoursesController(CoursesRepository context)
    {
      this.repo = context;
    }

    // GET: api/values
    [HttpGet]
    public IEnumerable<Course> Get()
    {
      var id = Request.Query["id"].FirstOrDefault();
      if (string.IsNullOrEmpty(id))
      {
        return repo.GetAllCourses();
      }
      else
      {
        return repo.GetCourse(id);
      }
    }

  }
}
