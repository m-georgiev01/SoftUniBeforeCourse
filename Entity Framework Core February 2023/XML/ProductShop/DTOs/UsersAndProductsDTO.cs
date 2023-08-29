using System.Xml.Serialization;

namespace ProductShop.DTOs
{
    [XmlType("Users")]
    public class UsersAndProductsDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public UserElementDTO[] Users { get; set; }
    }
}
