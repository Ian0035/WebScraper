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
            Console.WriteLine("On what would you like to search? : ");
            string searchterm = Console.ReadLine();
            string replaced = searchterm.Replace(" ", "+");
            string search = "https://www.youtube.com/results?search_query=" + replaced + "&sp=CAI%253D";
            // the URL of the target Wikipedia page
            string url = search;
            Console.WriteLine(url);
                        
            // to initialize the Chrome Web Driver in headless mode
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            IWebDriver driver = new ChromeDriver();

            // connecting to the target web page
            driver.Navigate().GoToUrl(url);
           

            IWebElement node = driver.FindElement(By.XPath(@"/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]"));
            // var nodes = driver.FindElements(By.XPath(@"//*[@id='contents']"));            
            // selecting the HTML nodes of interest 
    
            List<Video> videos = new();           
            int j =  1;
            while (j <= 5) {  
                videos.Add(new Video() {
                    Link = node.FindElement(By.XPath(@"/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + j + "]/div[1]/div/div[1]/div/h3/a")).GetAttribute("href") + " ",
                    Title = node.FindElement(By.XPath(@"/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + j + "]/div[1]/div/div[1]/div/h3/a/yt-formatted-string")).Text + " ",
                    Uploader = node.FindElement(By.XPath(@"/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + j + "]/div[1]/div/div[2]/ytd-channel-name/div/div/yt-formatted-string/a")).Text + " ",
                    Views =  node.FindElement(By.XPath(@"/html/body/ytd-app/div[1]/ytd-page-manager/ytd-search/div[1]/ytd-two-column-search-results-renderer/div/ytd-section-list-renderer/div[2]/ytd-item-section-renderer/div[3]/ytd-video-renderer[" + j + "]/div[1]/div/div[1]/ytd-video-meta-block/div[1]/div[2]/span[1]")).Text,
               });
               j++;
            }
            using (var writer = new StreamWriter("output.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                // populating the CSV file
                csv.WriteRecords(videos);
            }
            string jsonFilePath = "scraped_data.json";
            string jsonData = JsonConvert.SerializeObject(videos, Formatting.Indented);
            System.IO.File.WriteAllText(jsonFilePath, jsonData);

        }
    }
}
