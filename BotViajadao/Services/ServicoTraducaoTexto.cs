using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace BotViajadao.Services
{
    public class ServicoTraducaoTexto : ServicoBase
    {
        public ServicoTraducaoTexto()
        {
            _client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["TraducaoApiKey"]);
        }

        public async Task<string> TraduzirTexto(string texto)
        {
            string resposta = null;

            try
            {
                var builder = ConstruirUriBuilder(ConfigurationManager.AppSettings["TraducaoBaseUrl"]);
                builder.Path += "/Translate";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["appid"] = "";
                query["from"] = "pt-BR";
                query["to"] = "en";
                query["text"] = texto.Length >= 10000 ? texto.Substring(0, 10000) : texto;

                builder.Query = query.ToString();

                var url = builder.ToString();

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

        public string ObtemUrlParaTextoFalado(string texto, string token)
        {
            var builder = ConstruirUriBuilder(ConfigurationManager.AppSettings["TraducaoBaseUrl"]);

            try
            {
                builder.Path += "/Speak";
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["appid"] = $"Bearer {token}";
                query["language"] = "en";
                query["text"] = texto.Length >= 2000 ? texto.Substring(0, 2000) : texto;

                builder.Query = query.ToString();

                return builder.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}