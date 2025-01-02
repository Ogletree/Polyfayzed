using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspireApp.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class Markets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Market",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    EnableOrderBook = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Closed = table.Column<bool>(type: "bit", nullable: false),
                    Archived = table.Column<bool>(type: "bit", nullable: false),
                    AcceptingOrders = table.Column<bool>(type: "bit", nullable: false),
                    AcceptingOrderTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MinimumOrderSize = table.Column<int>(type: "int", nullable: false),
                    MinimumTickSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConditionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketSlug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDateIso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GameStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SecondsDelay = table.Column<int>(type: "int", nullable: false),
                    Fpmm = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MakerBaseFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TakerBaseFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NotificationsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    NegRisk = table.Column<bool>(type: "bit", nullable: false),
                    NegRiskMarketId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NegRiskRequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Is5050Outcome = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Market", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    MarketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tag_Market_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Market",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Token",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    MarketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Winner = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Token_Market_MarketId",
                        column: x => x.MarketId,
                        principalTable: "Market",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_MarketId",
                table: "Tag",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_Token_MarketId",
                table: "Token",
                column: "MarketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tag");

            migrationBuilder.DropTable(
                name: "Token");

            migrationBuilder.DropTable(
                name: "Market");
        }
    }
}
