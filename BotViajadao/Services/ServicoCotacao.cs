using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using BotViajadao.Model.Cotacoes;

namespace BotViajadao.Services
{
    public class ServicoCotacao : ServicoBase
    {
        private readonly UriBuilder _builder;

        private static readonly string _baseUrl = ConfigurationManager.AppSettings["CotacoesBaseUrl"];

        public ServicoCotacao()
        {
            _builder = ConstruirUriBuilder(_baseUrl);
        }

        public async Task<RespostaBuscaMoedasDisponiveis> BuscarMoedasDisponiveis()
        {
            RespostaBuscaMoedasDisponiveis resposta = null;
            try
            {
                var url = $"{_baseUrl}moedas";

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    resposta = await response.Content.ReadAsAsync<RespostaBuscaMoedasDisponiveis>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return resposta;
        }
    }
}