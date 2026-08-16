using System.Security.Claims;
using BibliotecaVirtual.Web.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaVirtual.Web.Controllers
{
    public class CuentaController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly IPasswordHasher<BibliotecaVirtual.Web.Models.Usuario> _passwordHasher;

        public CuentaController(
            BibliotecaContext context,
            IPasswordHasher<BibliotecaVirtual.Web.Models.Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ==========================================
        // LOGIN - GET
        // ==========================================

        [HttpGet]
        public IActionResult Login()
        {
            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] = "no-cache";

            Response.Headers["Expires"] = "0";

            return View();
        }


        // ==========================================
        // LOGIN - POST
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            string nombreUsuario,
            string password)
        {
            // Buscar el usuario en la base de datos
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.NombreUsuario == nombreUsuario);

            // ==========================================
            // USUARIO NO EXISTE
            // ==========================================

            if (usuario == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "El usuario no existe.");

                return View();
            }


            // ==========================================
            // USUARIO DESACTIVADO
            // ==========================================

            if (!usuario.Activo)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Este usuario está desactivado.");

                return View();
            }


            // ==========================================
            // VERIFICAR CONTRASEÑA
            // ==========================================

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.PasswordHash,
                password);


            if (resultado == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "La contraseña es incorrecta.");

                return View();
            }


            // ==========================================
            // CREAR CLAIMS
            // ==========================================

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    usuario.NombreUsuario),

                new Claim(
                    ClaimTypes.Role,
                    usuario.Rol)
            };


            var claimsIdentity =
                new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);


            // ==========================================
            // CREAR SESIÓN
            // ==========================================

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));


            // ==========================================
            // REDIRECCIÓN SEGÚN ROL
            // ==========================================

            if (usuario.Rol == "Admin")
            {
                return RedirectToAction(
                    "Index",
                    "Usuarios");
            }


            return RedirectToAction(
                "Index",
                "Libros");
        }


        // ==========================================
        // LOGOUT
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] =
                "no-cache";

            Response.Headers["Expires"] =
                "0";

            return RedirectToAction(
                nameof(Login));
        }
    }
}