using Microsoft.EntityFrameworkCore;
using TourManagementSystem.Data;
using TourManagementSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies; // << REQUIRED for Cookie Authentication

var builder = WebApplication.CreateBuilder(args);

// 1. Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// 2. Register your custom services for Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<ICarRentalService, CarRentalService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<ICruiseService, CruiseService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<ITripService, TripService>();

// 3. Configure Authentication (Cookie based for your custom system)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Sets the default scheme
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => // Configure the cookie scheme
    {
        options.LoginPath = "/Account/Login";       // Path to redirect for login
        options.LogoutPath = "/Account/Logout";      // Path for logout action
        options.AccessDeniedPath = "/Account/AccessDenied"; // Path if authorized but role mismatch
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // How long the cookie is valid
        options.SlidingExpiration = true; // Renews the cookie if user is active
        // options.Cookie.Name = "YourApp.AuthCookie"; // Optional: customize cookie name
        options.Cookie.HttpOnly = true; // Recommended for security
        options.Cookie.IsEssential = true; // Important for GDPR compliance if you don't have cookie consent
    });

// Authorization services are typically added by AddControllersWithViews or AddIdentity,
// but explicitly adding it can be done if needed.
// builder.Services.AddAuthorization();

// 4. Add MVC services
builder.Services.AddControllersWithViews();

// Session services are separate from authentication cookies.
// If you need general session state:
// builder.Services.AddDistributedMemoryCache();
// builder.Services.AddSession(options =>
// {
//     options.IdleTimeout = TimeSpan.FromMinutes(30);
//     options.Cookie.HttpOnly = true;
//     options.Cookie.IsEssential = true;
// });


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

app.UseRouting(); // Defines route matching.

// --- MIDDLEWARE ORDER IS CRITICAL ---
// 1. Authentication: Identifies who the user is based on the configured schemes (e.g., reads the cookie).
app.UseAuthentication(); // << NOW THIS IS ESSENTIAL AND CONFIGURED

// 2. Authorization: Determines if the identified user has permission.
app.UseAuthorization();  // This processes [Authorize] attributes

// app.UseSession(); // If you added session services above

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();