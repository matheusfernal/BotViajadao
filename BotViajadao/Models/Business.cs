using Newtonsoft.Json;

namespace BotViajadao.Models
{
    public class Business
    {
        public string Id { get; set; }
        public string Name { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }
}