Introduction
In this lab, you are going to add a CORS policy to an existing API.

Before you start
This lab starts from starter files which you can download from the https://online.u2u.be platform.

The starter files Contain 2 projects:

-          a client app which we will be using to test our CORS policy.

-          An API project that contains 1000 random customers which we want to GET or DELETE.

Applying a CORS Policy
Start your API and your client application, the client application should look something like this:

Figure 1

When you click either of the buttons on this page, you will be met with TypeError: Failed to fetch. This is because the Same Origin policy prevents the client app from loading any resources from the API.
Create a CORS policy that allows GET, POST, and PUT from your client app, and apply it globally to your API.
PROGRAM.CS
var defaultPolicy = "_defaultPolicy";
var builder = WebApplication.CreateBuilder(args);
...
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
  options.AddPolicy(name: defaultPolicy,
                          options =>
                          {
                            options.WithOrigins("https://localhost:7179")
                                   .WithMethods("GET", "PUT", "POST");
                          });
});
...
app.UseHttpsRedirection();
app.UseCors(defaultPolicy);
app.UseAuthorization();
Test your implementation. The GET button should now give a list of 50 customers, while the DELETE button still shows a CORS error.
This concludes the lab.