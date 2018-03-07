﻿using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace BotViajadao.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
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

    }
}