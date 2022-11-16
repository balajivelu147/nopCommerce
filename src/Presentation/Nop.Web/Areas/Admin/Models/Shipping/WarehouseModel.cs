using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a warehouse model
    /// </summary>
    public partial record WarehouseModel : BaseNopEntityModel
    {
        #region Ctor

        public WarehouseModel()
        {
            Address = new AddressModel();
            AvailableVendors = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.AdminComment")]
        public string AdminComment { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Shipping.Warehouses.Fields.Address")]
        public AddressModel Address { get; set; }


        [NopResourceDisplayName("Admin.Catalog.Products.Fields.Vendor")]
        public int VendorId { get; set; }
        public IList<SelectListItem> AvailableVendors { get; set; }

        #endregion
    }
}