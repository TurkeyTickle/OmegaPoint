using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OmegaPoint.Core.Json;
using OmegaPoint.Integration.Replicate.Models;

namespace OmegaPoint.Integration.Replicate
{
    public class ReplicateApi : IReplicateApi
    {
        private readonly HttpClient _client;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = new SnakeCaseJsonNamingPolicy(),
            Converters = { new JsonStringEnumConverter() },
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public ReplicateApi(ReplicateSettings settings)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(settings.ApiBaseUrl)
            };
            _client.DefaultRequestHeaders.Add("Authorization", "Token " + settings.ApiToken);
        }

        public async Task<PredictionResult<T>> CreatePrediction<T>(CreatePredictionRequest<T> request)
        {
            PredictionResult<T> result = null;

            var response = await _client.PostAsJsonAsync("/v1/predictions", request, _jsonSerializerOptions);

            if (response.IsSuccessStatusCode)
            {
                var resultString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(resultString))
                {
                    Debug.WriteLine(resultString);

                    result = JsonSerializer.Deserialize<PredictionResult<T>>(resultString, _jsonSerializerOptions);
                }
            }

            return result;
        }

        public async Task<PredictionResult<TInput, TOutput>> GetPrediction<TInput, TOutput>(string id)
        {
            // var response = await _client.GetFromJsonAsync<PredictionResult<TInput, TOutput>>($"/v1/predictions/{id}", _jsonSerializerOptions);
            var response = await _client.GetStringAsync($"/v1/predictions/{id}");
            var test = response.Replace(@"\\n", string.Empty);
            var result = JsonSerializer.Deserialize<PredictionResult<TInput, TOutput>>(test, _jsonSerializerOptions);
            return result;
            // return response;
        }
    }
}
