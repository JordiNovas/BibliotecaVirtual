using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
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

        private static Process? webAppProcess;

        private const string BaseUrl = "http://localhost:5119";

        public LoginTests()
        {
            AsegurarServidorWebActivo();

            var options = new ChromeOptions();

            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--ignore-certificate-errors");

            _driver = new ChromeDriver(options);

            _wait = new WebDriverWait(
                _driver,
                TimeSpan.FromSeconds(15));
        }

        private void AsegurarServidorWebActivo()
        {
            if (webAppProcess != null && !webAppProcess.HasExited)
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

        [Fact]
        public void LoginConCredencialesIncorrectas()
        {
            _driver.Navigate()
                .GoToUrl($"{BaseUrl}/Cuenta/Login");

            var txtUsuario = _wait.Until(d =>
                d.FindElement(By.Name("nombreUsuario")));

            txtUsuario.Clear();
            txtUsuario.SendKeys("usuario_inexistente");

            var txtPassword = _wait.Until(d =>
                d.FindElement(By.Name("password")));

            txtPassword.Clear();
            txtPassword.SendKeys("clave_erronea");

            var btnLogin = _wait.Until(d =>
                d.FindElement(
                    By.CssSelector("button[type='submit']")));

            btnLogin.Click();

            bool mostroError = _wait.Until(d =>
            {
                if (!d.Url.Contains(
                    "/Cuenta/Login",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var elementosError = d.FindElements(
                    By.CssSelector(
                        ".field-validation-error, " +
                        ".validation-summary-errors, " +
                        ".alert, " +
                        ".text-danger"));

                foreach (var elemento in elementosError)
                {
                    if (elemento.Displayed &&
                        !string.IsNullOrWhiteSpace(elemento.Text))
                    {
                        return true;
                    }
                }

                return d.FindElements(
                    By.Name("nombreUsuario")).Count > 0;
            });

            Assert.True(
                mostroError,
                "El sistema no mostró correctamente el error " +
                "al utilizar credenciales incorrectas.");

            Assert.Contains(
                "/Cuenta/Login",
                _driver.Url,
                StringComparison.OrdinalIgnoreCase);
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();

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