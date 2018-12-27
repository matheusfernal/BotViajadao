using Newtonsoft.Json;

namespace BotViajadao.Model.Yelp
{
    public class Localizacao
    {
        [JsonProperty("display_address")]
        public string[] DisplayAdress { get; set; }
    }
}