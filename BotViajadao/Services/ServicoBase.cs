using System;
using System.Net.Http;

namespace BotViajadao.Services
{
    public abstract class ServicoBase : IDisposable
    {
        protected readonly HttpClient _client;

        protected ServicoBase()
        {
            _client = new HttpClient();
        }

        protected UriBuilder ConstruirUriBuilder(string baseUrl) => new UriBuilder(baseUrl) {Port = -1};

        public void Dispose() => _client.Dispose();
    }
}