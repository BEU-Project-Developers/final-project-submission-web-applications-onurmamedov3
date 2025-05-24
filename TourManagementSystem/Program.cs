// File: TourManagementSystem/Program.cs
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Data;
using TourManagementSystem.Services;
// using Microsoft.AspNetCore.Identity; // If using Identity

var builder = WebApplication.CreateBuilder(args);

// 1. Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
// Add .LogTo(Console.WriteLine, LogLevel.Information) to see EF Core queries in console
);

// 2. Register your custom services for Dependency Injection
builder.Services.AddScoped<IHotelService, HotelService>();
// builder.Services.AddScoped<IUserService, UserService>(); // If you have a UserService
// Add other services (ICarRentalService, IFlightService, etc.) here

// 3. Add Identity services if you're using ASP.NET Core Identity
// builder.Services.AddIdentity<User, IdentityRole>() // Or your custom User and Role classes
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

// builder.Services.ConfigureApplicationCookie(options =>
// {
//    options.LoginPath = "/Account/Login";
//    options.AccessDeniedPath = "/Account/AccessDenied";
// });


// 4. Add MVC services
builder.Services.AddControllersWithViews();
// builder.Services.AddRazorPages(); // If you use Razor Pages alongside MVC

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}
else
{
    app.UseDeveloperExceptionPage(); // More detailed errors in development
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Enables serving static files from wwwroot

app.UseRouting();

// app.UseAuthentication(); // Must come before UseAuthorization
// app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// app.MapRazorPages(); // If using Razor Pages

app.Run();