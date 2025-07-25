using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticketApp.Migrations
{
    /// <inheritdoc />
    public partial class fixTicketCommentsTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketsId",
                table: "TicketComments");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_TicketsId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "TicketsId",
                table: "TicketComments");

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_TicketId",
                table: "TicketComments",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_TicketId",
                table: "TicketComments");

            migrationBuilder.AddColumn<int>(
                name: "TicketsId",
                table: "TicketComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_TicketsId",
                table: "TicketComments",
                column: "TicketsId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketsId",
                table: "TicketComments",
                column: "TicketsId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
