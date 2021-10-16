using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GTAVLiveMap.Core.Infrastructure.Responses
{
    public class Generic
    {
        public int Version = 1;
        public int StatusCode = 200;
        public object Data = new { };
        public IList<Error> Errors = new List<Error>();
    }

    public class Error
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
