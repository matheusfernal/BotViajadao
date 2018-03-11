using Newtonsoft.Json;

namespace BotViajadao.Model.Cotacoes
{
    public class Moeda
    {
        [JsonProperty("moeda")]
        public string Simbolo { get; set; }

        public string Nome { get; set; }

        public string Fonte { get; set; }
    }
}