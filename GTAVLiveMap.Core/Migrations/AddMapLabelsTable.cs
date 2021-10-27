using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211027141300)]
    public class AddMapLabelsTable : Migration
    {
        public const string TABLE_NAME = "MapLabels";
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
                .WithColumn("Coordinates")
                    .AsString()
                    .WithDefaultValue("")
                .WithColumn("Type")
                    .AsInt32()
                    .WithDefaultValue(1)
                .WithColumn("PopupBody")
                    .AsString()
                    .WithDefaultValue(null);
        }
    }
}
