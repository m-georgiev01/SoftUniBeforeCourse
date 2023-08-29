using System.Xml.Serialization;

namespace ProductShop.DTOs
{
    public class ProductElementDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public ProductDTO[] Products { get; set; }
    }
}
