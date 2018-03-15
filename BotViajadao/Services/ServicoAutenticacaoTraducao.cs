using System;
using System.Configuration;
using System.Threading.Tasks;

namespace BotViajadao.Services
{
    public class ServicoAutenticacaoTraducao : ServicoBase
    {
        public ServicoAutenticacaoTraducao()
        {
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["TraducaoApiKey"]);
        }

        public async Task<string> ObtemTokenAutenticacao()
        {
            string resposta = null;
            try
            {
                var url = ConfigurationManager.AppSettings["AutenticacaoBaseUrl"];

                var response = await _client.PostAsync(url, null);
                if (response.IsSuccessStatusCode)
                {
                    resposta = await response.Content.ReadAsStringAsync();
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