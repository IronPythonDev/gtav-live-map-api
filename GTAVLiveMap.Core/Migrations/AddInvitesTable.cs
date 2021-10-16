using FluentMigrator;
using GTAVLiveMap.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211016233500)]
    public class AddInvitesTable : Migration
    {
        private const string TABLE_NAME = "Invites";
        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id")
                    .AsGuid()
                    .Unique()
                    .WithDefault(SystemMethods.NewGuid)
                .WithColumn("Key")
                    .AsString()
                    .Unique()
                    .WithDefaultValue("")
                .WithColumn("Scopes")
                    .AsString()
                    .NotNullable()
                    .WithDefaultValue(string.Join(';', Enum.GetNames(typeof(MapScopeNameEnum))))
                .WithColumn("MapId")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("CreatedAt")
                    .AsDateTime()
                    .WithDefault(SystemMethods.CurrentDateTime);
        }
    }
}
