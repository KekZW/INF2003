using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RentingSystemMVC.Data;
using RentingSystemMVC.Models;

var builder = WebApplication.CreateBuilder(args);

// Load configuration settings from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");

// Configure DatabaseSettings from appsettings.json
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("MongoDB"));

// Register MongoDB context
builder.Services.AddSingleton<MongoDBContext>();

// Add other services
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

// Configure MySQL DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// Resolve and ensure MongoDB collections exist
var mongoContext = app.Services.GetRequiredService<MongoDBContext>();
mongoContext.EnsureCollectionsExist();

// Configure middleware and routing
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
