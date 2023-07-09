using OmegaPoint.Integration.Replicate.Models;
using OmegaPoint.Integration.Replicate.Models.Enums;

namespace OmegaPoint.Integration.Replicate
{
    public class ReplicateService : IReplicateService
    {
        private readonly HttpClient _httpClient;
        private readonly IReplicateApi _replicateApi;

        public ReplicateService(HttpClient httpClient, IReplicateApi replicateApi)
        {
            _httpClient = httpClient;
            _replicateApi = replicateApi;
        }

        public async Task<(string Id, TOutput Output)?> GetPrediction<TInput, TOutput>(string versionId, TInput input)
        {
            var request = new CreatePredictionRequest<TInput>()
            {
                Version = versionId,
                Input = input
            };

            var result = await _replicateApi.CreatePrediction<TInput>(request);

            if (result.Status != StatusType.Failed &&
                result.Status != StatusType.Cancelled)
            {
                while (result.Status != StatusType.Failed &&
                       result.Status != StatusType.Cancelled &&
                       result.Status != StatusType.Succeeded)
                {
                    await Task.Delay(1000);
                    var pollResult = await _replicateApi.GetPrediction<TInput, TOutput>(result.Id);

                    if (pollResult != null && pollResult.Status == StatusType.Succeeded && pollResult.Output != null)
                    {
                        // var response = await _httpClient.GetStreamAsync(result.Output.First());
                        return (result.Id, pollResult.Output);
                    }
                }
            }

            return null;
        }
    }
}
