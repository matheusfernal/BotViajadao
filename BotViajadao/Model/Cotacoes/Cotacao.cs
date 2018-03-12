using Newtonsoft.Json;

namespace BotViajadao.Model.Cotacoes
{
    public class Cotacao
    {
        public string Nome { get; set; }

        public float Valor { get; set; }

        public string Fonte { get; set; }
    }
}