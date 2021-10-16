using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211610175900)]
    public class DeleteRolesColumnFromMapMembersTable : Migration
    {
        public override void Down()
        {
            Create.Column("Roles")
                .OnTable("MapMembers")
                .AsString()
                .WithDefaultValue("");
        }

        public override void Up()
        {
            Delete.Column("Roles").FromTable("MapMembers");
        }
    }
}
