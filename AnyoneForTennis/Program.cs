using Microsoft.EntityFrameworkCore;
using AnyoneForTennis.Data;
using AnyoneForTennis.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using AnyoneForTennis.OldDatas.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContextConnection") 
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

var previousDataConnectionString = builder.Configuration.GetConnectionString("PreviousDatabaseConnection") 
    ?? throw new InvalidOperationException("Connection string 'PreviousDatabaseConnection' not found.");
builder.Services.AddDbContext<Hitdb1Context>(options =>
    options.UseSqlServer(previousDataConnectionString));


builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = false;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<DataInitializer>();

var app = builder.Build();

using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;

var initialiser = services.GetRequiredService<DataInitializer>();

initialiser.Run();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
