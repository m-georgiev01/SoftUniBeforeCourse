using Newtonsoft.Json;

namespace CarDealer.DTOs
{
    public class CarCustomerPriceDTO
    {
        [JsonProperty("car")]
        public CarDTO Car { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("discount")]
        public string Discount { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("priceWithDiscount")]
        public string PriceWithDiscount { get; set; }
    }
}
