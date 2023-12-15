using System;
using System.Collections.Generic;        
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using CsvHelper;
using System.IO;
using System.Text;
using System.Globalization;
using HtmlAgilityPack;
using Newtonsoft.Json;


namespace DynamicWebScraping {        
    public class Program {

        public static void Main() {
            string url = "https://www.coingecko.com/nl";
            // the URL of the target Wikipedia page
            Console.WriteLine(url);              
            Console.WriteLine("On which cryptocoin would you like today's details: ");
            string searchterm = Console.ReadLine();          
            // to initialize the Chrome Web Driver in headless mode
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            IWebDriver driver = new ChromeDriver();

            // connecting to the target web page
            driver.Navigate().GoToUrl(url);
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            IWebElement search = driver.FindElement(By.XPath(@"/html/body/header/div[3]/div[3]/div/div[4]/div[2]/div/div[1]/div[2]/div/div[1]/div[2]/input"));
            search.SendKeys(searchterm);

            IWebElement clicktoken = driver.FindElement(By.XPath(@"/html/body/header/div[3]/div[3]/div/div[4]/div[2]/div/div[1]/div[2]/div/div[3]/ul[1]/li[1]/a"));
            clicktoken.Click();

            List<Token> tokens = new();
            tokens.Add(new Token() {
                Name = driver.FindElement(By.XPath(@"/html/body/div[3]/main/div[1]/div[1]/div/div[1]/div[2]/h1/span[1]")).Text + " ",
                Price = driver.FindElement(By.XPath(@"/html/body/div[3]/main/div[1]/div[1]/div/div[1]/div[3]/div/div[1]/span[1]/span")).Text + " ",
                MarktCap = driver.FindElement(By.XPath(@"/html/body/div[3]/main/div[1]/div[1]/div/div[2]/div[2]/div[1]/div[1]/span[2]/span")).Text + " ",
                UpPercent24h = driver.FindElement(By.XPath(@"/html/body/div[3]/main/div[3]/div/div/div/div[1]/div/div[1]/div[1]/div[2]/div[2]/div[2]/span")).Text + " ",
                Rank = driver.FindElement(By.XPath(@"/html/body/div[3]/main/div[3]/div/div/div/div[1]/div/div[1]/div[2]/div[2]/div[1]/table/tbody/tr[5]/td/span")).Text + " ",
            });
            using (var writer = new StreamWriter("output.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // populating the CSV file
                csv.WriteRecords(tokens);
            }
            string jsonFilePath = "scraped_data.json";
            string jsonData = JsonConvert.SerializeObject(tokens, Formatting.Indented);
            System.IO.File.WriteAllText(jsonFilePath, jsonData);

        

        }
    }
}