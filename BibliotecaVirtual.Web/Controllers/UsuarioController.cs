using BibliotecaVirtual.Web.Data;
using BibliotecaVirtual.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaVirtual.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController : Controller
    {
        private readonly BibliotecaContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(
            BibliotecaContext context,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ==========================================
        // LISTAR USUARIOS
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .OrderBy(u => u.Id)
                .ToListAsync();

            return View(usuarios);
        }

        // ==========================================
        // CREAR USUARIO - GET
        // ==========================================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // ==========================================
        // CREAR USUARIO - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario, string password)
        {
            // La contraseña será introducida por el administrador.
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(
                    "password",
                    "La contraseña es obligatoria.");
            }

            // Comprobar que el nombre de usuario no exista.
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == usuario.NombreUsuario);

            if (usuarioExiste)
            {
                ModelState.AddModelError(
                    "NombreUsuario",
                    "Ese nombre de usuario ya existe.");
            }

            // Comprobar que el correo no exista.
            var correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Correo == usuario.Correo);

            if (correoExiste)
            {
                ModelState.AddModelError(
                    "Correo",
                    "Ese correo ya está registrado.");
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            // Todos los usuarios creados desde aquí serán usuarios normales.
            usuario.Rol = "Usuario";
            usuario.Activo = true;
            usuario.FechaRegistro = DateTime.Now;

            // Hashear contraseña.
            usuario.PasswordHash = _passwordHasher.HashPassword(
                usuario,
                password);

            _context.Usuarios.Add(usuario);

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Usuario registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // EDITAR USUARIO - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // ==========================================
        // EDITAR USUARIO - POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            var usuarioExistente = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Comprobar nombre de usuario duplicado.
            var nombreDuplicado = await _context.Usuarios
                .AnyAsync(u =>
                    u.NombreUsuario == usuario.NombreUsuario &&
                    u.Id != id);

            if (nombreDuplicado)
            {
                ModelState.AddModelError(
                    "NombreUsuario",
                    "Ese nombre de usuario ya existe.");
            }

            // Comprobar correo duplicado.
            var correoDuplicado = await _context.Usuarios
                .AnyAsync(u =>
                    u.Correo == usuario.Correo &&
                    u.Id != id);

            if (correoDuplicado)
            {
                ModelState.AddModelError(
                    "Correo",
                    "Ese correo ya está registrado.");
            }

            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            usuarioExistente.Nombre = usuario.Nombre;
            usuarioExistente.Correo = usuario.Correo;
            usuarioExistente.NombreUsuario = usuario.NombreUsuario;
            usuarioExistente.Activo = usuario.Activo;

            // No cambiamos la contraseña desde este formulario.
            // Se conserva el hash existente.

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Usuario actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // ELIMINAR USUARIO - GET
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // ==========================================
        // ELIMINAR USUARIO - POST
        // ==========================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios
                .FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            // Evitar que el administrador se elimine a sí mismo.
            var usuarioActualId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

            if (usuarioActualId != null &&
                int.TryParse(usuarioActualId.Value, out int idActual) &&
                idActual == usuario.Id)
            {
                TempData["Error"] =
                    "No puedes eliminar el usuario con el que estás conectado.";

                return RedirectToAction(nameof(Index));
            }

            // Comprobar si tiene préstamos.
            var tienePrestamos = await _context.Prestamos
                .AnyAsync(p => p.UsuarioId == id);

            if (tienePrestamos)
            {
                TempData["Error"] =
                    "No puedes eliminar este usuario porque tiene préstamos registrados.";

                return RedirectToAction(nameof(Index));
            }

            _context.Usuarios.Remove(usuario);

            await _context.SaveChangesAsync();

            TempData["Exito"] = "Usuario eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}