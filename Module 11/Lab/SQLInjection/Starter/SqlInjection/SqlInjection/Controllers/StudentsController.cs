using Microsoft.AspNetCore.Authorization;
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
  [Authorize]
  public class StudentsController : Controller
  {
    private SchoolContext context;

    public StudentsController(SchoolContext context)
    {
      this.context = context;
    }

    // GET: api/values
    [HttpGet]
    public IEnumerable<Student> Get()
    {
      return context.Students.ToList();
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public Student Get(int id)
    {
      return context.Students.Find(id);
    }

  }
}
