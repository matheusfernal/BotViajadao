using System.Globalization;

namespace BotViajadao.Model.Cotacoes
{
    public static class CotacaoExtensions
    {
        public static string ObtemTextoCotacao(this Cotacao cotacao)
        {
            return $"{cotacao.Nome}: **{cotacao.ObtemValorCotacaoFormatado()}**";
        }

        public static string ObtemValorCotacaoFormatado(this Cotacao cotacao)
        {
            return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", cotacao.Valor);
        }
    }
}