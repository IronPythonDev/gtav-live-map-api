using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GTAVLiveMap.Core.Migrations
{
    [Migration(2020101512100)]
    public class AddRolesColumnToUserTable : Migration
    {
        public override void Down()
        {
            Delete.Column("Roles").FromTable("Users");
        }

        public override void Up()
        {
            Alter.Table("Users")
                .AddColumn("Roles").AsString().NotNullable().WithDefaultValue("User;");
        }
    }
}
