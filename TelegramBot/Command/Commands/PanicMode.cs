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
    class PanicMode : Command
    {
        public override string Name { get; set; } = "/be_ready_to_panic";

        public override TelegramBotClient Bot { get; set; }

        public override async void Execute(Message message, TelegramBotClient client)
        {

            Bot = client;

            _ = await client.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 "Бот готовий сповіщати вас про критичні значення Вашої криптовалюти ");
            Bot.OnMessage += PanicOn;

        }
        async void PanicOn(object sender, MessageEventArgs e)
        {
            try
            {
                //    string apiAddress = Constants.address;
                string input = e.Message.Text;

                //    var inputs = input.Split(", ");
                var cl = new HttpClient();
                cl.BaseAddress = new Uri(Constants.address);



                var result1 = await cl.GetAsync($"/CryptoTask/get_data/getPanic?id={input}");
                var result2 = await cl.GetAsync($"/CryptoTask/get_data/{input}");
                if (result1.IsSuccessStatusCode && result2.IsSuccessStatusCode)
                {
                    var content6 = result1.Content.ReadAsStringAsync().Result;
                    var panic = JsonConvert.DeserializeObject<PanicModeDBModel>(content6);

                    var content7 = result1.Content.ReadAsStringAsync().Result;
                    var panic2 = JsonConvert.DeserializeObject<CurrencyValueResponse>(content7);
                    SendInf(panic, panic2, e.Message, input);
                }
                else
                {
                    await Bot.SendTextMessageAsync(e.Message.From.Id, "\nПомилка" +
                "\nМожливі варіанти помилки:" +
                "\n1)Такої криптовалюти не існує" +
                "\n2)Не правильно записана назва" +
                "\n3)Не дотримані вимоги до написання боту"
                );

                }
            }
            catch (FormatException)
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "Error");
            }
        }


        async void SendInf(PanicModeDBModel panic, CurrencyValueResponse panic2, Message e, string input)
        {
            if (panic != null && panic2 != null)
            {
                // SendInf(panic, panic2, e.Message);
                int i = 0;
                while ((Convert.ToDouble(panic.CriticalValue) > Convert.ToDouble(panic2.priceUSD))&& i<9) 
                {
                    i++;
                    var cl = new HttpClient();
                    cl.BaseAddress = new Uri(Constants.address);
                    var result1 = await cl.GetAsync($"/CryptoTask/get_data/getPanic?id={input}");
                    var result2 = await cl.GetAsync($"/CryptoTask/get_data/{input}");

                    await Bot.SendTextMessageAsync(e.Chat.Id, "‼️PANIC‼️" + "\n Look this video to know, is is time to sell cryptocurrency?" + "\nhttps://www.youtube.com/watch?v=uNKe1iEQX9A");
                    System.Threading.Thread.Sleep(10000);  
                   


                }
               // exit
            }
        }
       
    }




}

// var result1 = await cl.GetAsync($"/CryptoTask/get_data/getPanic?id={input}");
//var result2 = await cl.GetAsync($"/CryptoTask/get_data/{input}");
//if (result1.IsSuccessStatusCode || result2.IsSuccessStatusCode)
//{
//    var content6 = result1.Content.ReadAsStringAsync().Result;
//    var panic = JsonConvert.DeserializeObject<PanicModeDBModel>(content6);

//    var content7 = result1.Content.ReadAsStringAsync().Result;
// var panic2 = JsonConvert.DeserializeObject<CurrencyValueResponse>(content7);