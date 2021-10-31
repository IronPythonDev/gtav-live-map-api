using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211031232600)]
    public class AddMaxActionsColumnToMapConfigTable : Migration
    {
        public const string TABLE_NAME = "MapConfigs";
        public const string COLUMN_NAME = "MaxActions";
        public override void Down()
        {
            Delete.Column(COLUMN_NAME).FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Column(COLUMN_NAME)
                .OnTable(TABLE_NAME)
                .AsInt32()
                .WithDefaultValue(5);
        }
    }
}
