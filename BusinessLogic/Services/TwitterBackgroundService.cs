using BusinessLogic.Interfaces;
using DataAccess.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models.Twitter;

namespace BusinessLogic.Services
{
    public class TwitterBackgroundService : BackgroundService
    {
        private readonly ITwitterClient twitterClient;
        private readonly IRepository<MessageData> repository;
        private readonly ILogger<TwitterBackgroundService> logger;

        public TwitterBackgroundService(ITwitterClient twitterClient, IRepository<MessageData> repository, ILogger<TwitterBackgroundService> logger)
        {
            this.twitterClient = twitterClient;
            this.repository = repository;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                this.logger.LogInformation("TwitterBackgroundService started.");

                int messagesCount = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    await foreach (var twitterResponse in this.twitterClient.GetTweetsStream(stoppingToken))
                    {
                        if (twitterResponse.Data != null)
                        {
                            this.repository.Add(twitterResponse.Data);

                            this.logger.LogInformation($"Message with Id={twitterResponse.Data.Id} was pulled from twitter stream.");
                            messagesCount++;
                        }
                        else
                        {
                            this.logger.LogWarning("Pulled message data is invadid.");
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }

                this.logger.LogInformation($"TwitterBackgroundService completed. Pulled {messagesCount} tweets.");
            }
            catch (TaskCanceledException)
            {
                this.logger.LogWarning("TwitterBackgroundService canceled.");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "TwitterBackgroundService failed.");
                throw;
            }
        }
    }
}
