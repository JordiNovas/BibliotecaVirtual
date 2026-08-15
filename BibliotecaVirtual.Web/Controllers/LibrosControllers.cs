using System.Security.Claims;
using BibliotecaVirtual.Web.Data;   // <-- Importante para encontrar BibliotecaContext
using BibliotecaVirtual.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // 1. Catálogo Principal de Libros
        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        // 2. Acción para Solicitar Préstamo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Prestar(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null || !libro.Disponible)
            {
                TempData["Error"] = "El libro no está disponible.";
                return RedirectToAction(nameof(Index));
            }

            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null) return RedirectToAction("Login", "Cuenta");

            int usuarioId = int.Parse(usuarioIdClaim);

            var prestamo = new Prestamo
            {
                LibroId = libro.Id,
                UsuarioId = usuarioId,
                FechaPrestamo = DateTime.Now,
                Estado = "Activo"
            };

            libro.Disponible = false;

            _context.Prestamos.Add(prestamo);
            _context.Update(libro);
            await _context.SaveChangesAsync();

            TempData["Exito"] = $"¡Has solicitado '{libro.Titulo}' con éxito!";
            return RedirectToAction(nameof(MisPrestamos));
        }

        // 3. Ver los Libros del usuario actual
        public async Task<IActionResult> MisPrestamos()
        {
            var usuarioIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioIdClaim == null) return RedirectToAction("Login", "Cuenta");

            int usuarioId = int.Parse(usuarioIdClaim);

            var misPrestamos = await _context.Prestamos
                .Include(p => p.Libro)
                .Where(p => p.UsuarioId == usuarioId && p.Estado == "Activo")
                .ToListAsync();

            return View(misPrestamos);
        }

        // 4. Acción para Devolver un Libro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int prestamoId)
        {
            var prestamo = await _context.Prestamos.Include(p => p.Libro).FirstOrDefaultAsync(p => p.Id == prestamoId);

            if (prestamo != null && prestamo.Estado == "Activo")
            {
                prestamo.Estado = "Devuelto";
                prestamo.FechaDevolucion = DateTime.Now;
                prestamo.Libro!.Disponible = true;

                _context.Update(prestamo);
                _context.Update(prestamo.Libro);
                await _context.SaveChangesAsync();

                TempData["Exito"] = $"Has devuelto el libro '{prestamo.Libro.Titulo}'.";
            }

            return RedirectToAction(nameof(MisPrestamos));
        }
    }
}