using DataFetchWebsite.Contexts;
using DataFetchWebsite.Entities;
using DataFetchWebsite.Services;
using HtmlAgilityPack;
using System.Net.Http;

namespace DataFetchWebsite
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;    
        private readonly DataFetchDbContext _dataFetchDbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public Worker(ILogger<Worker> logger, DataFetchDbContext dataFetchDbContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _dataFetchDbContext = dataFetchDbContext;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    _logger.LogInformation("Starting to fetch data from the website.");

                    var httpClient = _httpClientFactory.CreateClient();
                    string url = "https://www.dsebd.org/latest_share_price_scroll_l.php"; // Replace with your target URL
                    var response = await httpClient.GetStringAsync(url);

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response);

                    var rows = htmlDoc.DocumentNode.SelectNodes("//table[@class='table table-bordered background-white shares-table fixedHeader']/tbody/tr");

                    if (rows != null)
                    {
                        var websiteDatas = rows.Select(row => new WebsiteData
                        {
                            TradingCode = row.SelectSingleNode("td[2]/a")?.InnerText.Trim(),
                            LTP = row.SelectSingleNode("td[3]")?.InnerText.Trim(),
                            High = row.SelectSingleNode("td[4]")?.InnerText.Trim(),
                            Low = row.SelectSingleNode("td[5]")?.InnerText.Trim(),
                            ClosePrice = row.SelectSingleNode("td[6]")?.InnerText.Trim(),
                            YCP = row.SelectSingleNode("td[7]")?.InnerText.Trim(),
                            Change = row.SelectSingleNode("td[8]")?.InnerText.Trim(),
                            Trade = row.SelectSingleNode("td[9]")?.InnerText.Trim(),
                            Value = row.SelectSingleNode("td[10]")?.InnerText.Trim(),
                            Volume = row.SelectSingleNode("td[11]")?.InnerText.Trim()
                        }).ToList();

                        _dataFetchDbContext.WebsiteDatas.AddRange(websiteDatas);
                        await _dataFetchDbContext.SaveChangesAsync();

                        _logger.LogInformation("Data saved to database.");

                    }
                    else
                    {
                        _logger.LogWarning("No data found to save.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while fetching website data.");
                }
                //try
                //{
                //    await _websiteDataService.GetAllWebsiteDataAsync();
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, "An error occurred in Worker.");
                //}
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
