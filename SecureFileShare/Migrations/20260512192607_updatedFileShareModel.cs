using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureFileShare.Migrations
{
    /// <inheritdoc />
    public partial class updatedFileShareModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SharedFromId",
                table: "FileShares",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FileShares_SharedFromId",
                table: "FileShares",
                column: "SharedFromId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedFromId",
                table: "FileShares",
                column: "SharedFromId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileShares_AspNetUsers_SharedFromId",
                table: "FileShares");

            migrationBuilder.DropIndex(
                name: "IX_FileShares_SharedFromId",
                table: "FileShares");

            migrationBuilder.DropColumn(
                name: "SharedFromId",
                table: "FileShares");
        }
    }
}
