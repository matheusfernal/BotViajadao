using System;
using System.Linq;
using System.Threading.Tasks;
using BotViajadao.Models;
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
        private readonly YelpService _servicoYelp;

        public MainDialog(ILuisService service) : base(service)
        {
            _servicoYelp = new YelpService();
        }

        [LuisIntent("Recomendar restaurantes")]
        public async Task RecomendarRestaurantesAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Recomendar restaurantes");
            context.Done<string>(null);
        }

        [LuisIntent("Recomendar passeios")]
        public async Task RecomendarPasseiosAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Recomendar passeios");
            context.Done<string>(null);
        }

        [LuisIntent("Recomendar hoteis")]
        public async Task RecomendarHoteisAsync(IDialogContext context, LuisResult result)
        {
            var cidades = result.Entities?.Select(e => e.Entity);
            if (cidades == null || cidades.Count() == 0)
            {
                await context.PostAsync("Entendi, agore me envie em qual cidade você quer se hospedar.");

            }
            else if (cidades.Count() > 1)
            {
                await context.PostAsync("Desculpe mas eu só consigo recomendar hotéis para uma cidade por vez :/");
                return;
            }

            var resposta = await new YelpService().BuscarHoteis("New York");

            await context.PostAsync("Recomendar hoteis");
            context.Done<string>(null);
        }

        [LuisIntent("Converter moeda")]
        public async Task ConverterMoedaAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Converter moeda");
            context.Done<string>(null);
        }

        [LuisIntent("Ajuda")]
        public async Task AjudaAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ajuda");
            context.Done<string>(null);
        }

        [LuisIntent("none")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("none");
            context.Done<string>(null);
        }

        /// <summary>
        /// Quando não houve intenção reconhecida.
        /// </summary>
        [LuisIntent("")]
        public async Task IntencaoNaoReconhecida(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("null");
            context.Done<string>(null);
        }

        private async Task RecomendarHoteis(IDialogContext context, IAwaitable<IMessageActivity> value)
        {
            var cidade = await value;
            using (var service = new YelpService())
            {
                var resposta = await service.BuscarHoteis(cidade.Text);

                var atividade = await value as Activity;
                var mensagem = atividade.CreateReply();
                mensagem.Attachments.Add(MontaCardResposta(resposta));

                await context.PostAsync(mensagem);
            }

            context.Wait(MessageReceived);
        }

        public Attachment MontaCardResposta(RespostaBuscaYelp resposta)
        {
            throw new NotImplementedException();
        }

    }
}