using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211026210900)]
    public class AddIndexes : Migration
    {
        public override void Down()
        {
            Delete.Index()
                .OnTable("Maps")
                .OnColumn("OwnerId");

            Delete.Index()
                .OnTable("Maps")
                .OnColumn("ApiKey");

            Delete.Index()
                .OnTable("MapMembers")
                .OnColumn("MapId");

            Delete.Index()
                .OnTable("MapMembers")
                .OnColumn("OwnerId");

            Delete.Index()
                .OnTable("MapMembers")
                .OnColumn("InviteKey");

            Delete.Index()
                .OnTable("MapMembers")
                .OnColumn("Scopes");

            Delete.Index()
                .OnTable("Invites")
                .OnColumn("Key");

            Delete.Index()
                .OnTable("Invites")
                .OnColumn("MapId");
        }

        public override void Up()
        {
            Create.Index()
                .OnTable("Maps")
                .OnColumn("OwnerId");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("MapId");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("OwnerId");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("InviteKey");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("Scopes");

            Create.Index()
                .OnTable("Invites")
                .OnColumn("MapId");
        }
    }
}
