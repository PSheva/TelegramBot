using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Web_Api.Models;


namespace TelegramBot.Command.Commands
{
    class GetDataFromDB : Command
    {
        public override string Name { get; set; } = "/check_favorite";
        public override TelegramBotClient Bot { get; set; }

        public override async void Execute(Message message, TelegramBotClient client)
        {

            Bot = client;

            _ = await client.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 " Enter a name for the currency to verify that it has been added and get short info about : ");

            Bot.OnMessage += GetCheckCurrenciInDB;


        }

        private async void GetCheckCurrenciInDB(object sender, MessageEventArgs e)
        {
            string currencyID = e.Message.Text;
            string apiAddress = Constants.address;
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiAddress);

            var result = await client.GetAsync($"/CryptoTask/get_data/getDB?id={currencyID}");
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync().Result;
                var info = JsonConvert.DeserializeObject<CurrencyValueResponse>(content);

                SendInf(info, e.Message);
            }
            else
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "\n ❗️ ❗️ ❗️Error❗️ ❗️ ❗️" +
              "\n❗️Можливі причини невдачі:" +
              "\n1)Ви ввели у неправильному регістрі" +
              "\n2)Такої криптовалюти не існує");

            }


        }
        protected async void SendInf(CurrencyValueResponse currencyValue, Message e)
        {
            if (currencyValue != null)
            {
                _ = Bot.SendTextMessageAsync(
               chatId: e.Chat.Id,
               $"\nCrypto: {currencyValue.Currency} " +
               $"\nGenesis Date: {currencyValue.Genesis_Date}" +
               $"\nMarket Capitalization Rank: {currencyValue.Market_Cap_Rank}" +
               $"\nPrice USD: {currencyValue.priceUSD}" +
               $"\nPrice UAH: {currencyValue.priceUAH}" +
               $"\nPrice RUB: {currencyValue.priceRUB}" +
               $"\nPrice change(24 hours): {currencyValue.price_change_percentage_24h}" +
               $"\nPrice change(30 days): {currencyValue.price_change_percentage_30d}" +
               $"\nPrice change(1 year): {currencyValue.price_change_percentage_1y}"
               );
            }
        }
    }
}
