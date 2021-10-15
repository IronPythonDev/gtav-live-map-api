using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20201016004400)]
    public class AddMapsTable : Migration
    {
        private const string TABLE_NAME = "Maps";

        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id")
                    .AsGuid()
                    .WithDefaultValue(SystemMethods.NewGuid)//CREATE EXTENSION "uuid-ossp";
                .WithColumn("Name")
                    .AsString()
                    .NotNullable()
                    .WithDefaultValue("Default Map Name")
                .WithColumn("ApiKey")
                    .AsString()
                    .Nullable()
                    .Unique()
                    .WithDefaultValue(null)
                .WithColumn("CreatedAt")
                    .AsDateTime()
                    .NotNullable()
                    .WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("MaxMembers")
                    .AsInt32()
                    .NotNullable()
                    .WithDefaultValue(10)
                .WithColumn("OwnerId")
                    .AsInt32()
                    .ForeignKey("Users", "Id").OnDelete(System.Data.Rule.Cascade);
        }
    }
}
