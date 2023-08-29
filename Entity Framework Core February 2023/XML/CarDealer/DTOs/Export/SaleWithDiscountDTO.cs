﻿using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("sale")]
    public class SaleWithDiscountDTO
    {
        [XmlElement("car")]
        public CarSaleDTO Car { get; set; }

        [XmlElement("discount")]
        public decimal Discount { get; set; }

        [XmlElement("customer-name")]
        public string CustomerName { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }

        [XmlElement("price-with-discount")]
        public decimal PriceWithDiscount { get; set; }
    }
}
