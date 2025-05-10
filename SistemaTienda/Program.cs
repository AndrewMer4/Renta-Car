using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaTienda.AccesoDatos.Data;
using SistemaTienda.AccesoDatos.Data.Repository;
using SistemaTienda.AccesoDatos.Data.Repository.iRepository;
using SistemaTienda.Data;
using SistemaTienda.Models;
using SistemaTienda.Utilidades;
using System.Globalization;

var culturaPersonalizada = new CultureInfo("es-SV");
culturaPersonalizada.NumberFormat.NumberDecimalSeparator = ".";
culturaPersonalizada.NumberFormat.CurrencyDecimalSeparator = ".";
CultureInfo.DefaultThreadCurrentCulture = culturaPersonalizada;
CultureInfo.DefaultThreadCurrentUICulture = culturaPersonalizada;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConexionSQL")
    ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSession();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection()
           .UseEphemeralDataProtectionProvider();
}

var app = builder.Build();

await CrearRolesIniciales(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area=Identity}/{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Redirect}/{action=ToAdmin}/{id?}");

app.MapControllerRoute(
    name: "empleado",
    pattern: "Empleado/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();

async Task CrearRolesIniciales(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { CNT.Admin, CNT.Empleado };
    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(new IdentityRole(rol));
        }
    }
}