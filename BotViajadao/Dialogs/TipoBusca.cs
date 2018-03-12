using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using BotViajadao.Util;

namespace BotViajadao.Dialogs
{
    public enum EnumTipoBusca
    {
        Hotel,
        Restaurante,
        Passeio
    }

    public static class TipoBusca
    {
        public static string MensagemPesquisandoItens(EnumTipoBusca tipoBusca)
        {
            switch (tipoBusca)
            {
                case EnumTipoBusca.Hotel:
                    return "Estou pesquisando hoteis para vc...";
                case EnumTipoBusca.Restaurante:
                    return "Segura a fome que já estou encontrando restaurantes para vc.";
                case EnumTipoBusca.Passeio:
                    return "Já estou consultando minhas fontes sobre passeios legais.";
                default:
                    return "Relaxa que já vou encontrar o que procura.";
            }
        }

        public static string MensagemItensEncontrados(EnumTipoBusca tipoBusca, int quantidadeItens)
        {
            switch (tipoBusca)
            {
                case EnumTipoBusca.Hotel:
                    return $"Aqui estão {quantidadeItens} hoteis que encontrei";
                case EnumTipoBusca.Restaurante:
                    return $"Encontrei {quantidadeItens} restaurantes pra vc. Espero que algum seja delicioso.";
                case EnumTipoBusca.Passeio:
                    return $"Não esqueça a câmera! Esses {quantidadeItens} passeios que encontrei parecem emocionantes.";
                default:
                    return "Relaxa que já vou encontrar o que procura.";
            }
        }

        public static string MensagemInformarCidade(EnumTipoBusca tipoBusca)
        {
            switch (tipoBusca)
            {
                case EnumTipoBusca.Hotel:
                    return "Entendi, agore me envie em qual cidade você quer se hospedar.";
                case EnumTipoBusca.Restaurante:
                    return "Me diga em qual cidade você procura um restaurante.";
                case EnumTipoBusca.Passeio:
                    return "Passeios são legais! Mas para qual cidade você que que eu procure?";
                default:
                    return "";
            }
        }

        public static string MensagemInformarApenasUmaCidade(EnumTipoBusca tipoBusca)
        {
            switch (tipoBusca)
            {
                case EnumTipoBusca.Hotel:
                    return $"Desculpe mas eu só consigo recomendar hotéis para uma cidade por vez {Emoji.Confused}";
                case EnumTipoBusca.Restaurante:
                    return $"Desculpe mas eu só consigo recomendar restaurantes para uma cidade por vez {Emoji.Confused}";
                case EnumTipoBusca.Passeio:
                    return $"Desculpe mas eu só consigo recomendar passeios para uma cidade por vez {Emoji.Confused}";
                default:
                    return "";
            }
        }

        public static string CategoriaBuscaYelp(EnumTipoBusca tipoBusca)
        {
            switch (tipoBusca)
            {
                case EnumTipoBusca.Hotel:
                    return "hotels";
                case EnumTipoBusca.Restaurante:
                    return "restaurants";
                case EnumTipoBusca.Passeio:
                    return "tours";
                default:
                    return "";
            }
        }

    }
}