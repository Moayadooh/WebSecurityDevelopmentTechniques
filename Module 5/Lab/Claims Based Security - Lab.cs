//Introduction
//In this exercise you will build a simple website that uses claims-based security.

//Prerequisites
//This lab requires a valid Azure subscription. There are no starter files, you will build a web site from scratch. This lab assumes the latest version of Visual Studio 2022.

/*Building the web site
Start Visual Studio and create a new project. Use the ASP.NET Core Web App (Model-View-Controller) template.
Set the Authentication Type to Microsoft identity platform.
Finish the wizard.
Open appsettings.json and locate the following configuration:*/
//APPSETTINGS.JSON
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "Domain": "qualified.domain.name",
  "TenantId": "22222222-2222-2222-2222-222222222222",
  "ClientId": "11111111-1111-1111-11111111111111111",
  "CallbackPath": "/signin-oidc"
},
/*You will see other values, of course, corresponding with the registration of the application in your Azure Active Directory.

One more thing:

Go to Project > Properties > Debug > Open Debug Launch Profiles and locate the debug URL. Make sure it is the one using HTTPS.

App registration
In this part, you will check your app registration in Microsoft Entra ID.

Visit the Azure portal (https://portal.azure.com) an sign in.
Go to Microsoft Entra ID (make sure you are in the correct tenant, if you have multiple).
Find App registrations and find your web app underneath All applications.
Visit your registration. Select Authentication. Under Implicit grant and hybrid flows, you will see ID tokens activated.

Seeing if it works
Let’s run the web site to see what happens. Press F5 to run.
The web site will immediately redirect you to the Microsoft login page:

Figure 1

Login using some identity you use for Azure (make sure that identity is known in Azure):

Looking at claims
Let us add some code to look at the claims that we got from Azure.

Open HomeController.cs (in the Controllers folder) and look for the Index method. Update its content:*/
//HOMECONTROLLER.CS
public IActionResult Index()
{
    ClaimsPrincipal claims = this.User;
    Claim? name = claims.FindFirst("name");
    ViewData["Message"] = $"Hello {name?.Value}";
    return View();
}
//Next, update the view so it displays the message:
//INDEX.CSHTML
@{
    ViewData["Title"] = "Home Page";
}
 
<div class="text-center">
    <h1 class="display-4">Welcome</h1>
    <p>@ViewData["Message"]</p>
</div>
/*Run the web site again, login and navigate to the homepage. You should see the user’s name.

Examining the generated code
So, what did Visual Studio do to enable Claims based security?

Open Program.cs. You’ll see the following implementation:*/
//STARTUP.CS
// Add services to the container.
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));
 
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
/*The important stuff is noted in bold. To make sure we can make use of Azure AD to get the user information, we need to add Authentication as a service. That method will create an AuthenticationBuilder that can be expanded with whatever Authentication you require. In our case we will add a connection to Microsoft identity but it can be used to add Cookie authentication, Jwt Bearer authentication, and others.

The AuthenticationBuilder needs to have a default AuthenticationScheme and the one from Microsoft Identity is the one we need now.

Binding the configuration “AzureAd” will make sure the appsettings.json file is read and bound to the properties of the options object.

This concludes the lab.*/