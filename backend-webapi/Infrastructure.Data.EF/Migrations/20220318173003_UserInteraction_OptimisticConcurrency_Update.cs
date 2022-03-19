using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.WebApi.Infrastructure.Data.EF.Migrations;

/*
 * "rowversion" SQL column cannot be changed by their nature.
 * As this information is not important to retain DROP-CREATE is OK.
 */
public partial class UserInteraction_OptimisticConcurrency_Update : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
           name: "RowVer",
           table: "UserInteraction");

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVer",
            table: "UserInteraction",
            type: "rowversion",
            rowVersion: true,
            nullable: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
           name: "RowVer",
           table: "UserInteraction");

        migrationBuilder.AddColumn<byte[]>(
            name: "RowVer",
            table: "UserInteraction",
            type: "rowversion",
            rowVersion: true,
            nullable: true);

    }
}

