using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class CodesModel
    {
        public CodesModel()
        {
            Codes = new List<string>();
        }

        [JsonPropertyName("blockId")]
        public string BlockId { get; set; }

        [JsonPropertyName("codes")]
        public List<string> Codes { get; set; }

        [JsonPropertyName("omsId")]
        public string OmsId { get; set; }
    }
}
