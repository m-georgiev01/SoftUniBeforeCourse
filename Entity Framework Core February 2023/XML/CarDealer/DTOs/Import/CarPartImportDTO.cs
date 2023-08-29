using System.Xml.Serialization;

namespace CarDealer.DTOs.Import
{
    [XmlType("partId")]
    public class CarPartImportDTO
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
