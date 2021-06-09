using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Web_Api.Models;


namespace TelegramBot.Command.Commands
{
    class GetValueByCurrency : Command
    {
        public override string Name { get; set; } = "/get_value";
        public override TelegramBotClient Bot { get; set; }
        

        public override async void Execute(Message message, TelegramBotClient client)
        {

            Bot = client;

            _ = await client.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 "Введіть назву криптовалюти: ");

            Bot.OnMessage += GetValueByName;
        }

        private async void GetValueByName(object sender, MessageEventArgs e)
        {
            string currencyID = e.Message.Text;
            string apiAddress = Constants.address;
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiAddress);

            var result = await client.GetAsync($"/CryptoTask/get_data/{currencyID}");
            if (result.IsSuccessStatusCode)
            {
                var content = result.Content.ReadAsStringAsync().Result;
                var info = JsonConvert.DeserializeObject<CurrencyValueResponse>(content);

                SendInf(info, e.Message);
            }
            else
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "\n ❗️ ❗️ FAIL❗️ ❗️ ❗️" +
              "\n Можливі причини невдачі:" +
              "\n1)Ви непраильно ввели назву криптовалюти" +
              "\n2)Ви ввели у неправильному регістрі" +
              "\n3)Такої криптовалюти не існує");
             
            }

            Bot.OnMessage -= GetValueByName;

        }

        protected async void SendInf(CurrencyValueResponse currencyValue, Message e)
        {
            if (currencyValue != null)
            {
                _ = Bot.SendTextMessageAsync(
               chatId: e.Chat.Id,
               $"\nCrypto: {currencyValue.Currency} " +
               $"\nGenesis Date: {currencyValue.Genesis_Date}"+
               $"\nMarket Capitalization Rank: {currencyValue.Market_Cap_Rank}"+
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
