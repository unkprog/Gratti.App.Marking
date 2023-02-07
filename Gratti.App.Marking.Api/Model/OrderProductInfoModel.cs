using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class OrderProductInfoModel
    {
        [Key]
        public string GTIN { get; set; }

        [JsonPropertyName("fat")]
        public string Fat { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("rawOrigin")]
        public string RawOrigin { get; set; }

        [JsonPropertyName("structure")]
        public string Structure { get; set; }

        [JsonPropertyName("tnVedCode")]
        public string TnVedCode { get; set; }

        [JsonPropertyName("packageType")]
        public string PackageType { get; set; }

        [JsonPropertyName("tnVedCode10")]
        public string TnVedCode10 { get; set; }

        [JsonPropertyName("packMaterial")]
        public string PackMaterial { get; set; }

        [JsonPropertyName("paymentGroup")]
        public int PaymentGroup { get; set; }

        [JsonPropertyName("productGroup")]
        public int ProductGroup { get; set; }

        [JsonPropertyName("volumeWeight")]
        public string VolumeWeight { get; set; }

        [JsonPropertyName("babyFoodProduct")]
        public string BabyFoodProduct { get; set; }

        [JsonPropertyName("milkProductType")]
        public string MilkProductType { get; set; }

        [JsonPropertyName("isShelfLife40Days")]
        public string IsShelfLife40Days { get; set; }

        [JsonPropertyName("veterinaryControl")]
        public string VeterinaryControl { get; set; }

        [JsonPropertyName("isSpecializedFoodProduct")]
        public string IsSpecializedFoodProduct { get; set; }

#region Calculate properties
        public string ProductGroupText
        {
            get
            {
                switch(ProductGroup)
                {
                    case 1: return Constants.group_lp;
                    case 2: return Constants.group_shoes;
                    case 4: return Constants.group_perfum;
                    case 5: return Constants.group_tires;
                    case 6: return Constants.group_photo;
                    case 8: return Constants.group_milk;
                    case 13: return Constants.group_water;
                    case 15: return Constants.group_beer;
                    default: return string.Empty;
                };
            }
        }
#endregion
    }
}
