using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatedticket2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_AspNetUsers_FromUserId",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_AspNetUsers_ToUserId",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Tickets_TicketId",
                table: "Feedback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback");

            migrationBuilder.RenameTable(
                name: "Feedback",
                newName: "Feedbacks");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_ToUserId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_TicketId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_FromUserId",
                table: "Feedbacks",
                newName: "IX_Feedbacks_FromUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_FromUserId",
                table: "Feedbacks",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_AspNetUsers_ToUserId",
                table: "Feedbacks",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Tickets_TicketId",
                table: "Feedbacks",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_FromUserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_AspNetUsers_ToUserId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Tickets_TicketId",
                table: "Feedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks");

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                newName: "Feedback");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_ToUserId",
                table: "Feedback",
                newName: "IX_Feedback_ToUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_TicketId",
                table: "Feedback",
                newName: "IX_Feedback_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_FromUserId",
                table: "Feedback",
                newName: "IX_Feedback_FromUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_AspNetUsers_FromUserId",
                table: "Feedback",
                column: "FromUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_AspNetUsers_ToUserId",
                table: "Feedback",
                column: "ToUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Tickets_TicketId",
                table: "Feedback",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
