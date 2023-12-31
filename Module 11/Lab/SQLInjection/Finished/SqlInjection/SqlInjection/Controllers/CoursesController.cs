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
      return repo.GetAllCourses();
    }

    [HttpGet("{id:int}")]
    public Course Get(int id)
    {
      return repo.GetCourse(id);
    }
  }
}
