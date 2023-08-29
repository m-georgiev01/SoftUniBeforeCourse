using System.Xml.Serialization;

namespace ProductShop.DTOs
{
    [XmlType("User")]
    public class UserElementDTO
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public ProductElementDTO SoldProducts { get; set; }
    }
}
