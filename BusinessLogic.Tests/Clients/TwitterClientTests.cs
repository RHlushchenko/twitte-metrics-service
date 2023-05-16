using BusinessLogic.Clients;
using Microsoft.Extensions.Options;
using Models.Configuration;
using Models.Twitter;
using Moq;
using Moq.Contrib.HttpClient;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using Xunit;

namespace BusinessLogic.Tests.Clients
{
    public class TwitterClientTests
    {
        private readonly MockRepository mockRepository;
        private readonly Mock<HttpMessageHandler> mockHttpHandler;
        private readonly HttpClient httpClient;
        private readonly Uri baseUri = new("https://api.twitter.com/", UriKind.Absolute);
        private readonly string bearerToken = "YOUR_BEARER_TOKEN";

        public TwitterClientTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);
            this.mockHttpHandler = this.mockRepository.Create<HttpMessageHandler>();
            this.httpClient = new HttpClient(this.mockHttpHandler.Object)
            {
                BaseAddress = this.baseUri
            };
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", this.bearerToken);
        }

        [Fact]
        public async Task GetTweetsStream_ShouldReturnTweets_WhenStreamIsNotEmpty()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new MemoryStream())
            };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var tweets = new List<TwitterResponse>
        {
            new TwitterResponse { Data = new MessageData { Id = "1", Text = "Hello, world!" } },
            new TwitterResponse { Data = new MessageData { Id = "2", Text = "Another tweet" } }
        };
            var serializedTweets = tweets.Select(tweet => JsonSerializer.Serialize(tweet));
            var responseStream = new MemoryStream();

            foreach (var tweet in serializedTweets)
            {
                var bytes = Encoding.UTF8.GetBytes(tweet);
                await responseStream.WriteAsync(bytes, 0, bytes.Length);
                await responseStream.WriteAsync(Encoding.UTF8.GetBytes(Environment.NewLine));
            }

            responseStream.Seek(0, SeekOrigin.Begin);
            responseMessage.Content = new StreamContent(responseStream);

            this.mockHttpHandler
                .SetupRequest(HttpMethod.Get, new Uri(this.baseUri, "2/tweets/sample/stream"))
                .ReturnsAsync(responseMessage);

            var configuration = Options.Create(new TwitterApiConfiguration
            {
                BaseUri = this.baseUri,
                BearerToken = this.bearerToken
            });

            var client = new TwitterClient(this.httpClient, configuration);

            // Act
            var result = await client.GetTweetsStream().ToListAsync();

            // Assert
            Assert.Equal(tweets.Count(), result.Count());
            Assert.Equal(result.Select(x => x.Data.Text), tweets.Select(x => x.Data.Text));
            Assert.Equal(result.Select(x => x.Data.Id), tweets.Select(x => x.Data.Id));
        }

        [Fact]
        public async Task GetTweetsStream_ShouldReturnEmptyList_WhenStreamIsEmpty()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(string.Empty)
            };

            this.mockHttpHandler
                .SetupRequest(HttpMethod.Get, new Uri(this.baseUri, "2/tweets/sample/stream"))
                .ReturnsAsync(responseMessage);

            var configuration = Options.Create(new TwitterApiConfiguration
            {
                BaseUri = this.baseUri,
                BearerToken = this.bearerToken
            });

            var client = new TwitterClient(this.httpClient, configuration);

            // Act
            var result = await client.GetTweetsStream().ToListAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTweetsStream_ShouldReturnEmptyList_WhenCancellationIsRequested()
        {
            // Arrange
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new

    StreamContent(new MemoryStream())
            };
            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            this.mockHttpHandler
                .SetupRequest(HttpMethod.Get, new Uri(this.baseUri, "2/tweets/sample/stream"))
                .ReturnsAsync(responseMessage);

            var configuration = Options.Create(new TwitterApiConfiguration
            {
                BaseUri = this.baseUri,
                BearerToken = this.bearerToken
            });

            var client = new TwitterClient(this.httpClient, configuration);

            // Act
            var result = await client.GetTweetsStream(cancellationTokenSource.Token).ToListAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}
