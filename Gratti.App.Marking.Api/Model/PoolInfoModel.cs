using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class PoolInfoModel
    {
        public enum StatusEnum
        {
            ER_NOT_AVAILABLE,
            REQUEST_ERROR,
            REQUESTED,
            IN_PROCESS,
            READY,
            CLOSED,
            DELETED,
            REJECTED
        }

        [JsonPropertyName("isRegistrarReady")]
        public bool IsRegistrarReady { get; set; }

        [JsonPropertyName("lastRegistrarErrorTimestamp")]
        public long LastRegistrarErrorTimestamp { get; set; }

        [JsonPropertyName("leftInRegistrar")]
        public int LeftInRegistrar { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("registrarErrorCount")]
        public int RegistrarErrorCount { get; set; }

        [JsonPropertyName("registrarId")]
        public string RegistrarId { get; set; }

        [JsonPropertyName("rejectionReason")]
        public string RejectionReason { get; set; }

        //[JsonPropertyName("status")]
        //[JsonConverter(typeof(StatusEnum))]
        [JsonIgnore]
        public StatusEnum Status { get; set; }

        [JsonPropertyName("status")]
        public string StatusStr { get; set; }
    }
}
