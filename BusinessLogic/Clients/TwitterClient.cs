using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;
using BusinessLogic.Interfaces;
using Microsoft.Extensions.Options;
using Models.Configuration;
using Models.Twitter;

namespace BusinessLogic.Clients
{
    public class TwitterClient : ITwitterClient
    {
        private readonly HttpClient httpClient;

        public TwitterClient(HttpClient httpClient, IOptions<TwitterApiConfiguration>? configuration)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = configuration?.Value.BaseUri;
            this.httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer",
                configuration?.Value.BearerToken);
        }

        public async IAsyncEnumerable<TwitterResponse> GetTweetsStream([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var response = await this.httpClient.GetAsync("2/tweets/sample/stream", HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var streamReader = new StreamReader(stream);

            string? rawMessage;
            do
            {
                rawMessage = await streamReader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(rawMessage))
                {
                    var message = JsonSerializer.Deserialize<TwitterResponse>(rawMessage, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (message != null)
                    {
                        yield return message;
                    }
                }
            } while (!string.IsNullOrWhiteSpace(rawMessage) && !cancellationToken.IsCancellationRequested);
        }
    }
}
