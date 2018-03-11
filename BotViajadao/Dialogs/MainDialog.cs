using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BotViajadao.Model;
using BotViajadao.Services;
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
            var moedas = result.Entities?.Select(e => e.Entity);

            await context.PostAsync("Converter moeda");
            context.Done<string>(null);
        }

        [LuisIntent("Ajuda")]
        public async Task AjudaAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Ajuda");
            context.Done<string>(null);
        }

        [LuisIntent("none")]
        public async Task NoneAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("none");
            context.Done<string>(null);
        }

        /// <summary>
        /// Quando não houve intenção reconhecida.
        /// </summary>
        [LuisIntent("")]
        public async Task IntencaoNaoReconhecida(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("null");
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

        #endregion

    }
}