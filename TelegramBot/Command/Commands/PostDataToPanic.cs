using System;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Web_Api.Models;
using System.Text;

namespace TelegramBot.Command
{
    class PostDataToPanic : Command
    {
        public override string Name { get; set; } = "/post_to_panic";
        public override TelegramBotClient Bot { get; set; }

        public override async void Execute(Message message, TelegramBotClient client)
        {
            Bot = client;

            _ = await client.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 "Введіть назву криптовалюти та її критичне значення(в доларах). Якщо поточне значення цієї валюти буде нижче за ваше, то Бот обовя'зково вас сповістить🗣📩 "+
                 "\nОсь приклад правильного запису для цієї функції (bitcoin, 36000)");

            Bot.OnMessage += PostToPanic;
        }

        private async void PostToPanic(object sender, MessageEventArgs e)
        {
            try
            {
                string apiAddress = Constants.address;
                string input = e.Message.Text;

                var inputs = input.Split(", ");

                var cl = new HttpClient();
                cl.BaseAddress = new Uri(Constants.address);


                var panicData = new PanicModeDBModel
                {
                    Name = Convert.ToString(inputs[0]),
                    CriticalValue = Convert.ToString(inputs[1])

                };
                SendInf(panicData, e.Message);
                }
            catch
            {
                await Bot.SendTextMessageAsync(e.Message.From.Id, "Look Try Catch");
            }
            Bot.OnMessage -= PostToPanic;
        }
        protected async void SendInf(PanicModeDBModel response, Message e)
        {
            if (response != null)
            {
                var cl = new HttpClient();
                cl.BaseAddress = new Uri(Constants.address);

                await Bot.SendTextMessageAsync(e.Chat.Id,
                    $"\n Ваш запит на Post" +
                    $"\nCurrency - {response.Name}" +
                    $"\nCritical Value- {response.CriticalValue}"
                    );


                var json = JsonConvert.SerializeObject(response);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var post = await cl.PostAsync($"/CryptoTask/add_to_panic", data);
                // post.EnsureSuccessStatusCode();
                var postcontent = post.Content.ReadAsStringAsync().Result;
                Console.WriteLine(postcontent);
                if (post.IsSuccessStatusCode)
                {
                    await Bot.SendTextMessageAsync(e.Chat.Id,$"Криптовалюта та значення були успішно записані у базі даних.🎯 "  );
                }
                else
                {
                    await Bot.SendTextMessageAsync(e.Chat.Id,
                                                $"⛔️FAIL⛔️" +
                                                $"\n Дані не були записані"+
                                                 "\n Можливі причини невдачі:" +
                                                "\nВи записали значення без дотримання вимог:" +
                                                "\n1)Ви ввели у неправильному регістрі" +
                                                "\n2)Ви пропустили кому, або ж пробіл після неї" +
                                                "\n3)Такої криптовалюти не існує"+
                                                "\nСпробуйте ще раз)");
                            
                               
                }

            }
        }
    }
}
