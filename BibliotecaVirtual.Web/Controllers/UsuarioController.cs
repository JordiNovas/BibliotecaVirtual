using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaVirtual.Web.Models;
using BibliotecaVirtual.Web.Data;

namespace BibliotecaVirtual.Web.Controllers
{
    [AllowAnonymous]
    public class UsuariosController : Controller
    {
        private readonly BibliotecaContext _context;

        public UsuariosController(BibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var usuarios = _context.Usuarios.ToList();

            return View(usuarios);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (usuario.FechaRegistro == default)
            {
                usuario.FechaRegistro = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        [HttpGet]
        public IActionResult Editar(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Editar(
            int id,
            string Nombre)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(Nombre))
            {
                ModelState.AddModelError(
                    "Nombre",
                    "El nombre es obligatorio.");

                return View(usuario);
            }

            usuario.Nombre = Nombre.Trim();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}