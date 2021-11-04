using GTAVLiveMap.Core.Infrastructure.Contexts;
using GTAVLiveMap.Core.Infrastructure.Repositories;
using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace GTAVLiveMap.TelegramBot
{
    class Program
    {
        private static TelegramBotClient Bot;
        
        public static DbContext DbContext;
        public static MapRepository MapRepository;
        public static UserRepository UserRepository;
        public static SessionKeyRepository SessionKeyRepository;
        public static InviteRepository InviteRepository;
        public static MapMemberRepository MapMemberRepository;
        public static MapConfigRepository MapConfigRepository;

        static void Main(string[] args)
        {
            DbContext = new(Configuration.GetConfiguration());

            MapRepository = new(DbContext);
            UserRepository = new(DbContext);
            SessionKeyRepository = new(DbContext);
            InviteRepository = new(DbContext);
            MapMemberRepository = new(DbContext);
            MapConfigRepository = new(DbContext);

            Bot = new TelegramBotClient(Configuration.GetConfiguration()["BotConfiguration:ApiKey"]);

            var me = Bot.GetMeAsync().GetAwaiter().GetResult();

            Console.WriteLine($"Bot Name: {me.Username}");

            using var cts = new CancellationTokenSource();

            Bot.StartReceiving(new DefaultUpdateHandler(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync), cts.Token);

            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();

            cts.Cancel();
        }
    }
}
