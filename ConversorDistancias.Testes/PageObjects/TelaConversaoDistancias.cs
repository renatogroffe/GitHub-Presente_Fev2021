using System;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ConversorDistancias.Testes.Utils;

namespace ConversorDistancias.Testes.PageObjects
{
    public class TelaConversaoDistancias
    {
        private readonly IConfiguration _configuration;
        private readonly string _cenario;
        private ChromeDriver _driver;

        public TelaConversaoDistancias(
            IConfiguration configuration,
            string cenario)
        {
            _configuration = configuration;
            _cenario = cenario;

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");

            chromeOptions.SetLoggingPreference(LogType.Browser, LogLevel.Off);
            chromeOptions.SetLoggingPreference(LogType.Driver, LogLevel.Off);

            if (!String.IsNullOrWhiteSpace(_configuration["CaminhoDriverChrome"]))
                _driver = new ChromeDriver(_configuration["CaminhoDriverChrome"], chromeOptions);
            else
                _driver = new ChromeDriver(chromeOptions);
        }

        public void CarregarPagina()
        {
            _driver.LoadPage(
                TimeSpan.FromSeconds(Convert.ToInt32(_configuration["Timeout"])),
                _configuration["UrlTelaConversaoDistancias"]);
        }

        public void PreencherDistanciaMilhas(double valor)
        {
            _driver.SetText(
                By.Name("DistanciaMilhas"),
                valor.ToString());
        }

        public void ProcessarConversao()
        {
            _driver.Submit(By.Id("btnConverter"));

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until((d) => d.FindElement(By.Id("DistanciaKm")) != null);
        }

        public double ObterDistanciaKm()
        {
            return Convert.ToDouble(
                _driver.GetText(By.Id("DistanciaKm")));
        }

        public void GerarScreenshot()
        {
            _driver.GetScreenshot().SaveAsFile(
                _configuration["CaminhoScreenshots"] +
                $"{_cenario}_{DateTime.Now:yyyyMMddHHmmss}.png",
                ScreenshotImageFormat.Png);
        }

        public void Fechar()
        {
            _driver.Quit();
            _driver = null;
        }
    }
}