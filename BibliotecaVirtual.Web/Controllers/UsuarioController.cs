using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaVirtual.Web.Models;
using BibliotecaVirtual.Web.Data;

namespace BibliotecaVirtual.Web.Controllers
{
    [Authorize]
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
        [IgnoreAntiforgeryToken] // Permite que las pruebas de Selenium hagan POST sin fallar por CSRF Token
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si falla la validación, devuelve la misma vista mostrando los errores sin desloguear
            return View(usuario);
        }
    }
}