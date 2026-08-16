using BibliotecaVirtual.Web.Data;
using BibliotecaVirtual.Web.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ======================================================
// MVC
// ======================================================

builder.Services.AddControllersWithViews();


// ======================================================
// BASE DE DATOS
// ======================================================

builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));


// ======================================================
// PASSWORD HASHER
// ======================================================

builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();


// ======================================================
// AUTENTICACIÓN CON COOKIES
// ======================================================

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Página a la que irá el usuario
        // cuando no haya iniciado sesión.
        options.LoginPath = "/Cuenta/Login";

        // Página a la que irá si no tiene permisos.
        options.AccessDeniedPath = "/Cuenta/Login";

        // Nombre de la cookie.
        options.Cookie.Name = "BibliotecaVirtual.Auth";

        // Evita problemas con cookies.
        options.Cookie.HttpOnly = true;

        // Seguridad de la cookie.
        options.Cookie.SecurePolicy =
            CookieSecurePolicy.SameAsRequest;

        // Tiempo de duración de la sesión.
        options.ExpireTimeSpan =
            TimeSpan.FromHours(2);

        // Renovar automáticamente la cookie
        // mientras el usuario siga activo.
        options.SlidingExpiration = true;
    });


// ======================================================
// AUTORIZACIÓN
// ======================================================

builder.Services.AddAuthorization();


var app = builder.Build();


// ======================================================
// CONFIGURACIÓN DEL PIPELINE
// ======================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}


// ======================================================
// HTTPS
// ======================================================

app.UseHttpsRedirection();


// ======================================================
// ARCHIVOS ESTÁTICOS
// ======================================================

app.UseStaticFiles();


// ======================================================
// ROUTING
// ======================================================

app.UseRouting();


// ======================================================
// AUTENTICACIÓN
// IMPORTANTE: debe ir ANTES de Authorization
// ======================================================

app.UseAuthentication();


// ======================================================
// AUTORIZACIÓN
// ======================================================

app.UseAuthorization();


// ======================================================
// RUTA PRINCIPAL
// ======================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}");


// ======================================================
// EJECUTAR APLICACIÓN
// ======================================================

app.Run();