using DataGet.Contexts;
using DataGet.Entities;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace DataGet
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DataGetDbContext _dataGetDbContext;
        private readonly IHttpClientFactory _httpClientFactory;

        public Worker(ILogger<Worker> logger, DataGetDbContext dataGetDbContext, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _dataGetDbContext = dataGetDbContext;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting to fetch data from the website.");

                    using (var httpClient = _httpClientFactory.CreateClient())
                    {
                        string url = "https://www.dsebd.org/latest_share_price_scroll_l.php";
                        var response = await httpClient.GetStringAsync(url);

                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(response);

                        var tableNode = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='table table-bordered background-white shares-table fixedHeader']");

                        var rows = tableNode.SelectNodes(".//tr").ToList();

                        if (rows != null)
                        {
                            foreach (var row in rows)
                            {

                                var tradingCodeNode = row.SelectSingleNode("td[2]/a");
                                if (tradingCodeNode == null)
                                {
                                    _logger.LogWarning("Trading code not found for a row. Skipping.");
                                    continue; 
                                }

                                var websiteData = new WebsiteData
                                {
                                    TradingCode = tradingCodeNode.InnerText.Trim(),
                                    LTP = row.SelectSingleNode("td[3]")?.InnerText.Trim(),
                                    High = row.SelectSingleNode("td[4]")?.InnerText.Trim(),
                                    Low = row.SelectSingleNode("td[5]")?.InnerText.Trim(),
                                    ClosePrice = row.SelectSingleNode("td[6]")?.InnerText.Trim(),
                                    YCP = row.SelectSingleNode("td[7]")?.InnerText.Trim(),
                                    Change = row.SelectSingleNode("td[8]")?.InnerText.Trim(),
                                    Trade = row.SelectSingleNode("td[9]")?.InnerText.Trim(),
                                    Value = row.SelectSingleNode("td[10]")?.InnerText.Trim(),
                                    Volume = row.SelectSingleNode("td[11]")?.InnerText.Trim()
                                };

                                _dataGetDbContext.websiteDatas.Add(websiteData);
                                _logger.LogInformation("Data added to DbContext for Trading Code: {TradingCode}", websiteData.TradingCode);
                            }

                            await _dataGetDbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation("Saved all data to the database.");
                        }
                        else
                        {
                            _logger.LogWarning("No rows found in the table.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while fetching or saving data.");
                }

                await Task.Delay(3000, stoppingToken); 
            }
        }
    }
}
