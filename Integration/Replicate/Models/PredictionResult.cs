using OmegaPoint.Integration.Replicate.Models.Enums;

namespace OmegaPoint.Integration.Replicate.Models
{
    public class PredictionResult<TInput>
    {
        public string Id { get; set; }

        public string Version { get; set; }

        public Urls Urls { get; set; }

        public string CreatedAt { get; set; }

        public string StartedAt { get; set; }

        public StatusType Status { get; set; }

        public TInput Input { get; set; }

        public string Error { get; set; }

        // public string Logs { get; set; }
    }

    public class PredictionResult<TInput, TOutput> : PredictionResult<TInput>
    {
        public TOutput Output { get; set; }
    }
}
