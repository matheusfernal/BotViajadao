using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using BotViajadao.Model.Cotacoes;

namespace BotViajadao.Services
{
    public class ServicoCotacao : ServicoBase
    {
        private static readonly string _baseUrl = ConfigurationManager.AppSettings["CotacoesBaseUrl"];

        public async Task<RespostaBuscaCotacoes> BuscarCotacoes()
        {
            RespostaBuscaCotacoes resposta = null;
            try
            {
                var url = $"{_baseUrl}valores";

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    resposta = await response.Content.ReadAsAsync<RespostaBuscaCotacoes>();
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