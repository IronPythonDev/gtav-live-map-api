using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211027205200)]
    public class AddConnectionTable : Migration
    {
        public const string TABLE_NAME = "Connections";
        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id")
                    .AsGuid()
                    .WithDefaultValue(SystemMethods.NewGuid)
                .WithColumn("ConnectionId")
                    .AsString()
                    .WithDefaultValue("")
                .WithColumn("MapId")
                    .AsGuid()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade);
        }
    }
}
