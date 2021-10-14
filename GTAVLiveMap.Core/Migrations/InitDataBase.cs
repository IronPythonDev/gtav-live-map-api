using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20201014123100)]
    public class InitDataBase : Migration
    {
        public override void Down()
        {
            Delete.Table("Users");
        }

        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Email").AsString().Unique();
        }
    }
}
