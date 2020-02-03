namespace ClubApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class LittleC : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ClubTypes", "Name", c => c.String(nullable: false, maxLength: 50));
        }

        public override void Down()
        {
            AlterColumn("dbo.ClubTypes", "Name", c => c.String(maxLength: 50));
        }
    }
}
