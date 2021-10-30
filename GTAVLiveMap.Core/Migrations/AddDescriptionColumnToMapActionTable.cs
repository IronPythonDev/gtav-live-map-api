using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211030125900)]
    public class AddDescriptionColumnToMapActionTable : Migration
    {
        public const string TABLE_NAME = "MapActions";

        public override void Down()
        {
            Delete.Column("Description").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("Description")
                .AsString()
                .WithDefaultValue("");
        }
    }
}
