using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tuckfirtle.Node.Migrations
{
    public partial class _001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blocks",
                columns: table => new
                {
                    BlockHash = table.Column<string>(nullable: false),
                    NetworkIdentifier = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Height = table.Column<string>(nullable: false),
                    Timestamp = table.Column<long>(nullable: false),
                    Nonce = table.Column<string>(nullable: false),
                    TargetDifficulty = table.Column<string>(nullable: false),
                    PreviousHash = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.BlockHash);
                });

            migrationBuilder.CreateTable(
                name: "Peers",
                columns: table => new
                {
                    PeerId = table.Column<Guid>(nullable: false),
                    Host = table.Column<string>(nullable: false),
                    Port = table.Column<int>(nullable: false),
                    TimeStamp = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peers", x => x.PeerId);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionHash = table.Column<string>(nullable: false),
                    BlockHash = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Timestamp = table.Column<long>(nullable: false),
                    BlockHash1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionHash);
                    table.ForeignKey(
                        name: "FK_Transactions_Blocks_BlockHash1",
                        column: x => x.BlockHash1,
                        principalTable: "Blocks",
                        principalColumn: "BlockHash",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionInputs",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionHash = table.Column<string>(nullable: false),
                    TransactionOutputIndex = table.Column<int>(nullable: false),
                    ScriptName = table.Column<string>(nullable: false),
                    ScriptValue = table.Column<string>(nullable: false),
                    TransactionHash1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionInputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionInputs_Transactions_TransactionHash1",
                        column: x => x.TransactionHash1,
                        principalTable: "Transactions",
                        principalColumn: "TransactionHash",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionOutputs",
                columns: table => new
                {
                    Id = table.Column<ulong>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TransactionHash = table.Column<string>(nullable: false),
                    TransactionOutputIndex = table.Column<int>(nullable: false),
                    ScriptName = table.Column<string>(nullable: false),
                    ScriptValue = table.Column<string>(nullable: false),
                    Amount = table.Column<string>(nullable: false),
                    TransactionHash1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionOutputs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionOutputs_Transactions_TransactionHash1",
                        column: x => x.TransactionHash1,
                        principalTable: "Transactions",
                        principalColumn: "TransactionHash",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionInputs_TransactionHash1",
                table: "TransactionInputs",
                column: "TransactionHash1");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionOutputs_TransactionHash1",
                table: "TransactionOutputs",
                column: "TransactionHash1");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BlockHash1",
                table: "Transactions",
                column: "BlockHash1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Peers");

            migrationBuilder.DropTable(
                name: "TransactionInputs");

            migrationBuilder.DropTable(
                name: "TransactionOutputs");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Blocks");
        }
    }
}
