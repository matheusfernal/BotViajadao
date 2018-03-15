using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BotViajadao.Model;
using BotViajadao.Model.Cotacoes;
using BotViajadao.Model.Yelp;
using BotViajadao.Services;
using BotViajadao.Util;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace BotViajadao.Dialogs
{
    [Serializable]
    public class MainDialog : LuisDialog<object>
    {
        private const int MaximoResultados = 5;
        private EnumTipoBusca _tipoBuscaAtual;

        public MainDialog(ILuisService service) : base(service) { }

        [LuisIntent("Recomendar restaurantes")]
        public async Task RecomendarRestaurantesAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await RecomendarItensAsync(context, activity, result, EnumTipoBusca.Restaurante);
        }

        [LuisIntent("Recomendar passeios")]
        public async Task RecomendarPasseiosAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await RecomendarItensAsync(context, activity, result, EnumTipoBusca.Passeio);
        }

        [LuisIntent("Recomendar hoteis")]
        public async Task RecomendarHoteisAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await RecomendarItensAsync(context, activity, result, EnumTipoBusca.Hotel);
        }

        [LuisIntent("Converter moeda")]
        public async Task ConverterMoedaAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var moedas = result.Entities?.Select(e => e.Entity).ToList();

            using (var servico = new ServicoCotacao())
            {
                var cotacoes = await servico.BuscarCotacoes();
                Cotacao cotacao = null;

                if (moedas != null && moedas.Count == 1)
                {
                    cotacao = EncontraCotacao(moedas.Single(), cotacoes.Valores);
                }

                if (cotacao != null)
                {
                    await context.PostAsync($"O valor do {moedas.Single()} hoje é de **{cotacao.ObtemValorCotacaoFormatado()}**");
                }
                else
                {
                    await context.PostAsync("As cotações que tenho hoje são:");
                    var texto = cotacoes.Valores.Values.Select(c => $"{c.ObtemTextoCotacao()}").Aggregate((t1, t2) => $"{t1}\n\n{t2}");

                    await context.PostAsync(texto);
                }
            }

            context.Done<string>(null);
        }

        [LuisIntent("Traduzir texto")]
        public async Task TraduzirTextoAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Blza! Agora digite o texto que deseja que eu traduza para você.");
            context.Wait((c, a) => TraduzirTexto(c,a));
        }

        [LuisIntent("Ajuda")]
        public async Task AjudaAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var mensagem = $"Estou sempre pronto para ajudar {Emoji.Grin}\n\n" +
                            "Você pode me pedir as seguintes coisas:\n" +
                            "* Recomendar restaurantes, hoteis ou passeios turísticos em alguma cidade.\n" +
                            "* Obter cotações de moedas.\n" +
                            "* Traduzir um texto para inglês.";

            await context.PostAsync(mensagem);
            context.Done<string>(null);
        }

        [LuisIntent("none")]
        public async Task NoneAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await ExibeMensagemQueNaoEntedeu(context);
            context.Done<string>(null);
        }

        /// <summary>
        /// Quando não houve intenção reconhecida.
        /// </summary>
        [LuisIntent("")]
        public async Task IntencaoNaoReconhecida(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await ExibeMensagemQueNaoEntedeu(context);
            context.Done<string>(null);
        }

        #region Métodos Privados

        private async Task RecomendarItensAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result, EnumTipoBusca tipoBusca)
        {
            var cidades = result.Entities?.Select(e => e.Entity);
            if (cidades == null || cidades.Count() == 0)
            {
                await context.PostAsync(TipoBusca.MensagemInformarCidade(tipoBusca));
                _tipoBuscaAtual = tipoBusca;
                context.Wait((c, a) => RecomendarItens(c, a));
                return;
            }
            else if (cidades.Count() > 1)
            {
                await context.PostAsync(TipoBusca.MensagemInformarApenasUmaCidade(tipoBusca));
            }
            else
            {
                await RecomendarItens(context, activity, cidades.Single(), tipoBusca);
            }

            context.Done<string>(null);
        }

        private async Task RecomendarItens(IDialogContext context, IAwaitable<IMessageActivity> value)
        {
            var cidade = await value;
            await RecomendarItens(context, value, cidade.Text, _tipoBuscaAtual);

        }

        private async Task RecomendarItens(IDialogContext context, IAwaitable<IMessageActivity> activity, string cidade, EnumTipoBusca tipoBusca)
        {
            await context.PostAsync(TipoBusca.MensagemPesquisandoItens(tipoBusca));

            using (var service = new ServicoYelp())
            {
                var resposta = await service.BuscarItens(cidade, tipoBusca);

                var atividade = await activity as Activity;
                var mensagem = atividade.CreateReply();
                var businesses = resposta.Businesses.ToList();
                for (var i = 0; i < businesses.Count && i < MaximoResultados; i++)
                {
                    var business = businesses[i];
                    mensagem?.Attachments.Add(MontaCardResposta(business));
                }

                await context.PostAsync(TipoBusca.MensagemItensEncontrados(tipoBusca, mensagem.Attachments.Count));
                await context.PostAsync(mensagem);
            }

            context.Wait(MessageReceived);
        }

        private Attachment MontaCardResposta(Business business)
        {
            return new HeroCard
            {
                Title = business.Name,
                Subtitle = business.ObtemSubtituloCard(),
                Images = new List<CardImage>
                {
                    new CardImage(business.ImageUrl, business.Name,
                        new CardAction(ActionTypes.OpenUrl, "Yelp", value: business.Url))
                },
                Buttons = new List<CardAction>
                {
                    new CardAction
                    {
                        Title = "Abrir no Yelp",
                        Type = ActionTypes.OpenUrl,
                        Value = business.Url
                    },
                    new CardAction
                    {
                        Title = "Abrir no mapa",
                        Type = ActionTypes.OpenUrl,
                        Value = $"http://maps.google.com/?q={business.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture)},{business.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture)}"
                    }
                }
            }.ToAttachment();
        }

        private Cotacao EncontraCotacao(string moedaUsuario, Dictionary<string, Cotacao> cotacoes)
        {
            foreach (var codigoMoeda in cotacoes.Keys)
            {
                // Se o usuário digitou um código tipo USD, EUR, etc -> verifica ignorando case e acentos
                if (string.Compare(moedaUsuario.Trim(), codigoMoeda.Trim(), CultureInfo.InvariantCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)
                {
                    return cotacoes[codigoMoeda];
                }
                
                // Se o usuário digitou o nome da moeda como dólar, euro, etc -> verifica se varia por duas letras ignorando case
                if (moedaUsuario.Trim().ToLowerInvariant().DamerauLevenshteinDistanceTo(cotacoes[codigoMoeda].Nome.Trim().ToLowerInvariant()) <= 2)
                {
                    return cotacoes[codigoMoeda];
                }
            }

            return null;
        }

        private async Task ExibeMensagemQueNaoEntedeu(IDialogContext context)
        {
            var mensagem = $"Isso é um pouco constrangedor mas não consegui entender o que você precisa. {Emoji.Flushed}\n\n" +
                           $"Se estiver se sentindo perdido, não exite em pedir ajuda. Todos nós estamos aprendendo. {Emoji.Blush}";


            await context.PostAsync(mensagem);
        }

        private async Task TraduzirTexto(IDialogContext context, IAwaitable<IMessageActivity> value)
        {
            var texto = await value;
            using (var service = new ServicoTraducaoTexto())
            {
                var resposta = await service.TraduzirTexto(texto.Text);

                await context.PostAsync("Aqui está o seu texto em inglês:");
                await context.PostAsync(resposta);
            }

            context.Wait(MessageReceived);
        }

        #endregion

    }
}