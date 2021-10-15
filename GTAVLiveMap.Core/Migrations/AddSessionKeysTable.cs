using FluentMigrator;
using System;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(20201014181501)]
    public class AddSessionKeysTable : Migration
    {
        public override void Down()
        {
            Delete.Table("SessionKeys");
        }

        public override void Up()
        {
            Create.Table("SessionKeys")
                .WithColumn("Key").AsString().Unique()
                .WithColumn("CreatedAt").AsDateTime().WithDefaultValue(RawSql.Insert("CURRENT_TIMESTAMP"))
                .WithColumn("LastAt").AsDateTime().WithDefaultValue(RawSql.Insert("CURRENT_TIMESTAMP"))
                .WithColumn("UserAgent").AsString().Nullable().WithDefaultValue(null)
                .WithColumn("CreatedIP").AsString().Nullable().WithDefaultValue(null)
                .WithColumn("LastIP").AsString().Nullable().WithDefaultValue(null)
                .WithColumn("OwnerId").AsInt32().ForeignKey("Users", "Id").OnDelete(System.Data.Rule.Cascade);
        }
    }
}
