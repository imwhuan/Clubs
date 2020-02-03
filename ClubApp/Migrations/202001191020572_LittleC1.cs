namespace ClubApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class LittleC1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClubNumbers", "Label", c => c.String(maxLength: 100));
            AddColumn("dbo.ClubNumbers", "CreateDate2", c => c.DateTime());
            AddColumn("dbo.ClubNumbers", "AuditID", c => c.Int(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.ClubNumbers", "AuditID");
            DropColumn("dbo.ClubNumbers", "CreateDate2");
            DropColumn("dbo.ClubNumbers", "Label");
        }
    }
}
