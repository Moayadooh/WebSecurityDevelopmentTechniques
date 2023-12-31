/*Introduction
In this lab we will modify an existing website to get some Access Control working. The website shows a User’s Account and a page with an overview of all of the Users. The website also shows the salaries of every user. We want to make sure the normal users can only see their Account. A Department Manager can only see his own employees and an HR employee can see all users.

This lab starts from starter files. Ask your trainer if you cannot find them. The files should be called something like “BrokenAccessControl”

Open and explore the project
Open the FixAccessControl.sln starter file.

Have a look around in the project. You should find an AccountController with a Login and a Logout method. And a Show and ShowAll Action. Those last two methods are the ones we are interested in.

To help you with starting up your application, we have created the Identity structure for you. It will be initialized through a class called LabManager (a very simple/quick and dirty implementation). To make sure that it works on your machine, deploy the migration to your database.

PACKAGE MANAGER CONSOLE*/
Update-Database
/*This should run the migrations and deploy everything correctly. You should find the tables in your database (by default the localdb instance).

 

You should now be able to run the application and have a look around. If you try to go to the Show or Show All action – or if you try to log in, you need a user account. You can find them all in the LabManager class, but if you want to see an overview:

Figure 1

You can try a couple of accounts if you want to, but the ShowAll action, currently shows everything for everyone. This is the action we really need to focus on!

Implementing the AuthorizationHandlers and Requirements
To make sure our users can only go to the ShowAll action if the user is in the “Manager” role or in the “HR” department, we will make sure to implement a policy to enforce this.

Add a policy which asserts whether the current user is in the "Manager" role, or in the "HR" department.
PROGRAM.CS*/
builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AllowMultipleUsers", pb =>
  {
    pb.RequireAssertion(context => context.User.HasClaim(c =>
            (c.Type == "Department" && c.Value == "HR") || context.User.IsInRole("Manager")));
  });
});

/*Add the policy in the controller
Modify the Authorize Attribute on top of the ShowAll action, so it makes use of the AllowMultipleUsers policy.

ACCOUNTCONTROLLER.CS*/
[Authorize(policy: "AllowMultipleUsers")]
public async Task<IActionResult> ShowAll(){ ... }
//This should now make sure only managers and HR employees can actually access the ShowAll method.

//To finish the settings, we are also going to look at the manager role inside this method. If the user is not part of the HR department, the user should only see the employees of the department the user is in.

/*Add a check for the claim of the Department for the current User
Look if the department value is “HR”
If it is not HR, only accept those people coming from the own department. The usermanager has a dedicated method just for this.
If it is HR, we can show all of the users.
ACCOUNTCONTROLLER.CS*/
[Authorize(policy: "AllowMultipleUsers")]
public async Task<IActionResult> ShowAll()
{
  var department = User.Claims.FirstOrDefault(
                                          c => c.Type == "Department")?.Value;
  IList<AppUser> selectedUsers;
  if (department != "HR")
  // We have a manager in this case who cannot see everything, only his own   
  // department!
  {
    selectedUsers = await _userManager.GetUsersForClaimAsync
                 (new System.Security.Claims.Claim("Department", department));
  }
  else
  {
    selectedUsers = _userManager.Users.ToList();
  }
  var vm = selectedUsers.Select(
              u => new ShowUserViewModel { Email = u.Email, Salary = u.Salary, PhoneNumber = u.PhoneNumber, UserName = u.UserName }).ToList();
  return View(vm);
}
//Test your application, you should now only be able to see all of the users if you are part of the HR department. You can see the users of your own department if you’re part of the Managers role. You can only see your own data, if you’re a normal user.

//This concludes the Lab.

//(Optional) Update the UI
//Hide UI elements if they’re not relevant to your current User position.
