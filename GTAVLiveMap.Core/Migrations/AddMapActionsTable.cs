using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211029222000)]
    public class AddMapActionsTable : Migration
    {
        public const string TABLE_NAME = "MapActions";
        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id")
                    .AsGuid()
                    .WithDefaultValue(SystemMethods.NewGuid)
                .WithColumn("Name")
                    .AsString()
                    .NotNullable()
                .WithColumn("MapId")
                    .AsGuid()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade);
        }
    }
}
