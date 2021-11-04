using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211104030900)]
    public class AddTelegramIDColumnForUser : Migration
    {
        public override void Down()
        {
            Delete.Column("TelegramID").FromTable("Users");
        }

        public override void Up()
        {
            Create.Column("TelegramID").OnTable("Users").AsString().Nullable().WithDefaultValue(null);
        }
    }
}
