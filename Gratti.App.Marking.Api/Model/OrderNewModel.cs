using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Gratti.App.Marking.Api.Model
{
    public class OrderNewModel 
    {
        public enum ReleaseMethodTypeEnum
        {

            [Display(Name = Constants.releasemethodtype_production)]
            PRODUCTION,
            [Display(Name = Constants.releasemethodtype_import)]
            IMPORT,
            [Display(Name = Constants.releasemethodtype_remains)]
            REMAINS,
            [Display(Name = Constants.releasemethodtype_crossborder)]
            CROSSBORDER,
            [Display(Name = Constants.releasemethodtype_remark)]
            REMARK,
            [Display(Name = Constants.releasemethodtype_comission)]
            COMMISSION,
            [Display(Name = Constants.releasemethodtype_contract_production)]
            CONTRACT_PRODUCTION
        }

        public enum CreateMethodTypeEnum
        {
            [Display(Name = Constants.createmethodtype_self_made)]
            SELF_MADE,
            [Display(Name = Constants.createmethodtype_cem)]
            CEM,
            [Display(Name = Constants.createmethodtype_cm)]
            CM,
            [Display(Name = Constants.createmethodtype_cl)]
            CL,
            [Display(Name = Constants.createmethodtype_ca)]
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
