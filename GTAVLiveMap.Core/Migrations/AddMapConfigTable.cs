using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211031211100)]
    public class AddMapConfigTable : Migration
    {
        public const string TABLE_NAME = "MapConfigs";
        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("MapId")
                    .AsGuid()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("MaxInvites")
                    .AsInt32()
                    .WithDefaultValue(5)
                .WithColumn("MaxMembers")
                    .AsInt32()
                    .WithDefaultValue(10)
                .WithColumn("MaxObjects")
                    .AsInt32()
                    .WithDefaultValue(100);
        }
    }
}
