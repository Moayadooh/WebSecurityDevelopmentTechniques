using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace MembersWebClient.Services
{
  public class ProductsService : IProductsService
  {
    private readonly HttpClient httpClient;
    private readonly ITokenAcquisition tokenAcquisition;
    private readonly IConfiguration configuration;


    public ProductsService(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IConfiguration configuration)
    {
      this.httpClient = httpClient;

      this.tokenAcquisition = tokenAcquisition;
      this.configuration = configuration;
    }
    private async Task PrepareAuthenticatedClient()
    {
      var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { configuration["DownstreamApi:Scopes"] });
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
      httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
    public async Task<List<string>> GetProductsAsync()
    {
      await PrepareAuthenticatedClient();
      var responseMessage = await
                      httpClient.GetAsync($"{configuration["DownstreamApi:BaseUrl"]}/api/products");
      if (responseMessage.IsSuccessStatusCode)
      {
        var jsonResult = await responseMessage.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<string>>(jsonResult); return products;
      }

      return null;
    }

  }

}
