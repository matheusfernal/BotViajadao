using BotViajadao.Dialogs;
using BotViajadao.Model.Yelp;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace BotViajadao.Services
{
    public class ServicoYelp : ServicoBase
    {
        public ServicoYelp()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["YelpApiKey"]);
        }

        public async Task<RespostaBuscaYelp> BuscarItens(string cidade, EnumTipoBusca tipoBusca)
        {
            RespostaBuscaYelp resposta = null;
            try
            {
                var builder = ConstruirUriBuilder(ConfigurationManager.AppSettings["YelpBaseUrl"]);

                var query = HttpUtility.ParseQueryString(builder.Query);
                query["categories"] = TipoBusca.CategoriaBuscaYelp(tipoBusca);
                query["location"] = cidade;
                builder.Query = query.ToString();

                var url = builder.ToString();

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    resposta = await response.Content.ReadAsAsync<RespostaBuscaYelp>();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return resposta;
        }
    }
}