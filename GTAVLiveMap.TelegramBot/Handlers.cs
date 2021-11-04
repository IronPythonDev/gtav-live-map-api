using GTAVLiveMap.Core.Infrastructure;
using GTAVLiveMap.Domain.Enums;
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

            var currentUser = await Program.UserRepository.GetByTelegramID($"{message.From.Id}");

            var command = message.Text.Split(' ').First();

            var action = currentUser != null ? (command) switch
            {
                "/create_map" => CreateMap(botClient, message),
                "/auth" => Auth(botClient, message),
                _ => Usage(botClient, message)
            } : command == "/auth" ? Auth(botClient , message) : ConnectTelegram(botClient, message);

            var sentMessage = await action;

            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

            static async Task<Message> ConnectTelegram(ITelegramBotClient botClient, Message message)
            {
                return await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: @"
This account is not connected to gtavlivemap.com account please send your session key for authorization
Session key you can get in developer/admin or your cabinet(if this function available)
For login, send this command /auth {sessionKey}");
            }

            static async Task<Message> Auth(ITelegramBotClient botClient, Message message)
            {
                try
                {
                    string[] args = message.Text.Split(' ');

                    if (args.Length < 2)
                        return await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: @"Use: /auth {sessionKey}");

                    string sessionKey = args[1];

                    var key = await Program.SessionKeyRepository.GetByKey(sessionKey);

                    if (key == null)
                        return await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: @"Invalid session key");

                    var user = await Program.UserRepository.UpdateColumnById(key.OwnerId, "TelegramID", $"{message.From.Id}");

                    if (user == null)
                        return await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: @"User not found");

                    return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: $"Successfully connected telegram for this {user.Email} account!");
                }
                catch (Exception)
                {
                    return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: @"Failed connect account");
                }
            }

            static async Task<Message> CreateMap(ITelegramBotClient botClient, Message message)
            {
                var currentUser = await Program.UserRepository.GetByTelegramID($"{message.From.Id}");

                if (!currentUser.Roles.Contains("Admin"))
                {
                    return await botClient.SendTextMessageAsync(
                        chatId: message.Chat.Id,
                        text: @"This command available for admins");
                }

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
                    Name = name,
                    MaxMembers = 10,
                    OwnerId = user.Id
                });

                var inviteKey = await Program.InviteRepository.Add(new Domain.Entities.Invite
                {
                    MapId = map.Id,
                    Key = Generator.GetRandomString(6, true),
                    Scopes = string.Join(';', Enum.GetNames(typeof(MapScopeNameEnum)))
                });

                var member = await Program.MapMemberRepository.Add(new Domain.Entities.MapMember
                {
                    Scopes = inviteKey.Scopes,
                    InviteKey = inviteKey.Key,
                    MapId = map.Id,
                    OwnerId = user.Id
                });

                var config = await Program.MapConfigRepository.Add(new Domain.Entities.MapConfig
                {
                    MapId = map.Id
                });

                var text = @$"

-----------Map Details-----------
Id: {map.Id}
APIKey: {map.ApiKey}
Name: {map.Name}
OwnerId: {map.OwnerId}
CreateAt: {map.CreatedAt}
-----------Invite Key------------
Id: {inviteKey.Id}
Key: {inviteKey.Key}
Scopes: {inviteKey.Scopes}
CreateAt: {inviteKey.CreatedAt}
-------------Member--------------
Id: {member.Id}
Key: {member.InviteKey}
Scopes: {member.Scopes}
-------------Config--------------
MaxAction: {config.MaxActions}
MaxInvites: {config.MaxInvites}
MaxObjects: {config.MaxObjects}
MaxMembers: {config.MaxMembers}
";

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
