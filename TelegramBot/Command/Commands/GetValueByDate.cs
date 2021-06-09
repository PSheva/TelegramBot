using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Web_Api.Clients;
using Web_Api.Models;

namespace TelegramBot.Command.Commands
{
    class GetValueByDate : Command
    {

        public override string Name { get; set; } = "/get_value_by_date";
         
        public override TelegramBotClient Bot { get; set; }

        public override async void Execute(Message message, TelegramBotClient client)
        {

            Bot = client;

            _ = await client.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 "Введіть назву криптовалюти та дату, за яку ви хочете отримати інформацію.📅" +
                 "\nОсь приклад правильного запису для цієї функції (bitcoin, 17-11-2018) ");
            Bot.OnMessage += GetValueBySomeDate;

        }

        private async void GetValueBySomeDate(object sender, MessageEventArgs e)
        {

            string input = e.Message.Text;
           
            var inputs = input.Split(", ");

            var client = new HttpClient();
            client.BaseAddress = new Uri(Constants.address);

            var result = await client.GetAsync($"/CryptoTask/get_data/{inputs[0]}/history/{inputs[1]}");
                      
            //Bot.SendTextMessageAsync(e.Message.Chat.Id, inputs[0] + inputs[1]);
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync().Result;
               // Bot.SendTextMessageAsync(e.Message.Chat.Id, content);
                var info = JsonConvert.DeserializeObject<ValueByDataResponse>(content);

                SendInf(info, e.Message);
            }
            else
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "\n ❗️ ❗️ ❗️FAIL❗️ ❗️ ❗️" +
              "\n Можливі причини невдачі:" +
               "\n Ви не правильно ввели значення дати:" +
              "\n1)Ви неправильно ввели назву криптовалюти" +
              "\n2)Ви ввели у неправильному регістрі" +
              "\n3)Такої криптовалюти не існує");

            }          

            Bot.OnMessage -= GetValueBySomeDate;
        }
        protected async void SendInf(ValueByDataResponse currencyValue, Message e)
        {

            if (currencyValue != null)
            {
                _ = Bot.SendTextMessageAsync(
               chatId: e.Chat.Id,
               $"\nCrypto: {currencyValue.Name} " +
               $"\nPrice USD: {currencyValue.Market_Data.Current_Price.usd}" +
               $"\nPrice UAH: {currencyValue.Market_Data.Current_Price.uah}" +
               $"\nPrice RUB: {currencyValue.Market_Data.Current_Price.rub}" 
               
               );
            }
        }

    }
}
