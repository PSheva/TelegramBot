using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using System.Collections.Generic;

using System;

using DocumentFormat.OpenXml.Office2010.CustomUI;
using TelegramBot.Command.Commands;
using TelegramBot.Command;

namespace TelegramBot
{

    class Program
    {
        private static ITelegramBotClient botClient;
        private static string token { get; set; } = "1830800743:AAEqdDsNUMFNkrYCVDukDSfxSNCz3z0sbmU";
       

        private static TelegramBotClient client;
        private static List<Command.Command> commands;
        static void Main(string[] args)
        {           

            client = new TelegramBotClient(token);
            commands = new List<Command.Command>
            {
                new GetValueByCurrency(),
                new GetValueByDate(),
                new PanicMode(),
                new PostDataToPanic(),
                ////new DeleteAllFromDB(),
                new DeleteFromPanic()
                //new GetAllFromDB()

            };

            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            if (message == null || message.Type != MessageType.Text)
                return;
            string name = $"{message.From.FirstName} {message.From.LastName}";
            Console.WriteLine($"{name} отправил сообщение: '{message.Text}'");

            foreach (var comm in commands)
            {
                if (message.Text == comm.Name)
                {
                    comm.Execute(message, client);
                }
            }
        }
    }
}
