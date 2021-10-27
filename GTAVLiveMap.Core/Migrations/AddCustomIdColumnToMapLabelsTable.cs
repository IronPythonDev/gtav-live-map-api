using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211027195000)]
    public class AddCustomIdColumnToMapLabelsTable : Migration
    {//CustomId
        public const string TABLE_NAME = "MapLabels";

        public override void Down()
        {
            Delete.Column("CustomId").FromTable(TABLE_NAME);
        }

        public override void Up()
        {
            Alter.Table(TABLE_NAME)
                .AddColumn("CustomId")
                .AsString()
                .Nullable();
        }
    }
}
