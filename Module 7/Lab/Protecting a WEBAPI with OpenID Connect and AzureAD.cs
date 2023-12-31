/*Introduction
In this exercise you will protect a WebAPI using OAuth, and consume this API from another application.

Today there are a lot of REST Api’s out there that are protected using OAuth. With the proper token you can have these services post messages on walls, send mails, create appointments, etc… So how does this work?

In our case we will use server-to-server authentication, and configure permissions the Web API requires, and configure the permissions the Client can use.*/

//Requirements
//This lab uses .NET 6. To check if you have the right version, open a command prompt and type

dotnet –-version
/*So should see at least version 6.0.xxx.

You will also need Visual Studio 2022 (or newer). Some of the wizards in Visual Studio require an Azure account, so it is best you add your Azure account at this point. Open Visual Studio, and go to File -> Account Settings…

Figure 1

Click on Add an account… to add your Azure account.

Getting started
Start by opening Visual Studio and creating a new ASP.NET Core Web Api project, calling it MembersAPI and the solution ProtectingAWebAPI:


Figure 2
Figure 3

In the next step select .NET 6, and change the authentication type to Microsoft identity platform.

Figure 4

Click on Create.

Once your application is scaffolded, you will see the following window pop up:

Figure 5

Click on finish to automatically install the required dependencies.

In the connected services panel in Visual Studio, configure Microsoft Identity Platform.

Figure 6


Click on the '+'-icon to create a new App Registration.

Figure 7

Name it MembersAPI.


Select the newly created App Registration and click Finish.

Figure 8

Implementing the API
We will now quickly build a simple Web API controller.

Right-click the Controllers folder and select Add -> Controller…

Figure 9

Select API Controller with read/write actions and name it ProductsController.

Run the project, the browser should open. Change the uri to end with api/products. You should see an array of two string values.

Implementing the client
Add another web project to the solution, now calling it MembersWebClient, using the same steps (except pick Web Application (Model-View-Controller) instead of WebAPI, and name the App registration MembersWebClient).

Figure 10

We’ll add some better practices to the project. First of all we’ll add a new folder Services to the Client application. In the services folder, we add a new interface IProductsService of which we’ll add an implementation in a second.
*/
//SERVICES/IPRODUCTSSERVICE
namespace MembersWebClient.Services
{
  public interface IProductsService
  {
    Task<List<string>> GetProductsAsync();
 
  }
}
//Next we add a new ProductsService in the same folder. This will implement the corresponding interface and it needs a constructor to take an instance of HttpClient.

//SERVICES/PRODUCTSSERVICE.CS
public class ProductsService : IProductsService
{
  private readonly HttpClient httpClient;
 
  public ProductsService(HttpClient httpClient)
  {
    this.httpClient = httpClient;
  }
 
  public async Task<List<string>> GetProductsAsync()
  {
    throw new NotImplementedException();
  }
}
 

//Add a constructor to HomeController to take an instance of IProductsService:

//HOMECONTROLLER.CS
private readonly IProductsService productsService;
 
 
public HomeController(ILogger<HomeController> logger, IProductsService productsService)
{
  _logger = logger;
  this.productsService = productsService;
}
//Next we need to make sure Dependency Injection is correctly configured. Go to the program file and add the HttpClient for the IProductsService/ProductsService combination. This also adds the IProductsService/ProductsService combination for the HomeController.

//PROGRAM.CS
builder.Services.AddHttpClient<IProductsService, ProductsService>();
Next, register the baseUrl for your membersApi in appsettings.json

"DownstreamApi": {
  "BaseUrl": "https://localhost:7155"
}
//NOTE: make sure the port number you use matches your Web Api. You can check the port number in the membersAPI's launchSettings.json

//We need to add some code into the ProductsService to return a list of products, when the GetProductsAsync() method gets called.

//PRODUCTSSERVICE.CS
public class ProductsService : IProductsService
{
  private readonly HttpClient httpClient;
  private readonly IConfiguration configuration;
 
  public ProductsService(HttpClient httpClient, IConfiguration configuration)
  {
    this.httpClient = httpClient;
    this.configuration = configuration;
  }
 
  public async Task<List<string>> GetProductsAsync()
  {
    var responseMessage = await
                    httpClient.GetAsync($"{configuration["DownstreamApi:BaseUrl"]}/api/products");
    if (responseMessage.IsSuccessStatusCode)
    {
      var jsonResult = await responseMessage.Content.ReadAsStringAsync();
      var products = JsonSerializer.Deserialize<List<string>>(jsonResult);    
      return products;
    }
 
    return null;
  }
}
//Now let’s modify the Index action method in the HomeController to show the list of products.

//HOMECONTROLLER.CS
public async Task<IActionResult> Index()
{
  var products = await productsService.GetProductsAsync();
  if(products != null)
  {
    return View(products);
  }
  return RedirectToAction(nameof(Error));
}
//We’ll change the index view as well, to just show the products in a UL element.

//VIEWS/HOME/INDEX.CSHTML
@model List<string>
 
@{
  ViewData["Title"] = "Home Page";
}
 
<div class="row">
  <div class="col-lg-12">
    <ul>
      @foreach (var s in Model)
      {
        <li>@s</li>
      }
    </ul>
  </div>
