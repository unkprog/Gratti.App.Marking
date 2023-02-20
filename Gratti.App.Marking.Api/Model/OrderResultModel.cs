using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class OrderResultModel
    {
        [JsonPropertyName("expectedCompleteTimestamp")]
        public long ExpectedCompleteTimestamp { get; set; }

        [JsonPropertyName("omsId")]
        public string OmsId { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }
    }
}
