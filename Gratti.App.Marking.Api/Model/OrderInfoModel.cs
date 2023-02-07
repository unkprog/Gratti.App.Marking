using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using Gratti.App.Marking.Core.Extensions;

namespace Gratti.App.Marking.Api.Model
{
    public class OrderInfoModel
    {
        public enum OrderStatusEnum
        {
            [Display(Name = "Заказ создан")]
            CREATED,
            [Display(Name = "Ожидает подтверждения ГИС МТ")]
            PENDING,
            [Display(Name = "Не подтверждён в ГИС МТ")]
            DECLINED,
            [Display(Name = "Подтверждён в ГИС МТ")]
            APPROVED,
            [Display(Name = "Готов к печати")]
            READY,
            [Display(Name = "Срок заказа истек")]
            EXPIRED,
            [Display(Name = "Заказ закрыт")]
            CLOSED
        }

        public OrderInfoModel()
        {
            Buffers = new List<BufferInfoModel>();
        }

        [JsonPropertyName("createdTimestamp")]
        public long СreatedTimestamp { get; set; }

        [JsonPropertyName("declineReason")]
        public string DeclineReason { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("orderStatus")]
        public OrderStatusEnum OrderStatus { get; set; }

        [JsonPropertyName("buffers")]
        public List<BufferInfoModel> Buffers { get; set; }

        [JsonPropertyName("productionOrderId")]
        public string ProductionOrderId { get; set; }

#region Calculate properties

        [JsonIgnore]
        public DateTime CreatedDateTime => new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddMilliseconds(Convert.ToInt64(СreatedTimestamp));

        [JsonIgnore]
        public int TotalBuffers
        {
            get
            {
                return (Buffers == null ? 0 : Buffers.Count);
            }
        }

        [JsonIgnore]
        public int TotalCodes
        {
            get
            {
                return (Buffers == null ? 0 : Buffers.Sum((BufferInfoModel x) => x.TotalCodes));
            }
        }

        [JsonIgnore]
        public int TotalAvailableCodes
        {
            get
            {
                return (Buffers == null ? 0 : Buffers.Sum((BufferInfoModel x) => x.AvailableCodes));
            }
        }

        [JsonIgnore]
        public string OrderStatusText => OrderStatus.GetDisplayName();

#endregion
    }
}
