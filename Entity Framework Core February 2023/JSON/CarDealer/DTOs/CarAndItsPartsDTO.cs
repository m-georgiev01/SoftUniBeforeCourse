using Newtonsoft.Json;

namespace CarDealer.DTOs
{
    public class CarAndItsPartsDTO
    {
        [JsonProperty("car")]
        public CarDTO Car { get; set; }

        [JsonProperty("parts")]
        public IEnumerable<PartDTO> Parts { get; set; }
    }
}
