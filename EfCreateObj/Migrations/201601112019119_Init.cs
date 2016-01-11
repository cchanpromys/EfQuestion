namespace EfCreateObj
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chairs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Width = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Desks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomId = c.Int(),
                        HouseId = c.Int(),
                        ChairId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chairs", t => t.ChairId)
                .ForeignKey("dbo.Houses", t => t.HouseId)
                .ForeignKey("dbo.Rooms", t => t.RoomId)
                .Index(t => t.RoomId)
                .Index(t => t.HouseId)
                .Index(t => t.ChairId);
            
            CreateTable(
                "dbo.Houses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HouseId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Houses", t => t.HouseId)
                .Index(t => t.HouseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rooms", "HouseId", "dbo.Houses");
            DropForeignKey("dbo.Desks", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.Desks", "HouseId", "dbo.Houses");
            DropForeignKey("dbo.Desks", "ChairId", "dbo.Chairs");
            DropIndex("dbo.Rooms", new[] { "HouseId" });
            DropIndex("dbo.Desks", new[] { "ChairId" });
            DropIndex("dbo.Desks", new[] { "HouseId" });
            DropIndex("dbo.Desks", new[] { "RoomId" });
            DropTable("dbo.Rooms");
            DropTable("dbo.Houses");
            DropTable("dbo.Desks");
            DropTable("dbo.Chairs");
        }
    }
}
