using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211027145000)]
    public class AddMapIdColumnToMapLabelsTable : Migration
    {
        public const string TABLE_NAME = "MapLabels";

        public override void Down()
        {
            Delete.Column("MapId").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("MapId")
                .AsGuid()
                .NotNullable()
                .ForeignKey("Maps", "Id")
                .OnDelete(System.Data.Rule.Cascade);
        }
    }
}
