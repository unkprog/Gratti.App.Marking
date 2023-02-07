using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class OrdersModel
    {
        public OrdersModel()
        {
            Orders = new List<OrderInfoModel>();
        }

        [JsonPropertyName("omsId")]
        public string OmsId { get; set; }

        [JsonPropertyName("orderInfos")]
        public List<OrderInfoModel> Orders { get; set; }
    }
}
