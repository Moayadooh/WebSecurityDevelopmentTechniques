var defaultPolicy = "_defaultPolicy";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CustomersDb>(options =>
{
  options.UseInMemoryDatabase("Customers");
});

// builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  CustomersDb db = scope.ServiceProvider.GetRequiredService<CustomersDb>();
  db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(defaultPolicy);
app.UseAuthorization();

app.MapControllers();

app.Run();
