using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Data;
using TourManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure DbContext (MUST be registered before services that depend on it)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// 2. Register ALL your custom services for Dependency Injection
//    Order of registration for AddScoped/AddTransient/AddSingleton doesn't strictly matter
//    among themselves, but services that depend on DbContext must be registered AFTER DbContext.

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICarRentalService, CarRentalService>(); // <<--- ENSURE THIS LINE EXISTS AND IS CORRECT
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<ICruiseService, CruiseService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<ITripService, TripService>();
// Add any other services you have

// 3. Configure Authentication (Cookie based for your custom system)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// 4. Add MVC services
builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Ensures ILogger<T> can be injected

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();