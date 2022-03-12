using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.WebApi.Infrastructure.Data.EF.Migrations;
public partial class UserInteraction_OptimisticConcurrency_Support : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "RowVer",
            table: "UserInteraction",
            type: "rowversion",
            rowVersion: true,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RowVer",
            table: "UserInteraction");
    }
}
