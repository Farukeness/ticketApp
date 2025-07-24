using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ticketApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Tickets_TicketId",
                table: "TicketAttachments");

            migrationBuilder.DropIndex(
                name: "IX_TicketAttachments_TicketId",
                table: "TicketAttachments");

            migrationBuilder.AddColumn<int>(
                name: "TicketsIdId",
                table: "TicketAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachments_TicketsIdId",
                table: "TicketAttachments",
                column: "TicketsIdId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Tickets_TicketsIdId",
                table: "TicketAttachments",
                column: "TicketsIdId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAttachments_Tickets_TicketsIdId",
                table: "TicketAttachments");

            migrationBuilder.DropIndex(
                name: "IX_TicketAttachments_TicketsIdId",
                table: "TicketAttachments");

            migrationBuilder.DropColumn(
                name: "TicketsIdId",
                table: "TicketAttachments");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAttachments_TicketId",
                table: "TicketAttachments",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAttachments_Tickets_TicketId",
                table: "TicketAttachments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
