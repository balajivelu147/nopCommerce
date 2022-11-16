namespace Nop.Core.Domain.Shipping
{
    /// <summary>
    /// Represents a shipment
    /// </summary>
    public partial class Warehouse : BaseEntity
    {
        /// <summary>
        /// Gets or sets the warehouse name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the address identifier of the warehouse
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Gets or sets the vendor identifier of the warehouse
        /// </summary>
        public int VendorId { get; set; }

    }
    public partial class WarehouseWithLatLong  
    {

        public WarehouseWithLatLong(int id, string name, decimal? latitude, decimal? longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            Name = name;
            //VendorId = vendorId;
            Id = id;
        }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Name { get; set; }
        //public int VendorId { get; set; }
        public int Id { get; set; }

    }
}