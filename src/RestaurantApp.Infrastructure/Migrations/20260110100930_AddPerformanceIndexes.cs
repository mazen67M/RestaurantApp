using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add index on Order.UserId for faster user order queries
            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            // Add index on Order.BranchId for faster branch-specific queries
            migrationBuilder.CreateIndex(
                name: "IX_Orders_BranchId",
                table: "Orders",
                column: "BranchId");

            // Add index on Order.Status for faster status filtering
            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                table: "Orders",
                column: "Status");

            // Add composite index on Order.Status and CreatedAt for dashboard queries
            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_CreatedAt",
                table: "Orders",
                columns: new[] { "Status", "CreatedAt" });

            // Add index on User.Email for faster login queries (if not already exists)
            // Note: This might already exist from Identity, but adding for completeness
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: false);

            // Add index on MenuItem.CategoryId for faster category filtering
            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CategoryId_IsAvailable",
                table: "MenuItems",
                columns: new[] { "CategoryId", "IsAvailable" });

            // Add index on Review.MenuItemId for faster item review queries
            migrationBuilder.CreateIndex(
                name: "IX_Reviews_MenuItemId_IsApproved",
                table: "Reviews",
                columns: new[] { "MenuItemId", "IsApproved" });

            // Add index on Favorite.UserId for faster user favorites queries
            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            // Add index on Address.UserId for faster user address queries
            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BranchId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_CreatedAt",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_MenuItems_CategoryId_IsAvailable",
                table: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_MenuItemId_IsApproved",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses");
        }
    }
}
