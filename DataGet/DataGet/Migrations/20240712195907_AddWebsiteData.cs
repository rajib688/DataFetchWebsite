using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataGet.Migrations
{
    public partial class AddWebsiteData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "websiteDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TradingCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LTP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    High = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Low = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosePrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YCP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Change = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Trade = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Volume = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_websiteDatas", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "websiteDatas");
        }
    }
}
