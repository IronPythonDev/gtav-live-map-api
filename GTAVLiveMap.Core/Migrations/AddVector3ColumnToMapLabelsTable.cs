using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211030130700)]
    public class AddVector3ColumnToMapLabelsTable : Migration
    {
        public const string TABLE_NAME = "MapLabels";

        public override void Down()
        {
            Delete.Column("Vector3").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("Vector3")
                .AsString()
                .WithDefaultValue("{}");
        }
    }
}
