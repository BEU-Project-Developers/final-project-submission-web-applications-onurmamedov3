// At the top, add these using statements if they aren't there:
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Data;     // For ApplicationDbContext
using TourManagementSystem.Services;  // For IUserService and UserService (the files you just created)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Configure DbContext for MySQL (You should already have this from previous steps)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// 2. Register your UserService for Dependency Injection
// This tells the application: "When someone asks for an IUserService, give them an instance of UserService"
// AddScoped means a new UserService will be created for each web request.
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
 app.UseAuthentication(); // You would uncomment and configure this for actual login sessions
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();