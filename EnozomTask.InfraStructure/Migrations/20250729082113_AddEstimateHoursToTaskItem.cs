using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnozomTask.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEstimateHoursToTaskItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "EstimateHours",
                table: "TaskItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimateHours",
                table: "TaskItems");
        }
    }
}
