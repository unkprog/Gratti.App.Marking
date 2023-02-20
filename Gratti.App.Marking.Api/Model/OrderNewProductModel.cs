using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Api.Model
{
    public class OrderNewProductModel
    {
        public enum CisTypeEnum
        {
            UNIT,
            BUNDLE,
            SET,
            GROUP
        }
        public enum SerialNumerTypeEnum
        {
            SELF_MADE,
            OPERATOR
        }

        [JsonPropertyName("cisType")]
        public CisTypeEnum CisType { get; set; }

        [JsonPropertyName("exporterTaxpayerId")]
        public string ExporterTaxpayerId { get; set; }

        [JsonPropertyName("gtin")]
        public string Gtin { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("serialNumberType")]
        public SerialNumerTypeEnum SerialNumberType { get; set; }

        [JsonPropertyName("serialNumbers")]
        public List<string> SerialNumbers { get; set; }

        [JsonPropertyName("templateId")]
        public int TemplateId { get; set; }
    }
}
