using FluentMigrator;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20201016104400)]
    public class AddUniqueConstraintToMapId : Migration
    {
        public override void Down()
        {
            Alter.Table("Maps")
            .AlterColumn("Id")
            .AsGuid();
        }

        public override void Up()
        {
            Alter.Table("Maps")
                .AlterColumn("Id")
                .AsGuid()
                .Unique();
        }
    }
}
