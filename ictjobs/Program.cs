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
            Console.WriteLine("On what jobs would you like to search? : ");
            string searchterm = Console.ReadLine();
            string search = "https://www.ictjob.be/nl/it-vacatures-zoeken?keywords=" + searchterm;
            // the URL of the target Wikipedia page
            string url = search;
            Console.WriteLine(url);
                        
            // to initialize the Chrome Web Driver in headless mode
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            IWebDriver driver = new ChromeDriver();

            // connecting to the target web page
            driver.Navigate().GoToUrl(url);
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            IWebElement cookie = driver.FindElement(By.XPath(@"/html/body/div[2]/div/div/div/div[2]/a"));
            cookie.Click();

            IWebElement datum = driver.FindElement(By.XPath(@"/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[1]/div[2]/div/div[2]/span[2]/a"));
            datum.Click();
            Console.WriteLine("Before sleep");
            System.Threading.Thread.Sleep(11000);
            Console.WriteLine("After sleep");
            IWebElement node = driver.FindElement(By.XPath(@"/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul"));
            
            List<Job> jobs = new();           
            int j =  1;
            while (j <= 6) { 
                if (j == 4) {
                    j++;
                }
                jobs.Add(new Job() {
                    Title = node.FindElement(By.XPath(@"/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + j + "]/span[2]/a/h2")).Text + " ",
                    Company = node.FindElement(By.XPath(@"/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + j + "]/span[2]/span[1]")).Text + " ",
                    Location = node.FindElement(By.XPath(@"/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + j + "]/span[2]/span[2]/span[2]/span/span")).Text + " ",
                    Keywords = node.FindElement(By.XPath(@"/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + j + "]/span[2]/span[3]")).Text + " ",
                    Link = node.FindElement(By.XPath(@"/html/body/section/div[1]/div/div[2]/div/div/form/div[2]/div/div/div[2]/section/div/div[2]/div[1]/div/ul/li[" + j + "]/span[2]/a")).GetAttribute("href"),
                });
            j++;
            }
            using (var writer = new StreamWriter("output.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // populating the CSV file
                csv.WriteRecords(jobs);
            }
            string jsonFilePath = "scraped_data.json";
            string jsonData = JsonConvert.SerializeObject(jobs, Formatting.Indented);
            System.IO.File.WriteAllText(jsonFilePath, jsonData);

                
        }
    }
}