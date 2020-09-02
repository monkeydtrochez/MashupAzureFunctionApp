using Microsoft.Azure.Cosmos.Table;

namespace MashupAzureFunctionApp.Models
{
    public class MashupEntityModel : TableEntity
    {
        public string JsonModel { get; set; }
    }
}
