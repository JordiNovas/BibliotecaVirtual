using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;
using BibliotecaVirtual.Web.Data;
using BibliotecaVirtual.Web.Models;

namespace BibliotecaVirtual.Tests
{
    public class UsuarioTests : IDisposable
    {
        private readonly IWebDriver driver;
        private readonly WebDriverWait wait;

        private static Process? webAppProcess;

        private const string BaseUrl = "http://localhost:5119";

        private const string DbConnectionString =
            @"Server=JORDINOVAS\MSSQLSERVER02;Database=BibliotecaVirtualDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        public UsuarioTests()
        {
            AsegurarServidorWebActivo();

            var options = new ChromeOptions();

            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);

            wait = new WebDriverWait(
                driver,
                TimeSpan.FromSeconds(15));
        }

        private void AsegurarServidorWebActivo()
        {
            if (webAppProcess != null &&
                !webAppProcess.HasExited)
            {
                return;
            }

            webAppProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",

                    Arguments =
                        @"run --project C:\BibliotecaVirtual\BibliotecaVirtual.Web\BibliotecaVirtual.Web.csproj --urls http://localhost:5119",

                    UseShellExecute = false,
                    CreateNoWindow = true,

                    WorkingDirectory =
                        @"C:\BibliotecaVirtual\BibliotecaVirtual.Web"
                }
            };

            webAppProcess.Start();

            using var client = new HttpClient();

            bool listo = false;

            for (int i = 0; i < 30 && !listo; i++)
            {
                try
                {
                    var response = client
                        .GetAsync(BaseUrl)
                        .GetAwaiter()
                        .GetResult();

                    listo = response.IsSuccessStatusCode;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }

            if (!listo)
            {
                throw new Exception(
                    "La aplicación BibliotecaVirtual.Web no pudo iniciar en " +
                    BaseUrl);
            }
        }

        private BibliotecaContext CrearDbContext()
        {
            var optionsBuilder =
                new DbContextOptionsBuilder<BibliotecaContext>();

            optionsBuilder.UseSqlServer(DbConnectionString);

            return new BibliotecaContext(
                optionsBuilder.Options);
        }

        private void SembrarUsuarioEnBD()
        {
            using var db = CrearDbContext();

            var usuarioExistente = db.Usuarios
                .FirstOrDefault(u =>
                    u.NombreUsuario == "testuser_selenium");

            if (usuarioExistente == null)
            {
                db.Usuarios.Add(new Usuario
                {
                    Nombre = "Usuario Test Selenium",
                    Correo = "selenium.test@biblioteca.com",
                    NombreUsuario = "testuser_selenium",
                    PasswordHash = "Password123!",
                    Activo = true,
                    FechaRegistro = DateTime.Now
                });

                db.SaveChanges();
            }
        }

        [Fact]
        public void EditarUsuarioConDatosValidos()
        {
            SembrarUsuarioEnBD();

            driver.Navigate()
                .GoToUrl($"{BaseUrl}/Usuarios");

            var filaUsuario = wait.Until(d =>
                d.FindElement(
                    By.XPath(
                        "//table/tbody/tr[td[contains(normalize-space(), 'testuser_selenium')]]"
                    )
                )
            );

            var btnEditar = filaUsuario.FindElement(
                By.XPath(
                    ".//a[contains(@href, '/Usuarios/Editar')]"
                )
            );

            var urlEditar = btnEditar.GetAttribute("href");

            Assert.False(
                string.IsNullOrWhiteSpace(urlEditar),
                "El enlace de edición no contiene una URL válida.");

            driver.Navigate()
                .GoToUrl(urlEditar!);

                
            var inputNombre = wait.Until(d =>
                d.FindElement(By.Id("Nombre"))
            );

            inputNombre.Clear();

            inputNombre.SendKeys(
                "Nombre Editado Selenium");

            var btnGuardar = wait.Until(d =>
                d.FindElement(By.Id("btnGuardar"))
            );

            btnGuardar.Click();

            wait.Until(d =>
                d.Url.EndsWith("/Usuarios") ||
                d.Url.EndsWith("/Usuarios/")
        );

            Assert.Contains(
                "/Usuarios",
                driver.Url,
                StringComparison.OrdinalIgnoreCase);

            using var db = CrearDbContext();

            var usuario = db.Usuarios
                .FirstOrDefault(u =>
                    u.NombreUsuario == "testuser_selenium");

            Assert.NotNull(usuario);

            Assert.Equal(
                "Nombre Editado Selenium",
                usuario!.Nombre);
        }

        [Fact]
        public void EliminarUsuarioExitosamente()
        {
            SembrarUsuarioEnBD();

            driver.Navigate()
                .GoToUrl($"{BaseUrl}/Usuarios");

            var filaUsuario = wait.Until(d =>
                d.FindElement(
                    By.XPath(
                        "//table/tbody/tr[td[contains(normalize-space(), 'testuser_selenium')]]"
                    )
                )
            );

            var btnEliminar = filaUsuario.FindElement(
                By.XPath(
                    ".//a[contains(@href, '/Usuarios/Eliminar')]"
                )
            );

            var urlEliminar =
                btnEliminar.GetAttribute("href");

            Assert.False(
                string.IsNullOrWhiteSpace(urlEliminar));

            driver.Navigate()
                .GoToUrl(urlEliminar);

            wait.Until(d =>
                d.Url.Contains(
                    "/Usuarios/Eliminar",
                    StringComparison.OrdinalIgnoreCase)
            );

            Assert.Contains(
                "/Usuarios/Eliminar",
                driver.Url,
                StringComparison.OrdinalIgnoreCase);

            var btnConfirmarEliminar = wait.Until(d =>
                d.FindElement(
                    By.Id("btnConfirmarEliminar")
                )
            );

            btnConfirmarEliminar.Click();

            wait.Until(d =>
                d.Url.EndsWith("/Usuarios") ||
                d.Url.EndsWith("/Usuarios/"));

            Assert.Contains(
                "/Usuarios",
                driver.Url,
                StringComparison.OrdinalIgnoreCase);

            using var db = CrearDbContext();

            var usuarioEliminado = db.Usuarios
                .FirstOrDefault(u =>
                    u.NombreUsuario == "testuser_selenium");

            Assert.Null(usuarioEliminado);
        }

        public void Dispose()
        {
            driver?.Quit();
            driver?.Dispose();

            if (webAppProcess != null &&
                !webAppProcess.HasExited)
            {
                try
                {
                    webAppProcess.Kill(true);
                    webAppProcess.WaitForExit();
                    webAppProcess.Dispose();
                }
                catch
                {
                    // El proceso pudo haber terminado automáticamente.
                }

                webAppProcess = null;
            }
        }
    }
}