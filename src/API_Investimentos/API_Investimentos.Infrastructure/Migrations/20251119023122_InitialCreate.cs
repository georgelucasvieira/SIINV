using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Investimentos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricoInvestimentos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    TipoProduto = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rentabilidade = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    DataInvestimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Resgatado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DataResgate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Excluido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExcluidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoInvestimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfisRisco",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    Perfil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Pontuacao = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FatoresCalculo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataProximaAvaliacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Excluido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExcluidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisRisco", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NivelRisco = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TaxaRentabilidade = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    ValorMinimo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrazoMinimoMeses = table.Column<int>(type: "int", nullable: false),
                    PrazoMaximoMeses = table.Column<int>(type: "int", nullable: true),
                    LiquidezDiaria = table.Column<bool>(type: "bit", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TaxaAdministracao = table.Column<decimal>(type: "decimal(10,6)", nullable: true),
                    TaxaPerformance = table.Column<decimal>(type: "decimal(10,6)", nullable: true),
                    IsentoIR = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Excluido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExcluidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Simulacoes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<long>(type: "bigint", nullable: false),
                    ProdutoId = table.Column<long>(type: "bigint", nullable: false),
                    ValorInvestido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrazoMeses = table.Column<int>(type: "int", nullable: false),
                    DataVencimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorFinalBruto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorIR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorFinalLiquido = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxaRentabilidadeEfetiva = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    AliquotaIR = table.Column<decimal>(type: "decimal(10,6)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Observacoes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CriadoPorId = table.Column<long>(type: "bigint", nullable: true),
                    AtualizadoPorId = table.Column<long>(type: "bigint", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Excluido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExcluidoEm = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Simulacoes_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoInvestimentos_ClienteId",
                table: "HistoricoInvestimentos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoInvestimentos_ClienteId_Resgatado",
                table: "HistoricoInvestimentos",
                columns: new[] { "ClienteId", "Resgatado" });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoInvestimentos_DataInvestimento",
                table: "HistoricoInvestimentos",
                column: "DataInvestimento");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoInvestimentos_TipoProduto",
                table: "HistoricoInvestimentos",
                column: "TipoProduto");

            migrationBuilder.CreateIndex(
                name: "IX_PerfisRisco_ClienteId",
                table: "PerfisRisco",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisRisco_DataProximaAvaliacao",
                table: "PerfisRisco",
                column: "DataProximaAvaliacao");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Ativo",
                table: "Produtos",
                column: "Ativo");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Nome",
                table: "Produtos",
                column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Tipo",
                table: "Produtos",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_Tipo_Ativo",
                table: "Produtos",
                columns: new[] { "Tipo", "Ativo" });

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_ClienteId",
                table: "Simulacoes",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_ClienteId_CriadoEm",
                table: "Simulacoes",
                columns: new[] { "ClienteId", "CriadoEm" });

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_CriadoEm",
                table: "Simulacoes",
                column: "CriadoEm");

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_ProdutoId",
                table: "Simulacoes",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Simulacoes_Status",
                table: "Simulacoes",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoInvestimentos");

            migrationBuilder.DropTable(
                name: "PerfisRisco");

            migrationBuilder.DropTable(
                name: "Simulacoes");

            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}
