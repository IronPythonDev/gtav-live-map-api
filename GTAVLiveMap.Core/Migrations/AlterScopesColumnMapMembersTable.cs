using FluentMigrator;
using GTAVLiveMap.Domain.Enums;
using System;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211610174900)]
    public class AlterScopesColumnMapMembersTable : Migration
    {
        private const string TABLE_NAME = "MapMembers";

        public override void Down()
        {
            Delete.Column("Scopes").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("Scopes")
                    .AsString()
                    .WithDefaultValue(string.Join(';' , Enum.GetNames(typeof(MapScopeNameEnum))));
        }
    }
}
