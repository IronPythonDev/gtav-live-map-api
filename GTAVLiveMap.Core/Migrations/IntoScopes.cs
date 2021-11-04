using FluentMigrator;
using GTAVLiveMap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211104044200)]
    public class IntoScopes : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            var dict = new Dictionary<string, object>();

            foreach (var name in Enum.GetNames(typeof(MapScopeNameEnum)))
            {
                Insert.IntoTable("MapScopes").Row(new { Name = name });
            }
        }
    }
}
