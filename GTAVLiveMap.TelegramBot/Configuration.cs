using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GTAVLiveMap.TelegramBot
{
    public static class Configuration
    {
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }
    }
}
