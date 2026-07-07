using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConfirmedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:order_status", "pending,confirmed,paid,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:order_status", "pending,paid,shipped,delivered,cancelled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:order_status", "pending,paid,shipped,delivered,cancelled")
                .OldAnnotation("Npgsql:Enum:order_status", "pending,confirmed,paid,shipped,delivered,cancelled");
        }
    }
}
