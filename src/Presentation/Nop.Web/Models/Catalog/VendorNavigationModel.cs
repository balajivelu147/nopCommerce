using System;
using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Vendors;

namespace Nop.Web.Models.Catalog
{
    public partial record VendorNavigationModel : BaseNopModel
    {
        public VendorNavigationModel()
        {
            Vendors = new List<VendorBriefInfoModel>();
        }

        public IList<VendorBriefInfoModel> Vendors { get; set; }

        public int TotalVendors { get; set; }
    }

    public partial record VendorBriefInfoModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string SeName { get; set; }

        public  decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public IList<VendorAttributeModel> VendorAttributes { get; set;}

}
}