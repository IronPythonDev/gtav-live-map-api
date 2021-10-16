using FluentMigrator;
using GTAVLiveMap.Domain.Enums;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211016173900)]
    public class AddMapScopesTable : Migration
    {
        private const string TABLE_NAME = "MapScopes";
        public override void Down()
        {
            Delete.Table(TABLE_NAME);
        }

        public override void Up()
        {
            Create.Table(TABLE_NAME)
                .WithColumn("Id")
                    .AsInt64()
                    .PrimaryKey()
                    .Identity()
                    .Unique()
                .WithColumn("Name")
                    .AsString()
                    .WithDefaultValue(MapScopeNameEnum.All.ToString());
        }
    }
}
