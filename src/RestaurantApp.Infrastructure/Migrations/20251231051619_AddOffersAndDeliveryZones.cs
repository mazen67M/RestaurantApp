using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOffersAndDeliveryZones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Offers_OfferId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "OfferUsages");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OfferId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OfferId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ApplicableOrderTypes",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CurrentUsageCount",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "DiscountValue",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "MinOrderAmount",
                table: "Offers",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "MaxUsagePerUser",
                table: "Offers",
                newName: "UsageCount");

            migrationBuilder.RenameColumn(
                name: "MaxUsageCount",
                table: "Offers",
                newName: "UsageLimit");

            migrationBuilder.RenameColumn(
                name: "MaxDiscountAmount",
                table: "Offers",
                newName: "MinimumOrderAmount");

            migrationBuilder.RenameColumn(
                name: "DiscountType",
                table: "Offers",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumDiscount",
                table: "Offers",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MenuItemId",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PerUserLimit",
                table: "Offers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeliveryZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinDistanceKm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxDistanceKm = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinimumOrderForFreeDelivery = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EstimatedMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryZones_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CategoryId",
                table: "Offers",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Offers_MenuItemId",
                table: "Offers",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryZones_BranchId",
                table: "DeliveryZones",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_MenuCategories_CategoryId",
                table: "Offers",
                column: "CategoryId",
                principalTable: "MenuCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_MenuItems_MenuItemId",
                table: "Offers",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_MenuCategories_CategoryId",
                table: "Offers");

            migrationBuilder.DropForeignKey(
                name: "FK_Offers_MenuItems_MenuItemId",
                table: "Offers");

            migrationBuilder.DropTable(
                name: "DeliveryZones");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CategoryId",
                table: "Offers");

            migrationBuilder.DropIndex(
                name: "IX_Offers_MenuItemId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "MaximumDiscount",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "PerUserLimit",
                table: "Offers");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Offers",
                newName: "MinOrderAmount");

            migrationBuilder.RenameColumn(
                name: "UsageLimit",
                table: "Offers",
                newName: "MaxUsageCount");

            migrationBuilder.RenameColumn(
                name: "UsageCount",
                table: "Offers",
                newName: "MaxUsagePerUser");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Offers",
                newName: "DiscountType");

            migrationBuilder.RenameColumn(
                name: "MinimumOrderAmount",
                table: "Offers",
                newName: "MaxDiscountAmount");

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OfferId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicableOrderTypes",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentUsageCount",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountValue",
                table: "Offers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Offers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OfferUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfferId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscountApplied = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfferUsages_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferUsages_Offers_OfferId",
                        column: x => x.OfferId,
                        principalTable: "Offers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferUsages_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OfferId",
                table: "Orders",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferUsages_OfferId",
                table: "OfferUsages",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferUsages_OrderId",
                table: "OfferUsages",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OfferUsages_UserId",
                table: "OfferUsages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Offers_OfferId",
                table: "Orders",
                column: "OfferId",
                principalTable: "Offers",
                principalColumn: "Id");
        }
    }
}
