using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class foreignKeyAddedCatagoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CatagoryId",
                table: "Product",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1,
                column: "CatagoryId",
                value: 18);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2,
                column: "CatagoryId",
                value: 18);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3,
                column: "CatagoryId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 4,
                column: "CatagoryId",
                value: 19);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 5,
                column: "CatagoryId",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 6,
                column: "CatagoryId",
                value: 20);

            migrationBuilder.CreateIndex(
                name: "IX_Product_CatagoryId",
                table: "Product",
                column: "CatagoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Catagories_CatagoryId",
                table: "Product",
                column: "CatagoryId",
                principalTable: "Catagories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Catagories_CatagoryId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_CatagoryId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CatagoryId",
                table: "Product");
        }
    }
}
