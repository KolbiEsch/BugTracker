using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using BugTracker;
using BugTracker.Data;
using BugTracker.ProjectServices;
using Microsoft.AspNetCore.Authorization;
using BugTracker.Data.Authorization;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environments.Staging,
    WebRootPath = "wwwroot"
});


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews().AddJsonOptions(jsonOptions =>
{
    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
});


builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1.0);
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();

var emailConfig = config.GetSection("EmailConfiguration").Get<EmailConfiguration>();

builder.Services.AddSingleton(emailConfig);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("assignedTicketPolicy",
        policy =>
            policy.AddRequirements(
                new AssignedTicketRequirement()
            ));
});

builder.Services.AddScoped<ProjectService>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddSingleton<IAuthorizationHandler, DocumentAuthorizationHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
