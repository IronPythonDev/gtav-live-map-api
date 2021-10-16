using FluentMigrator;
using GTAVLiveMap.Domain.Enums;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20211610174300)]
    public class InsertDefaultScopes : Migration
    {
        private const string TABLE_NAME = "MapScopes";

        public override void Down()
        {
            Delete.FromTable(TABLE_NAME).AllRows();
        }

        public override void Up()
        {
            Insert.IntoTable(TABLE_NAME)
                .Row(new { Name = MapScopeNameEnum.AddAction.ToString() })
                .Row(new { Name = MapScopeNameEnum.EditAction.ToString() })
                .Row(new { Name = MapScopeNameEnum.EmitAction.ToString() });
        }
    }
}
