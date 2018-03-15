using System;
using System.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace BotViajadao.Services
{
    public class ServicoTraducaoTexto : ServicoBase
    {
        private readonly UriBuilder _builder;

        public ServicoTraducaoTexto()
        {
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["TraducaoApiKey"]);
            _builder = ConstruirUriBuilder(ConfigurationManager.AppSettings["TraducaoBaseUrl"]);
        }

        public async Task<string> TraduzirTexto(string texto)
        {
            string resposta = null;
            try
            {
                _builder.Path += "/Translate";
                var query = HttpUtility.ParseQueryString(_builder.Query);
                query["appid"] = "";
                query["from"] = "pt-BR";
                query["to"] = "en";
                query["text"] = texto.Length >= 10000 ? texto.Substring(0, 10000) : texto;

                _builder.Query = query.ToString();

                var url = _builder.ToString();

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    resposta = XElement.Parse(result).Value;
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