using BotViajadao.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BotViajadao.Services
{
    public class YelpService : IDisposable
    {
        private readonly HttpClient _client;
        private readonly UriBuilder _builder;

        private const string BaseUrl = "https://api.yelp.com/v3/businesses/search";

        public YelpService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["YelpApiKey"]);

            _builder = new UriBuilder(BaseUrl);
            _builder.Port = -1;
        }

        public async Task<RespostaBuscaYelp> BuscarHoteis(string cidade)
        {
            RespostaBuscaYelp resposta = null;
            try
            {
                var query = HttpUtility.ParseQueryString(_builder.Query);
                query["term"] = "hotels";
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

        public void Dispose() => _client.Dispose();
    }
}