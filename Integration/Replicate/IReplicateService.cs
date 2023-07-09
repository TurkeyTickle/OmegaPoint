using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaPoint.Integration.Replicate
{
    public interface IReplicateService
    {
        Task<(string Id, TOutput Output)?> GetPrediction<TInput, TOutput>(string versionId, TInput input);
    }
}
