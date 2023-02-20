using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class OrderNewModel 
    {
        public enum ReleaseMethodTypeEnum
        {
            PRODUCTION,
            IMPORT,
            REMAINS,
            CROSSBORDER,
            REMARK,
            COMMISSION,
            CONTRACT_PRODUCTION
        }

        public enum CreateMethodTypeEnum
        {
            SELF_MADE,
            CEM,
            CM,
            CL,
            CA
        }

        public OrderNewModel()
        {
            Products = new List<OrderNewProductModel>();
        }

        [JsonPropertyName("contactPerson")]
        public string ContactPerson { get; set; }

        [JsonPropertyName("releaseMethodType")]
        public ReleaseMethodTypeEnum ReleaseMethodType { get; set; }

        [JsonPropertyName("createMethodType")]
        public CreateMethodTypeEnum CreateMethodType { get; set; }

        [JsonPropertyName("productionOrderId")]
        public string ProductionOrderId { get; set; }

        [JsonPropertyName("serviceProviderId")]
        public string ServiceProviderId { get; set; }

        [JsonPropertyName("products")]
        public List<OrderNewProductModel> Products { get; set; }
    }

    
   
}
