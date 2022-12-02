﻿using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents a vendor attribute
    /// </summary>
    public partial class VendorAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attribute is required
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the attribute control type identifier
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Gets or sets the AttributeGroup
        /// </summary>
        public string AttributeGroup { get; set; }

        /// <summary>
        /// Gets or sets the DependentAttributeId
        /// </summary>
        public int? DependentAttributeId { get; set; }


        /// <summary>
        /// Gets or sets the DependencyType
        /// </summary>
        public string DependencyType { get; set; }

        /// <summary>
        /// Gets or sets the ToolTip
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get => (AttributeControlType)AttributeControlTypeId;
            set => AttributeControlTypeId = (int)value;
        }
    }
}