using GTAVLiveMap.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace GTAVLiveMap.TelegramBot
{
    public static class Handlers
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message        => BotOnMessageReceived(botClient , update.Message),              
                UpdateType.EditedMessage  => BotOnMessageReceived(botClient , update.EditedMessage),
                _                         => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Type != MessageType.Text)
                return;

            var action = (message.Text.Split(' ').First()) switch
            {
                "/create_map" => CreateMap(botClient, message),
                _ => Usage(botClient, message)
            };

            var sentMessage = await action;
            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

            static async Task<Message> CreateMap(ITelegramBotClient botClient, Message message)
            {
                string[] args = message.Text.Split(' ');

                if (args.Length < 3)
                    return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: @"Use: /create_map {email} {name}");

                string email = args[1];
                string name = args[2];

                var user = await Program.UserRepository.GetByEmail(email);

                if (user == null)
                    return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Not found {email} user");

                var map = await Program.MapRepository.Add(new Domain.Entities.Map
                {
                    ApiKey = Generator.GetRandomString(16),
                    Name = name,
                    OwnerId = user.Id
                });

                var text = @$"
Id: {map.Id}
APIKey: {map.ApiKey}
Name: {map.Name}
OwnerId: {map.OwnerId}
CreateAt: {map.CreatedAt}";

                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: text);
            }

            static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
            {
                const string usage = "Invalid command";

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: usage);
            }
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}
