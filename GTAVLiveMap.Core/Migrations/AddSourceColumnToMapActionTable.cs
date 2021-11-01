using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211101165000)]
    public class AddSourceColumnToMapActionTable : Migration
    {
        public const string TABLE_NAME = "MapActions";

        public override void Down()
        {
            Delete.Column("Source").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("Source")
                .AsString()
                .Nullable()
                .WithDefaultValue("");
        }
    }
}
