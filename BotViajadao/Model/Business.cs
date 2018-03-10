using Newtonsoft.Json;

namespace BotViajadao.Model
{
    public class Business
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }

        public string Url { get; set; }

        [JsonProperty("review_count")]
        public int ReviewCount { get; set; }

        public float Rating { get; set; }

        [JsonProperty("display_phone")]
        public string DisplayPhone { get; set; }

        public Localizacao Location { get; set; }

        public Coordenadas Coordinates { get; set; }

    }
}