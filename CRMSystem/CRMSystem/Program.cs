using CRMSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register DbContext
builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Identity with roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<CrmDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Required for Identity UI

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; // ✅ Required for cross-site
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // ✅ HTTPS only
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // ✅ Important
    });
});

var app = builder.Build();

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRolesAndAdminAsync(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("AllowAngular");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();





app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

//app.Use(async (context, next) =>
//{
//    context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
//    context.Response.Headers["Pragma"] = "no-cache";
//    context.Response.Headers["Expires"] = "0";
//    await next();
//});

app.MapRazorPages(); // Enable Identity UI routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Report}/{action=LoginRedirect}/{id?}");

app.Run();

