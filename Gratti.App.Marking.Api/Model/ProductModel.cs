using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gratti.App.Marking.Api.Model
{
    public class ProductModel
    {
        [JsonPropertyName("good_id")]
        public string Id { get; set; }
        [JsonPropertyName("good_name")]
        public string Name { get; set; }
        [JsonPropertyName("brand_name")]
        public string Brand { get; set; }

        [JsonPropertyName("identified_by")]
        public ProductIdentifiedModel[] IdentifiedBy { get; set; }


        [JsonPropertyName("good_attrs")]
        public ProductAttributeModel[] Attrs { get; set; }

    }

    public class ProductIdentifiedModel
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("type")]
        public string ValueType { get; set; }
        [JsonPropertyName("multiplier")]
        public string Multiplier { get; set; }
        [JsonPropertyName("level")]
        public string Level { get; set; }


    }

    public class ProductAttributeModel
    {
        [JsonPropertyName("attr_id")]
        public string Id { get; set; }
        [JsonPropertyName("attr_name")]
        public string Name { get; set; }
        [JsonPropertyName("attr_value_id")]
        public string Value_Id { get; set; }
        [JsonPropertyName("attr_value")]
        public string Value { get; set; }
        [JsonPropertyName("attr_value_type")]
        public string ValueType { get; set; }


    }

}
