using System;

namespace GTAVLiveMap.Core.Infrastructure
{
    public static class Generator
    {
        public static string GetRandomString(int size, bool upperCase = false)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[size];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
                stringChars[i] = chars[random.Next(chars.Length)];

            var finalString = new String(stringChars);

            return upperCase ? finalString.ToUpper() : finalString;
        }
    }
}
