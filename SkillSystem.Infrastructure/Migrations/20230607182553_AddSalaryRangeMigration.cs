using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SkillSystem.Infrastructure.Migrations
{
    public partial class AddSalaryRangeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalaryRanges",
                columns: table => new
                {
                    GradeId = table.Column<int>(type: "integer", nullable: false),
                    MinimumWage = table.Column<decimal>(type: "numeric", nullable: false),
                    MaximumWage = table.Column<decimal>(type: "numeric", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryRanges", x => x.GradeId);
                });
    }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryRanges");
        }
    }
}
