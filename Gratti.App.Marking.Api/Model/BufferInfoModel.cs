using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Gratti.App.Marking.Core.Extensions;

namespace Gratti.App.Marking.Api.Model
{
    public class BufferInfoModel
    {
        public enum BufferStatusEnum
        {
            [Display(Name = "В ожидании")]
            PENDING,
            [Display(Name = "Готов к печати")]
            ACTIVE,
            [Display(Name = "Исчерпан")]
            EXHAUSTED,
            [Display(Name = "Не доступен")]
            REJECTED,
            [Display(Name = "Закрыт")]
            CLOSED
        }

        public BufferInfoModel()
        {
            PoolInfos = new List<PoolInfoModel>();
        }

        [JsonPropertyName("omsId")]
        public string OmsId { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("availableCodes")]
        public int AvailableCodes { get; set; }

        [JsonPropertyName("bufferStatus")]
        public BufferStatusEnum BufferStatus { get; set; }

        [JsonPropertyName("expiredDate")]
        public long ExpiredDate { get; set; }

        [JsonPropertyName("gtin")]
        public string Gtin { get; set; }

        [JsonPropertyName("leftInBuffer")]
        public int LeftInBuffer { get; set; }

        [JsonPropertyName("poolInfos")]
        public List<PoolInfoModel> PoolInfos { get; set; }


        [JsonPropertyName("poolsExhausted")]
        public bool PoolsExhausted { get; set; }


        [JsonPropertyName("productionOrderId")]
        public string ProductionOrderId { get; set; }


        [JsonPropertyName("rejectionReason")]
        public string RejectionReason { get; set; }


        [JsonPropertyName("totalCodes")]
        public int TotalCodes { get; set; }


        [JsonPropertyName("totalPassed")]
        public int TotalPassed { get; set; }


        [JsonPropertyName("unavailableCodes")]
        public int UnavailableCodes { get; set; }

#region Calculate properties
        public DateTime ExpiredDateTime
        {
            get
            {
                return (ExpiredDate == 0 ? DateTime.MinValue : new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddMilliseconds(Convert.ToInt64(ExpiredDate)));
            }
        }

        [JsonIgnore]
        public OrderProductInfoModel ProductInfo { get; set; }

        [JsonIgnore]
        public string FullName => ProductInfo?.FullName;

        [JsonIgnore]
        public string BufferStatusText => BufferStatus.GetDisplayName();
#endregion
    }
}
