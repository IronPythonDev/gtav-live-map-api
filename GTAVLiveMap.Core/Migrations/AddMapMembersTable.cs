using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211016134600)]
    public class AddMapMembersTable : Migration
    {
        private const string TABLE_NAME = "MapMembers";

        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id")
                    .AsGuid()
                    .WithDefault(SystemMethods.NewGuid)
                .WithColumn("Roles")
                    .AsString()
                    .WithDefaultValue("")
                .WithColumn("MapId")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("OwnerId")
                    .AsInt64()
                    .NotNullable()
                    .ForeignKey("Users", "Id")
                    .OnDelete(System.Data.Rule.Cascade);
        }
    }
}
