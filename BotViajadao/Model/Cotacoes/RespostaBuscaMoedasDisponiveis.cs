using System.Collections.Generic;

namespace BotViajadao.Model.Cotacoes
{
    public class RespostaBuscaMoedasDisponiveis
    {
        public string Status { get; set; }
        public Dictionary<string, Moeda> Moedas { get; set; }
    }
}