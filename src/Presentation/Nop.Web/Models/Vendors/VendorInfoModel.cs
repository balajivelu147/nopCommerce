using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Vendors
{
    public record VendorInfoModel : BaseNopModel
    {
        public VendorInfoModel()
        {
            VendorAttributes = new List<VendorAttributeModel>();
        }

        [NopResourceDisplayName("Account.VendorInfo.Name")]
        public string Name { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Account.VendorInfo.Email")]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Picture")]
        public string PictureUrl { get; set; }

        public IList<VendorAttributeModel> VendorAttributes { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Longitude")]

        public decimal Longitude { get; set; }
        
        [NopResourceDisplayName("Account.VendorInfo.Latitude")]
        public decimal Latitude { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.IsWholeCitySupply")]
        public bool IsWholeCitySupply { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.Citys")]
        public string Citys { get; set; }
        [NopResourceDisplayName("Account.VendorInfo.CityFromPrice")] 
        public decimal CityFromPrice { get; set; }
        [NopResourceDisplayName("Account.VendorInfo.CityUptoPrice")]
        public decimal CityUptoPrice { get; set; }

        [NopResourceDisplayName("Account.VendorInfo.CityFromWeight")] 
        public decimal CityFromWeight { get; set; }
        [NopResourceDisplayName("Account.VendorInfo.CityUptoWeight")] 
        public decimal CityUptoWeight { get; set; }
       
        [NopResourceDisplayName("Account.VendorInfo.ProductTemplateTypes")]
        public string ProductTemplateTypes { get; set; }


        [NopResourceDisplayName("Account.VendorInfo.ZipCodes")] 
        public string ZipCodes { get; set; }
    }
}