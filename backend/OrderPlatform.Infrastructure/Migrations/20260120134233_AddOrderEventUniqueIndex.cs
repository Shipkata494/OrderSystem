using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderEventUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OrderEvents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_OrderEvents_OrderId_Type",
                table: "OrderEvents",
                columns: new[] { "OrderId", "Type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderEvents_OrderId_Type",
                table: "OrderEvents");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OrderEvents",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
