using System.Collections.Generic;

namespace MashupAzureFunctionApp.Models
{
    public class MashupResultModel
    {
        public string MbId { get; set; }
        public string Name { get; set; }
        public string MusicType { get; set; }
        public string Country { get; set; }
        public LifeSpan LifeSpan { get; set; }
        public string Description { get; set; }

        public List<ReleaseGroup> Albums { get; set; }
    }
}
