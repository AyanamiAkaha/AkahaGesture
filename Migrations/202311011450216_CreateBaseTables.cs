namespace Akaha_Gesture.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBaseTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.images",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        path = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.sessions",
                c => new
                    {
                        start = c.DateTime(nullable: false),
                        end = c.DateTime(nullable: false),
                        n_images = c.Int(nullable: false),
                        sec_per_image = c.Int(nullable: false),
                        summary = c.String(),
                        Image_id = c.Int(),
                    })
                .PrimaryKey(t => t.start)
                .ForeignKey("dbo.images", t => t.Image_id)
                .Index(t => t.Image_id);
            
            CreateTable(
                "dbo.session_images",
                c => new
                    {
                        session_id = c.DateTime(nullable: false),
                        image_id = c.Int(nullable: false),
                        order = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.session_id, t.image_id })
                .ForeignKey("dbo.images", t => t.image_id, cascadeDelete: true)
                .ForeignKey("dbo.sessions", t => t.session_id, cascadeDelete: true)
                .Index(t => t.session_id)
                .Index(t => t.image_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.sessions", "Image_id", "dbo.images");
            DropForeignKey("dbo.session_images", "session_id", "dbo.sessions");
            DropForeignKey("dbo.session_images", "image_id", "dbo.images");
            DropIndex("dbo.session_images", new[] { "image_id" });
            DropIndex("dbo.session_images", new[] { "session_id" });
            DropIndex("dbo.sessions", new[] { "Image_id" });
            DropTable("dbo.session_images");
            DropTable("dbo.sessions");
            DropTable("dbo.images");
        }
    }
}
