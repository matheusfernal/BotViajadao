using System.Collections.Generic;

namespace BotViajadao.Model.Cotacoes
{
    public class RespostaBuscaCotacoes
    {
        public string Status { get; set; }
        public Dictionary<string, Cotacao> Valores { get; set; }
    }
}