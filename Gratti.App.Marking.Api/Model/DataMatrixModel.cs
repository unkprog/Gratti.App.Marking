using System.Reflection;
using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class DataMatrixModel
    {

        public DataMatrixModel()
        {

        }

        public DataMatrixModel(string cisTrue)
        {
            if (string.IsNullOrEmpty(cisTrue))
                return;

            CisTrue = cisTrue;
            int char29 = CisTrue.IndexOf((char)29);
            Cis = Uit = (char29 > -1 ? CisTrue.Substring(0, char29) : CisTrue);
            Gtin = Cis.Substring(2, 14);
            Sgtin = Cis.Substring(18, 13);

        }


        [JsonPropertyName("cis")]
        public string Cis { get; set; }
        [JsonPropertyName("gtin")]
        public string Gtin { get; set; }

        [JsonPropertyName("sgtin")]
        public string Sgtin { get; set; }
        [JsonPropertyName("uit")]
        public string Uit { get; set; }
        [JsonPropertyName("cistrue")]
        public string CisTrue { get; set; }

        [JsonPropertyName("productGroup")]
        public string ProductGroup { get; set; }

        [JsonPropertyName("barcode")]
        public string Barcode { get; set; }
    }
}
