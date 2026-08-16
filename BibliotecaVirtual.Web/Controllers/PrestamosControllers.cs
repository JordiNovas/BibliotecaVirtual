using BibliotecaVirtual.Web.Data;
using BibliotecaVirtual.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaVirtual.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PrestamosController : Controller
    {
        private readonly BibliotecaContext _context;

        public PrestamosController(BibliotecaContext context)
        {
            _context = context;
        }

        // ==========================================
        // VER TODOS LOS PRÉSTAMOS
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var prestamos = await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .OrderByDescending(p => p.FechaPrestamo)
                .ToListAsync();

            return View(prestamos);
        }

        // ==========================================
        // VER DETALLES DE UN PRÉSTAMO
        // ==========================================
        public async Task<IActionResult> Details(int id)
        {
            var prestamo = await _context.Prestamos
                .Include(p => p.Usuario)
                .Include(p => p.Libro)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prestamo == null)
            {
                return NotFound();
            }

            return View(prestamo);
        }
    }
}