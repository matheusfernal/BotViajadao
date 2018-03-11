using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;

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
                    return $"Esses {quantidadeItens} passeios que encontrei parecem legais.";
                default:
                    return "Relaxa que já vou encontrar o que procura.";
            }
        }
    }
}