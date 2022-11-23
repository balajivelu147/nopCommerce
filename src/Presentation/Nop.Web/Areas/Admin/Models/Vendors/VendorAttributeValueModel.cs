using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Vendors
{
    /// <summary>
    /// Represents a vendor attribute value model
    /// </summary>
    public partial record VendorAttributeValueModel : BaseNopEntityModel, ILocalizedModel<VendorAttributeValueLocalizedModel>
    {
        #region Ctor

        public VendorAttributeValueModel()
        {
            Locales = new List<VendorAttributeValueLocalizedModel>();
            AvailableAttributes = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public int VendorAttributeId { get; set; }

        //TODO:typo: should be "DependantAttributeValueId"
        public int DependentAttributeValueId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

        public IList<VendorAttributeValueLocalizedModel> Locales { get; set; }
        public IList<SelectListItem> AvailableAttributes { get; set; }

        #endregion
    }

    public partial record VendorAttributeValueLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Vendors.VendorAttributes.Values.Fields.Name")]
        public string Name { get; set; }
    }
}