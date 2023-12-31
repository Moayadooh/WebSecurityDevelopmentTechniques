using MembersWebClient.Models;
using MembersWebClient.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Diagnostics;

namespace MembersWebClient.Controllers
{
  [Authorize]
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IProductsService productsService;
    public HomeController(ILogger<HomeController> logger, IProductsService productsService)
    {
      _logger = logger;
      this.productsService = productsService;
    }

    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public async Task<IActionResult> Index()
    {
      var products = await productsService.GetProductsAsync();
      if (products != null)
      {
        return View(products);
      }
      return RedirectToAction(nameof(Error));
    }


    public IActionResult Privacy()
    {
      return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}