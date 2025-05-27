// File: TourManagementSystem/Program.cs
using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Data;
using TourManagementSystem.Services; // Ensure this using directive is present
// using Microsoft.AspNetCore.Identity; // Uncomment if you plan to use ASP.NET Core Identity features

var builder = WebApplication.CreateBuilder(args);

// 1. Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
// .LogTo(Console.WriteLine, LogLevel.Information) // Uncomment to see EF Core queries in console during development
);

// 2. Register your custom services for Dependency Injection
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IUserService, UserService>(); 

builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<ICarRentalService, CarRentalService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<ICruiseService, CruiseService>();    
builder.Services.AddScoped<IActivityService, ActivityService>();

// 3. Add Identity services if you're using ASP.NET Core Identity
//    If your AccountController relies on UserManager<User> or SignInManager<User>,
//    you MUST configure Identity here.
//
//    Example (ensure your User model inherits from IdentityUser if you use this):
//    using TourManagementSystem.Models; // Assuming User model is here
//    using Microsoft.AspNetCore.Identity;
//
//    builder.Services.AddIdentity<User, IdentityRole>(options => {
//        options.SignIn.RequireConfirmedAccount = false; // Adjust as needed
//        options.Password.RequireDigit = true;
//        options.Password.RequireLowercase = true;
//        options.Password.RequireUppercase = true;
//        options.Password.RequireNonAlphanumeric = false;
//        options.Password.RequiredLength = 6;
//    })
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();
//
//    builder.Services.ConfigureApplicationCookie(options =>
//    {
//        options.LoginPath = "/Account/Login";       // Your login page
//        options.LogoutPath = "/Account/Logout";      // Your logout action
//        options.AccessDeniedPath = "/Account/AccessDenied"; // Your access denied page
//        options.SlidingExpiration = true;
//    });


// 4. Add MVC services
builder.Services.AddControllersWithViews();
// builder.Services.AddRazorPages(); // If you use Razor Pages

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();


}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// If you configured ASP.NET Core Identity, you need these:
// app.UseAuthentication(); // Crucial: Verifies the user's identity
// app.UseAuthorization();  // Crucial: Checks if the authenticated user has permission

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// app.MapRazorPages(); // If using Razor Pages

app.Run();