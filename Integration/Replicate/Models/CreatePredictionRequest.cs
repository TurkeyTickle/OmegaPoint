using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaPoint.Integration.Replicate.Models
{
    public class CreatePredictionRequest<T>
    {
        public string Version { get; set; }

        public T Input { get; set; }
    }
}
