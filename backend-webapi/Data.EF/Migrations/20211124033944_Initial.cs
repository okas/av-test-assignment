using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.WebApi.Data.EF.Migrations;

public partial class Initial : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "UserInteraction",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsOpen = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserInteraction", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserInteraction");
    }
}

