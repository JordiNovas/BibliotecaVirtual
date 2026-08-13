using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace BibliotecaVirtual.Tests
{
    public class UsuarioTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private const string BaseUrl = "http://localhost:5119";

        public UsuarioTests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        private void IniciarSesion()
        {
            _driver.Navigate().GoToUrl($"{BaseUrl}/Cuenta/Login");

            var txtUsuario = _wait.Until(d => d.FindElement(By.Name("nombreUsuario")));
            txtUsuario.Clear();
            txtUsuario.SendKeys("admin");

            var txtPassword = _driver.FindElement(By.Name("password"));
            txtPassword.Clear();
            txtPassword.SendKeys("Admin123!");

            var btnLogin = _driver.FindElement(By.XPath("//form[contains(@action, 'Login') or contains(@action, 'login')]//button[@type='submit'] | //button[@type='submit']"));
            btnLogin.Click();

            _wait.Until(d => !d.Url.Contains("/Cuenta/Login", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void CrearUsuarioConDatosValidos()
        {
            IniciarSesion();

            _driver.Navigate().GoToUrl($"{BaseUrl}/Usuarios/Crear");

            string timeStamp = DateTime.Now.Ticks.ToString().Substring(10);
            string nombreTest = $"TestUser_{timeStamp}";
            string correoTest = $"user_{timeStamp}@prueba.com";
            string usuarioTest = $"usr_{timeStamp}";

            LlenarCampoSiExiste(By.Name("Nombre"), nombreTest);
            LlenarCampoSiExiste(By.Name("Correo"), correoTest);
            LlenarCampoSiExiste(By.Name("NombreUsuario"), usuarioTest);
            LlenarCampoSiExiste(By.Name("PasswordHash"), "User123!");
            LlenarCampoSiExiste(By.Name("Clave"), "User123!");

            var btnGuardar = _driver.FindElement(By.XPath("//form[not(contains(@action, 'Logout'))]//button[@type='submit' or contains(text(), 'Guardar') or contains(text(), 'Crear')] | //input[@type='submit']"));
            btnGuardar.Click();

            _wait.Until(d => !d.Url.EndsWith("/Usuarios/Crear", StringComparison.OrdinalIgnoreCase));

            string urlFinal = _driver.Url;

            Assert.DoesNotContain("/Cuenta/Logout", urlFinal, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("/Cuenta/Login", urlFinal, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("/Usuarios", urlFinal, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void EditarUsuarioConDatosValidos()
        {
            IniciarSesion();

            _driver.Navigate().GoToUrl($"{BaseUrl}/Usuarios");

            var btnEditar = _wait.Until(d => d.FindElement(By.XPath("//a[contains(@href, '/Usuarios/Editar') or contains(text(), 'Editar')]")));
            btnEditar.Click();

            string nuevoNombre = $"UserModificado_{DateTime.Now.Ticks.ToString().Substring(10)}";

            var txtNombre = _wait.Until(d => d.FindElement(By.Id("Nombre")));
            txtNombre.Clear();
            txtNombre.SendKeys(nuevoNombre);

            var btnGuardar = _driver.FindElement(By.XPath("//form[not(contains(@action, 'Logout'))]//button[@type='submit' or contains(text(), 'Guardar') or contains(text(), 'Actualizar')]"));
            btnGuardar.Click();

            _wait.Until(d => d.Url.EndsWith("/Usuarios", StringComparison.OrdinalIgnoreCase) || d.Url.Contains("/Usuarios/Index", StringComparison.OrdinalIgnoreCase));

            Assert.Contains(nuevoNombre, _driver.PageSource);
        }

        [Fact]
        public void EliminarUsuarioExitosamente()
        {
            IniciarSesion();

            // 1. Crear un usuario rápido exclusivo para ser eliminado
            _driver.Navigate().GoToUrl($"{BaseUrl}/Usuarios/Crear");

            string timeStamp = DateTime.Now.Ticks.ToString().Substring(10);
            string usuarioAEliminar = $"Eliminar_{timeStamp}";

            _wait.Until(d => d.FindElement(By.Id("Nombre"))).SendKeys(usuarioAEliminar);
            LlenarCampoSiExiste(By.Name("Correo"), $"del_{timeStamp}@test.com");
            LlenarCampoSiExiste(By.Name("NombreUsuario"), $"usr_del_{timeStamp}");
            LlenarCampoSiExiste(By.Name("PasswordHash"), "User123!");

            var btnGuardar = _driver.FindElement(By.XPath("//form[not(contains(@action, 'Logout'))]//button[@type='submit' or contains(text(), 'Guardar') or contains(text(), 'Crear')]"));
            btnGuardar.Click();

            // Esperar a que salga de la vista Crear
            _wait.Until(d => !d.Url.EndsWith("/Usuarios/Crear", StringComparison.OrdinalIgnoreCase));

            // 2. Asegurarnos de estar en la tabla de usuarios
            if (!_driver.Url.Contains("/Usuarios", StringComparison.OrdinalIgnoreCase))
            {
                _driver.Navigate().GoToUrl($"{BaseUrl}/Usuarios");
            }

            // 3. Localizar el enlace/botón "Eliminar" específico para esa fila
            var btnEliminar = _wait.Until(d => d.FindElement(By.XPath($"//tr[td[contains(text(), '{usuarioAEliminar}')]]//a[contains(@href, 'Eliminar') or contains(text(), 'Eliminar')] | //tr[td[contains(text(), '{usuarioAEliminar}')]]//button[contains(text(), 'Eliminar')]")));
            btnEliminar.Click();

            // 4. Manejar posible alerta JS o formulario de confirmación en vista/modal
            try
            {
                var alert = _driver.SwitchTo().Alert();
                alert.Accept();
            }
            catch (NoAlertPresentException)
            {
                var botonesConfirmar = _driver.FindElements(By.XPath("//form[contains(@action, 'Eliminar')]//button[@type='submit' or contains(text(), 'Eliminar')] | //form[contains(@action, 'Delete')]//button[@type='submit'] | //input[@type='submit']"));
                if (botonesConfirmar.Count > 0 && botonesConfirmar[0].Displayed)
                {
                    botonesConfirmar[0].Click();
                }
            }

            // 5. Esperar a que la página procese la acción
            _wait.Until(d => !d.Url.Contains("/Usuarios/Eliminar", StringComparison.OrdinalIgnoreCase));

            // 6. Verificar que el usuario creado ya no aparece en el código fuente
            Assert.DoesNotContain(usuarioAEliminar, _driver.PageSource);
        }

        private void LlenarCampoSiExiste(By locator, string valor)
        {
            var elementos = _driver.FindElements(locator);
            if (elementos.Count > 0 && elementos[0].Displayed)
            {
                elementos[0].Clear();
                elementos[0].SendKeys(valor);
            }
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}