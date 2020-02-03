namespace ClubApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class BaseCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnnounceMents",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Label = c.String(maxLength: 500),
                    Title1 = c.String(maxLength: 100),
                    Title2 = c.String(maxLength: 100),
                    Content = c.String(maxLength: 1000),
                    ImgList = c.String(maxLength: 50),
                    CreateDate = c.DateTime(),
                    User_UserId = c.String(maxLength: 10),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserNumbers", t => t.User_UserId)
                .Index(t => t.User_UserId);

            CreateTable(
                "dbo.UserNumbers",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 10),
                    UserName = c.String(maxLength: 30),
                    HeadImg = c.String(maxLength: 500),
                    State = c.Int(),
                    OnlineState = c.Int(),
                    Class = c.String(maxLength: 30),
                    RelName = c.String(maxLength: 30),
                    Gender = c.Int(),
                    Birthday = c.DateTime(),
                    ShortDesc = c.String(maxLength: 100),
                    Desc = c.String(maxLength: 500),
                    CreateDate = c.DateTime(),
                    LoginDate = c.DateTime(),
                    Coloege_Id = c.Int(),
                })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Coloeges", t => t.Coloege_Id)
                .Index(t => t.Coloege_Id);

            CreateTable(
                "dbo.Coloeges",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 50),
                    State = c.Int(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ApplyAudits",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ApplicationDesc = c.String(maxLength: 500),
                    ApplicationFiled = c.String(maxLength: 500),
                    ApplyDate = c.DateTime(),
                    CheckState = c.Int(),
                    AuditDate = c.DateTime(),
                    AuditTimes = c.Int(),
                    ApplyUser_UserId = c.String(maxLength: 10),
                    Club_ClubId = c.String(maxLength: 10),
                    Type_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserNumbers", t => t.ApplyUser_UserId)
                .ForeignKey("dbo.ClubNumbers", t => t.Club_ClubId)
                .ForeignKey("dbo.ApplyTypes", t => t.Type_Id, cascadeDelete: true)
                .Index(t => t.ApplyUser_UserId)
                .Index(t => t.Club_ClubId)
                .Index(t => t.Type_Id);

            CreateTable(
                "dbo.ClubNumbers",
                c => new
                {
                    ClubId = c.String(nullable: false, maxLength: 10),
                    Name = c.String(maxLength: 50),
                    HeadImg = c.String(maxLength: 500),
                    ShortDesc = c.String(maxLength: 100),
                    Desc = c.String(maxLength: 500),
                    State = c.Int(),
                    CreateDate = c.DateTime(),
                    Type_Id = c.Int(),
                    User_UserId = c.String(maxLength: 10),
                })
                .PrimaryKey(t => t.ClubId)
                .ForeignKey("dbo.ClubTypes", t => t.Type_Id)
                .ForeignKey("dbo.UserNumbers", t => t.User_UserId)
                .Index(t => t.Type_Id)
                .Index(t => t.User_UserId);

            CreateTable(
                "dbo.ClubTypes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 50),
                    Enable = c.Int(),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.ApplyTypes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 20),
                    Enable = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.AuditDetails",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    ApplyId = c.Int(nullable: false),
                    CheckState = c.Int(),
                    AuditDesc = c.String(maxLength: 500),
                    AuditDate = c.DateTime(),
                    AuditUser_UserId = c.String(maxLength: 10),
                    FromUser_UserId = c.String(maxLength: 10),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserNumbers", t => t.AuditUser_UserId)
                .ForeignKey("dbo.UserNumbers", t => t.FromUser_UserId)
                .Index(t => t.AuditUser_UserId)
                .Index(t => t.FromUser_UserId);

            CreateTable(
                "dbo.UserClubs",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CreateDate = c.DateTime(),
                    Desc = c.String(maxLength: 500),
                    Enable = c.Int(),
                    Club_ClubId = c.String(maxLength: 10),
                    Status_Id = c.Int(),
                    User_UserId = c.String(maxLength: 10),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClubNumbers", t => t.Club_ClubId)
                .ForeignKey("dbo.UserStatus", t => t.Status_Id)
                .ForeignKey("dbo.UserNumbers", t => t.User_UserId)
                .Index(t => t.Club_ClubId)
                .Index(t => t.Status_Id)
                .Index(t => t.User_UserId);

            CreateTable(
                "dbo.UserStatus",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(maxLength: 50),
                    Enable = c.Int(),
                })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.UserClubs", "User_UserId", "dbo.UserNumbers");
            DropForeignKey("dbo.UserClubs", "Status_Id", "dbo.UserStatus");
            DropForeignKey("dbo.UserClubs", "Club_ClubId", "dbo.ClubNumbers");
            DropForeignKey("dbo.AuditDetails", "FromUser_UserId", "dbo.UserNumbers");
            DropForeignKey("dbo.AuditDetails", "AuditUser_UserId", "dbo.UserNumbers");
            DropForeignKey("dbo.ApplyAudits", "Type_Id", "dbo.ApplyTypes");
            DropForeignKey("dbo.ApplyAudits", "Club_ClubId", "dbo.ClubNumbers");
            DropForeignKey("dbo.ClubNumbers", "User_UserId", "dbo.UserNumbers");
            DropForeignKey("dbo.ClubNumbers", "Type_Id", "dbo.ClubTypes");
            DropForeignKey("dbo.ApplyAudits", "ApplyUser_UserId", "dbo.UserNumbers");
            DropForeignKey("dbo.AnnounceMents", "User_UserId", "dbo.UserNumbers");
            DropForeignKey("dbo.UserNumbers", "Coloege_Id", "dbo.Coloeges");
            DropIndex("dbo.UserClubs", new[] { "User_UserId" });
            DropIndex("dbo.UserClubs", new[] { "Status_Id" });
            DropIndex("dbo.UserClubs", new[] { "Club_ClubId" });
            DropIndex("dbo.AuditDetails", new[] { "FromUser_UserId" });
            DropIndex("dbo.AuditDetails", new[] { "AuditUser_UserId" });
            DropIndex("dbo.ClubNumbers", new[] { "User_UserId" });
            DropIndex("dbo.ClubNumbers", new[] { "Type_Id" });
            DropIndex("dbo.ApplyAudits", new[] { "Type_Id" });
            DropIndex("dbo.ApplyAudits", new[] { "Club_ClubId" });
            DropIndex("dbo.ApplyAudits", new[] { "ApplyUser_UserId" });
            DropIndex("dbo.UserNumbers", new[] { "Coloege_Id" });
            DropIndex("dbo.AnnounceMents", new[] { "User_UserId" });
            DropTable("dbo.UserStatus");
            DropTable("dbo.UserClubs");
            DropTable("dbo.AuditDetails");
            DropTable("dbo.ApplyTypes");
            DropTable("dbo.ClubTypes");
            DropTable("dbo.ClubNumbers");
            DropTable("dbo.ApplyAudits");
            DropTable("dbo.Coloeges");
            DropTable("dbo.UserNumbers");
            DropTable("dbo.AnnounceMents");
        }
    }
}
