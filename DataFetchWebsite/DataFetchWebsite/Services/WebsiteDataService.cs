using DataFetchWebsite.Contexts;
using DataFetchWebsite.Entities;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataFetchWebsite.Services
{
    public class WebsiteDataService : IWebsiteDataService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WebsiteDataService> _logger;

        public WebsiteDataService(IServiceProvider serviceProvider, ILogger<WebsiteDataService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task GetAllWebsiteDataAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DataFetchDbContext>();

                try
                {
                    // Fetch data from website
                    using (var httpClient = new HttpClient())
                    {
                        string url = "https://www.dsebd.org/latest_share_price_scroll_l.php"; // Replace with your target URL
                        var response = await httpClient.GetStringAsync(url);

                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(response);

                        var websiteDatas = htmlDoc.DocumentNode.SelectNodes("//div[@class='article']") // Modify the XPath to match your HTML structure
                            .Select(node => new WebsiteData
                            {
                                //Title = node.SelectSingleNode(".//h2").InnerText,
                                //Content = node.SelectSingleNode(".//p").InnerText
                            }).ToList();

                        dbContext.WebsiteDatas.AddRange(websiteDatas);
                        await dbContext.SaveChangesAsync();

                        _logger.LogInformation("Data saved to database.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while fetching website data.");
                }
            }
        }


        //private readonly IServiceProvider _serviceProvider;
        //private readonly ILogger<WebsiteDataService> _logger;
        //public WebsiteDataService(IServiceProvider serviceProvider, ILogger<WebsiteDataService> logger) 
        //{
        //    _serviceProvider = serviceProvider;
        //    _logger = logger;
        //}

        //public void GetAllWebsiteData()
        //{
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var dbContext = scope.ServiceProvider.GetRequiredService<DataFetchDbContext>();

        //        // Fetch data from website
        //        var httpClient = new HttpClient();
        //        string url = "https://www.dsebd.org/latest_share_price_scroll_l.php"; // Replace with your target URL
        //        var response = httpClient.GetStringAsync(url);

        //        var htmlDoc = new HtmlDocument();
        //        //htmlDoc.LoadHtml(response);

        //        var WebsiteDatas = htmlDoc.DocumentNode.SelectNodes("//div[@class='article']") // Modify the XPath to match your HTML structure
        //            .Select(node => new WebsiteData
        //            {
        //                //Title = node.SelectSingleNode(".//h2").InnerText,
        //                //Content = node.SelectSingleNode(".//p").InnerText
        //            }).ToList();

        //        dbContext.WebsiteDatas.AddRange(WebsiteDatas);

        //        dbContext.SaveChanges();

        //        _logger.LogInformation("Data saved to database.");
        //    }
        //}
    }
}
