using System.Xml.Serialization;

namespace ProductShop.DTOs
{
    [XmlType("User")]
    public class UserWithSoldProductsDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public ProductDTO[] SoldProducts{ get; set; }
    }
}
