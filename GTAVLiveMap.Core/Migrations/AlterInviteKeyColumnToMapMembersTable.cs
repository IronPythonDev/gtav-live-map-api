using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211017191800)]
    public class AlterInviteKeyColumnToMapMembersTable : Migration
    {
        private const string COLUMN_NAME = "InviteKey";
        private const string TABLE_NAME = "MapMembers";

        public override void Down()
        {
            Delete.Column(COLUMN_NAME).FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Column(COLUMN_NAME).OnTable(TABLE_NAME)
                .AsString()
                .Nullable();
        }
    }
}
