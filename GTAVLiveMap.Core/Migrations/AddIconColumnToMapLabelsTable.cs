using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211101200700)]
    public class AddIconColumnToMapLabelsTable : Migration
    {
        public const string TABLE_NAME = "MapLabels";

        public override void Down()
        {
            Delete.Column("Icon").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("Icon")
                .AsString()
                .Nullable()
                .WithDefaultValue("marker");
        }
    }
}
