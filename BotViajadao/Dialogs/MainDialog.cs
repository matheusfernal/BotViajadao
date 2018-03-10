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
using Util;

namespace BotViajadao.Dialogs
{
    [Serializable]
    public class MainDialog : LuisDialog<object>
    {
        private const int MaximoResultados = 5;

        public MainDialog(ILuisService service) : base(service) { }

        [LuisIntent("Recomendar restaurantes")]
        public async Task RecomendarRestaurantesAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Recomendar restaurantes");
            context.Done<string>(null);
        }

        [LuisIntent("Recomendar passeios")]
        public async Task RecomendarPasseiosAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Recomendar passeios");
            context.Done<string>(null);
        }

        [LuisIntent("Recomendar hoteis")]
        public async Task RecomendarHoteisAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var cidades = result.Entities?.Select(e => e.Entity);
            if (cidades == null || cidades.Count() == 0)
            {
                await context.PostAsync("Entendi, agore me envie em qual cidade você quer se hospedar.");
                context.Wait((c, a) => RecomendarHoteis(c, a));
                return;
            }
            else if (cidades.Count() > 1)
            {
                await context.PostAsync($"Desculpe mas eu só consigo recomendar hotéis para uma cidade por vez {Emoji.Confused}");
            }
            else
            {
                await RecomendarHoteis(context, activity, cidades.Single());
            }

            context.Done<string>(null);
        }

        [LuisIntent("Converter moeda")]
        public async Task ConverterMoedaAsync(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
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

        private async Task RecomendarHoteis(IDialogContext context, IAwaitable<IMessageActivity> value)
        {
            var cidade = await value;
            await RecomendarHoteis(context, value, cidade.Text);

        }

        private async Task RecomendarHoteis(IDialogContext context, IAwaitable<IMessageActivity> activity, string cidade)
        {
            await context.PostAsync("Estou pesquisando hoteis para vc...");

            using (var service = new YelpService())
            {
                var resposta = await service.BuscarHoteis(cidade);

                var atividade = await activity as Activity;
                var mensagem = atividade?.CreateReply();
                var businesses = resposta.Businesses.ToList();
                for (var i = 0; i < businesses.Count && i < MaximoResultados; i++)
                {
                    var business = businesses[i];
                    mensagem?.Attachments.Add(MontaCardResposta(business));
                }

                await context.PostAsync("Aqui estão 5 hoteis que encontrei");
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

    }
}