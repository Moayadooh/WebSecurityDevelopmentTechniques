using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MembersAPI.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    // GET: api/<ProductsController>
    [HttpGet]
    [Authorize]
    [RequiredScope("Products.List")]
    public IEnumerable<string> Get()
    {
      return new string[] { "value1", "value2" };
    }

    // GET api/<ProductsController>/5
    [HttpGet("{id}")]
    [Authorize]
    [RequiredScope("Products.List")]
    public string Get(int id)
    {
      return "value";
    }

    // POST api/<ProductsController>
    [HttpPost]
    [Authorize]
    [RequiredScope("Products.Add")]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<ProductsController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<ProductsController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
