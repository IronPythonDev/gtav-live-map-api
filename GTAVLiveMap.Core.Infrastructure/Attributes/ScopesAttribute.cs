using System;

namespace GTAVLiveMap.Core.Infrastructure.Attributes
{
    public class ScopesAttribute : Attribute
    {
        public string Scopes = "";
        public ScopesAttribute(string scopes)
        {
            Scopes = scopes;
        }
    }
}
