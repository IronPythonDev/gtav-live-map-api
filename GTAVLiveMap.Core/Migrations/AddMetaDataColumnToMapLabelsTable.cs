using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211027163000)]
    public class AddMetaDataColumnToMapLabelsTable : Migration
    {
        public const string TABLE_NAME = "MapLabels";

        public override void Down()
        {
            Delete.Column("MapId").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("MetaData")
                .AsString()
                .WithDefaultValue("{}");
        }
    }
}
