namespace OmegaPoint.Integration.Replicate.Models
{
    public class CaptionRequest
    {
        public string Image { get; set; }

        public string Prompt { get; set; }

        public double Temperature { get; set; }
    }
}