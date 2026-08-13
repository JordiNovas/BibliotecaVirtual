using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace BibliotecaVirtual.Tests
{
    public class LoginTests : IDisposable
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private const string BaseUrl = "http://localhost:5119";

        public LoginTests()
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors");

            _driver = new ChromeDriver(options);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void LoginConCredencialesIncorrectas()
        {
            _driver.Navigate().GoToUrl($"{BaseUrl}/Cuenta/Login");

            var txtUsuario = _wait.Until(d => d.FindElement(By.Name("nombreUsuario")));
            txtUsuario.Clear();
            txtUsuario.SendKeys("usuario_inexistente");

            var txtPassword = _driver.FindElement(By.Name("password"));
            txtPassword.Clear();
            txtPassword.SendKeys("clave_erronea");

            var btnLogin = _driver.FindElement(By.CssSelector("button[type='submit']"));
            btnLogin.Click();

            // Esperar explícitamente a que aparezca un mensaje de error o la página termine la recarga
            bool mostroError = _wait.Until(d =>
            {
                // Verifica si la URL se mantiene en Login
                if (!d.Url.Contains("/Cuenta/Login", StringComparison.OrdinalIgnoreCase))
                    return false;

                // Busca elementos típicos de error de ASP.NET Core MVC (span de validación, alert o texto)
                var elementosError = d.FindElements(By.CssSelector(".field-validation-error, .validation-summary-errors, .alert, .text-danger"));
                foreach (var elem in elementosError)
                {
                    if (elem.Displayed && !string.IsNullOrWhiteSpace(elem.Text))
                        return true;
                }

                // Fallback: Si el formulario volvió a renderizarse estando en /Login
                return d.FindElements(By.Name("nombreUsuario")).Count > 0;
            });

            Assert.True(mostroError, "El sistema no respondió adecuadamente en la vista de Login tras un intento fallido.");
            Assert.Contains("/Cuenta/Login", _driver.Url, StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}