using System.Security.Claims;
using BibliotecaVirtual.Web.Data;
using BibliotecaVirtual.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;

namespace BibliotecaVirtual.Web.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly BibliotecaContext _context;

        public LibrosController(BibliotecaContext context)
        {
            _context = context;
        }

        // ==========================================
        // CATÁLOGO DE LIBROS
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros
                .ToListAsync();

            return View(libros);
        }

        // ==========================================
        // SOLICITAR PRÉSTAMO
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prestar(int id)
        {
            // Buscar libro.
            var libro = await _context.Libros
                .FindAsync(id);

            if (libro == null)
            {
                TempData["Error"] = "El libro no existe.";

                return RedirectToAction(nameof(Index));
            }

            if (!libro.Disponible)
            {
                TempData["Error"] = "El libro no está disponible.";

                return RedirectToAction(nameof(Index));
            }

            // Obtener ID del usuario autenticado.
            var usuarioIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioIdClaim) ||
                !int.TryParse(usuarioIdClaim, out int usuarioId))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            // Comprobar que el usuario existe realmente.
            var usuarioExiste = await _context.Usuarios
                .AnyAsync(u =>
                    u.Id == usuarioId &&
                    u.Activo);

            if (!usuarioExiste)
            {
                await HttpContext.SignOutAsync();

                return RedirectToAction("Login", "Cuenta");
            }

            // ==========================================
            // CREAR PRÉSTAMO
            // ==========================================
            var prestamo = new Prestamo
            {
                LibroId = libro.Id,
                UsuarioId = usuarioId,
                FechaPrestamo = DateTime.Now,
                Estado = "Activo"
            };

            // Marcar libro como no disponible.
            libro.Disponible = false;

            _context.Prestamos.Add(prestamo);

            await _context.SaveChangesAsync();

            TempData["Exito"] =
                $"¡Has solicitado '{libro.Titulo}' con éxito!";

            return RedirectToAction(nameof(MisPrestamos));
        }

        // ==========================================
        // MIS PRÉSTAMOS
        // ==========================================
        public async Task<IActionResult> MisPrestamos()
        {
            var usuarioIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioIdClaim) ||
                !int.TryParse(usuarioIdClaim, out int usuarioId))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var misPrestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Where(p =>
                    p.UsuarioId == usuarioId &&
                    p.Estado == "Activo")
                .ToListAsync();

            return View(misPrestamos);
        }

        // ==========================================
        // DEVOLVER LIBRO
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int prestamoId)
        {
            var usuarioIdClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(usuarioIdClaim) ||
                !int.TryParse(usuarioIdClaim, out int usuarioId))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var prestamo = await _context.Prestamos
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p =>
                    p.Id == prestamoId &&
                    p.UsuarioId == usuarioId);

            if (prestamo == null)
            {
                TempData["Error"] = "El préstamo no existe.";

                return RedirectToAction(nameof(MisPrestamos));
            }

            if (prestamo.Estado != "Activo")
            {
                TempData["Error"] =
                    "Este préstamo ya fue devuelto.";

                return RedirectToAction(nameof(MisPrestamos));
            }

            prestamo.Estado = "Devuelto";
            prestamo.FechaDevolucion = DateTime.Now;

            if (prestamo.Libro != null)
            {
                prestamo.Libro.Disponible = true;
            }

            await _context.SaveChangesAsync();

            TempData["Exito"] =
                $"Has devuelto el libro '{prestamo.Libro?.Titulo}'.";

            return RedirectToAction(nameof(MisPrestamos));
        }
    }
}