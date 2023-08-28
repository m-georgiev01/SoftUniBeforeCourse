using Newtonsoft.Json;

namespace ProductShop.DTOs
{
    public class UserWithSoldProductsDTO
    {
        [JsonProperty("firstName")]
        public string? FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("soldProducts")]
        public IEnumerable<SoldProductDTO> SoldProducts { get; set; }
    }
}
