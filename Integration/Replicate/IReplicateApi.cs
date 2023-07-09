using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaPoint.Integration.Replicate.Models;

namespace OmegaPoint.Integration.Replicate
{
    public interface IReplicateApi
    {
        Task<PredictionResult<TInput>> CreatePrediction<TInput>(CreatePredictionRequest<TInput> request);
        Task<PredictionResult<TInput, TOutput>> GetPrediction<TInput, TOutput>(string id);
    }
}
