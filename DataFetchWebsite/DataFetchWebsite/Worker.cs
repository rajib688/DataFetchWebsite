using DataFetchWebsite.Entities;
using DataFetchWebsite.Services;

namespace DataFetchWebsite
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWebsiteDataService _websiteDataService;

        public Worker(ILogger<Worker> logger, IWebsiteDataService WebsiteDataService)
        {
            _logger = logger;
            _websiteDataService = WebsiteDataService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                try
                {
                    await _websiteDataService.GetAllWebsiteDataAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in Worker.");
                }
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