</div>
/*In Visual Studio change the Startup Projects (right click on the Solution and select Startup Projects). In here select multiple startup projects and start both projects.

Double-check that the API controller does not have the Authorize attribute set. Put a breakpoint in this method, and run. Step through the code, you should get a list of values.

Configuring the WebAPI in microsoft entra id
Ok. Let’s provision this WebAPI in Entra ID. Go back to the Azure portal, and navigate to the MembersAPI registration.

Now click on Expose an API.

This will allow you to add scopes to your application. These scopes are the permissions that we can request from the Client Application. We need to add two scopes for the sake of this lab exercise.

Figure 11

We will add a scope to be able to list products and add products. Call them Products.List and Products.Add.

Create both scopes. The display text doesn’t really matter (for the sake of this exercise), but just make sure you select the Admins and Users option.

Figure 12

After you’ve created both scopes it’s now time to add them as a permission to the API service.

Protecting the WebAPI with permissions
Now we are ready to check if the client application actually has these permissions.

In the ProductsController class add the Authorize attribute, and the RequiredScope attribute to the Get method, pass the Products.List scope as a paremeter:
*/
//PRODUCTSCONTROLLER.CS
[HttpGet]
[Authorize] 
[RequiredScope("Products.List")]
public IEnumerable<string> Get()
{
  return new string[] { "value1", "value2" };
}
//Do this again for the other Get method:

//PRODUCTSCONTROLLER.CS
[HttpGet("{id}", Name = "Get")]
[Authorize] 
[RequiredScope("Products.List")]
public string Get(int id)
{
  return "value";
}
//And once more for the Post method:

//PRODUCTSCONTROLLER.CS
[HttpPost]
[Authorize] 
[RequiredScope("Products.Add")]
public void Post([FromBody] string value)
{
}
/*So try running the projects again. However, you will now receive an access denied, because the client does not have the correct access token for the WebAPI.

Figure 13

So our WebAPI service is protected. Now allowing the client to access it…

Getting the Access token from Entra ID.
The client application will use the OAuth authorization endpoint from Azure to get an Access token. It will then attach this access token to each request to the Web API, which will verify the token.

So how does this work again? After getting the authorization code, the application uses that code together with a client secret (which identifies the client application) to get an Access token. So we will need a client secret. Open the Azure portal, go to Microsoft Entra ID, open the client application (MembersWebClient), and then open Certificates & secrets.

Figure 14

Here you can create keys which will then be used to identify the application (not the user) to Entra ID.

Enter a name and expiration.

Figure 15

When you add the secret the UI will display a secret string which you will need to copy to your application. It will show this secret only once, so copy carefully . Should you lose this secret string, simply create a new key all over again…

Figure 16

Take this key and copy it to your client’s (MemberWebClient) appsettings.json (In real life this would be a very bad move, since you would use source control, where your secret ends up, … You would actually protect this using Azure Key Vault, or put it into an environment variable). For example, in my case:
*/
//APPSETTINGS.JSON
"AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "u2u.be ",
    "TenantId": "f091f6a9-ae72-4a6d-a3a8-cbf123fb97fa",
    "ClientId": "f3914336-0c60-46b4-b0f1-cfe9166e4c2b",
    "CallbackPath": "/signin-oidc",
    "ClientSecret": "3t3LmceZznVp-EC*---------------"
  },
//One more thing. We're going to add another section to our appsettings to store some configuration for the MembersAPI.

//Add both scope URIs (can be found in the app registration for your service).

//APPSETTINGS.JSON
"DownStreamApi": {
  "Scopes": "api://xxx/Products.List api://xxx/Products.Create",
  "BaseUrl": "https://localhost:7155"
},

//Finishing up the client application
//In the program class of the client application, change the authentication service so that it also requests permissions to call our members API, and caches access tokens in memory. You can do this using the

//EnableTokenAcquisitionToCallDownstreamApi and AddInMemoryTokenCaches methods.

//PROGRAM.CS
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
  .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
  .EnableTokenAcquisitionToCallDownstreamApi()
  .AddDownstreamWebApi("DownstreamApi", builder.Configuration.GetSection("DownstreamApi"))
  .AddInMemoryTokenCaches();
builder.Services.AddAuthorization(options =>
{
  options.FallbackPolicy = options.DefaultPolicy;
});
builder.Services.AddControllersWithViews(options =>
{
  var policy = new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser()
      .Build();
  options.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();
builder.Services.AddHttpClient<IProductsService, ProductsService>();
var app = builder.Build(); 
/*This makes sure our client application requests the needed scopes from the api.

Next, let's see what we'll need to update in our ProductsService.

Start by injecting an ITokenAcquisition and an IConfiguration service into the constructor and storing them as private fields.
*/
//PRODUCTSSERVICE.CS
private readonly ITokenAcquisition tokenAcquisition;
private readonly IConfiguration configuration;
 
public ProductsService(HttpClient httpClient, ITokenAcquisition tokenAcquisition, IConfiguration configuration )
{
  this.httpClient = httpClient;
  this.tokenAcquisition = tokenAcquisition;
  this.configuration = configuration;
}
//Then, add a helper method to prepare your httpClient. The method should get an accessToken for the user and pass it in a bearer header for any request the httpClient might make. Then, from the GetProductsAsync method, call your prepare method. (While your're at it, replace the hardcoded url for your service with one that reads the baseAddress from config)

//PRODUCTSSERVICE.CS
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
    var products = JsonSerializer.Deserialize<List<string>>(jsonResult);
    return products;
  }
  return null;
} 
//This leaves us with one more thing to do, and that is assure the user gets asked to give consent to the client application to access the Products.List and Products.Add scopes. You can do this by adding the AuthorizeForScopes filter to the Index action of the HomeController.

//HOMECONTROLLER.CS
[AuthorizeForScopes(ScopeKeySection = "DownStreamApi:Scopes")]
public async Task<IActionResult> Index()
{
//That's it! Your application should now work. Test it to confirm.