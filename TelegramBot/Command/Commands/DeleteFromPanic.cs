using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Web_Api.Models;

namespace TelegramBot.Command.Commands
{
    class DeleteFromPanic: Command
    {
        public override string Name { get; set; } = "/delete_from_panic";

        public override TelegramBotClient Bot { get; set; }

        public override async void Execute(Message message, TelegramBotClient client)
        {

            Bot = client;

            _ = await client.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 "Введіть назву криптовалюти щоб видалити ");
            Bot.OnMessage += DeletePanic;

        }

        private async void DeletePanic(object sender, MessageEventArgs e)
        {

            string input = e.Message.Text;
              

            var client = new HttpClient();
            client.BaseAddress = new Uri(Constants.address);

            var result = await client.DeleteAsync($"/CryptoTask/delete_fromDB?id={input}");

            //Bot.SendTextMessageAsync(e.Message.Chat.Id, inputs[0] + inputs[1]);
            if (result.IsSuccessStatusCode)
            {              

                SendInf(input, e.Message);
            }
            else
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "\n Not Success"+"\n1) Такої криптовалюти не знайдено у базі даних \n2))");
              

            };

            Bot.OnMessage -= DeletePanic;
        }
        protected async void SendInf( string input, Message e)
        {

            if (input != null)
            {
                _ = Bot.SendTextMessageAsync(
               chatId: e.Chat.Id,
               $"\nCrypto: {input} Успішно видалена з бази даних "); ;
            }
        }

    }
}
