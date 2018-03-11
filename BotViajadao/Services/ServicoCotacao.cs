using System;
using System.Configuration;

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
    }
}