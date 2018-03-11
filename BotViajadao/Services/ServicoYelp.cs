using BotViajadao.Model;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BotViajadao.Dialogs;

namespace BotViajadao.Services
{
    public class ServicoYelp : ServicoBase
    {
        private readonly UriBuilder _builder;

        private static readonly string _baseUrl = ConfigurationManager.AppSettings["YelpBaseUrl"];

        public ServicoYelp()
        {
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["YelpApiKey"]);

            _builder = ConstruirUriBuilder(_baseUrl);
        }

        public async Task<RespostaBuscaYelp> BuscarItens(string cidade, EnumTipoBusca tipoBusca)
        {
            RespostaBuscaYelp resposta = null;
            try
            {
                var query = HttpUtility.ParseQueryString(_builder.Query);
                query["categories"] = TipoBusca.CategoriaBuscaYelp(tipoBusca);
                query["location"] = cidade;
                _builder.Query = query.ToString();

                var url = _builder.ToString();

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    resposta = await response.Content.ReadAsAsync<RespostaBuscaYelp>();
                }
            }
            catch(Exception e)
            {
                var st = e.StackTrace;
                //TODO: Logar exceção
            }

            return resposta;
        }
    }
}