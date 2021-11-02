using FluentMigrator;
using GTAVLiveMap.Domain.Enums;
using System;

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
                .WithColumn("Id")
                    .AsInt64()
                    .PrimaryKey()
                    .Identity()
                .WithColumn("Email")
                    .AsString()
                    .Unique()
                .WithColumn("Roles")
                    .AsString()
                    .NotNullable()
                    .WithDefaultValue("User");

            Create.Table("SessionKeys")
                .WithColumn("Key")
                    .AsString()
                    .Unique()
                .WithColumn("CreatedAt")
                    .AsDateTime()
                    .WithDefaultValue(RawSql.Insert("CURRENT_TIMESTAMP"))
                .WithColumn("LastAt")
                    .AsDateTime()
                    .WithDefaultValue(RawSql.Insert("CURRENT_TIMESTAMP"))
                .WithColumn("UserAgent")
                    .AsString()
                    .Nullable()
                    .WithDefaultValue(null)
                .WithColumn("CreatedIP")
                    .AsString()
                    .Nullable()
                    .WithDefaultValue(null)
                .WithColumn("LastIP")
                    .AsString()
                    .Nullable()
                    .WithDefaultValue(null)
                .WithColumn("OwnerId")
                    .AsInt32()
                    .ForeignKey("Users", "Id")
                    .OnDelete(System.Data.Rule.Cascade);

            Execute.Sql(@"CREATE EXTENSION ""uuid-ossp""");

            Create.Table("Maps")
                .WithColumn("Id")
                    .AsGuid()
                    .Unique()
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
                    .ForeignKey("Users", "Id")
                    .OnDelete(System.Data.Rule.Cascade);

            Create.Table("MapScopes")
                .WithColumn("Id")
                    .AsInt64()
                    .PrimaryKey()
                    .Identity()
                    .Unique()
                .WithColumn("Name")
                    .AsString()
                    .WithDefaultValue(string.Join(';', Enum.GetNames(typeof(MapScopeNameEnum))));

            Create.Table("MapMembers")
                .WithColumn("Id")
                    .AsGuid()
                    .WithDefault(SystemMethods.NewGuid)
                .WithColumn("Roles")
                    .AsString()
                    .WithDefaultValue("")
                .WithColumn("InviteKey")
                    .AsString()
                    .Nullable()
                .WithColumn("MapId")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Scopes")
                    .AsString()
                    .WithDefaultValue(string.Join(';', Enum.GetNames(typeof(MapScopeNameEnum))))
                .WithColumn("OwnerId")
                    .AsInt64()
                    .NotNullable()
                    .ForeignKey("Users", "Id")
                    .OnDelete(System.Data.Rule.Cascade);

            Create.Table("Connections")
                .WithColumn("Id")
                    .AsGuid()
                    .WithDefaultValue(SystemMethods.NewGuid)
                .WithColumn("ConnectionId")
                    .AsString()
                    .WithDefaultValue("")
                .WithColumn("MapId")
                    .AsGuid()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade);

            Create.Table("MapLabels")
                .WithColumn("Id")
                    .AsGuid()
                    .Unique()
                    .WithDefault(SystemMethods.NewGuid)
                .WithColumn("Coordinates")
                    .AsString()
                    .WithDefaultValue("")
                .WithColumn("Type")
                    .AsInt32()
                    .WithDefaultValue(1)
                .WithColumn("CustomId")
                    .AsString()
                    .Nullable()
                .WithColumn("Icon")
                    .AsString()
                    .Nullable()
                    .WithDefaultValue("marker")
                .WithColumn("Vector3")
                    .AsString()
                    .WithDefaultValue("{}")
                .WithColumn("MapId")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("MetaData")
                    .AsString()
                    .WithDefaultValue("{}")
                .WithColumn("PopupBody")
                    .AsString()
                    .WithDefaultValue(null);


            Create.Table("Invites")
                .WithColumn("Id")
                    .AsGuid()
                    .Unique()
                    .WithDefault(SystemMethods.NewGuid)
                .WithColumn("Key")
                    .AsString()
                    .Unique()
                    .WithDefaultValue("")
                .WithColumn("Scopes")
                    .AsString()
                    .NotNullable()
                    .WithDefaultValue(string.Join(';', Enum.GetNames(typeof(MapScopeNameEnum))))
                .WithColumn("MapId")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("CreatedAt")
                    .AsDateTime()
                    .WithDefault(SystemMethods.CurrentDateTime);

            Create.Table("MapActions")
                .WithColumn("Id")
                    .AsGuid()
                    .WithDefaultValue(SystemMethods.NewGuid)
                .WithColumn("Name")
                    .AsString()
                    .NotNullable()
                .WithColumn("Description")
                    .AsString()
                    .WithDefaultValue("")
                .WithColumn("Source")
                    .AsString()
                    .Nullable()
                    .WithDefaultValue("")
                .WithColumn("MapId")
                    .AsGuid()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade);

            Create.Table("MapConfigs")
                .WithColumn("MapId")
                    .AsGuid()
                    .ForeignKey("Maps", "Id")
                    .OnDelete(System.Data.Rule.Cascade)
                .WithColumn("MaxInvites")
                    .AsInt32()
                    .WithDefaultValue(5)
                .WithColumn("MaxMembers")
                    .AsInt32()
                    .WithDefaultValue(10)
                .WithColumn("MaxActions")
                    .AsInt32()
                    .WithDefaultValue(5)
                .WithColumn("MaxObjects")
                    .AsInt32()
                    .WithDefaultValue(100);

            Create.Index()
                .OnTable("Maps")
                .OnColumn("OwnerId");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("MapId");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("OwnerId");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("InviteKey");

            Create.Index()
                .OnTable("MapMembers")
                .OnColumn("Scopes");

            Create.Index()
                .OnTable("Invites")
                .OnColumn("MapId");

            Insert.IntoTable("MapScopes")
                .Row(new { Name = MapScopeNameEnum.AddAction.ToString() })
                .Row(new { Name = MapScopeNameEnum.EditAction.ToString() })
                .Row(new { Name = MapScopeNameEnum.EmitAction.ToString() })
                .Row(new { Name = MapScopeNameEnum.CreateInvite.ToString() });
        }
    }
}
