
using System;
using API_Investimentos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_Investimentos.Infrastructure.Migrations
{
    [DbContext(typeof(InvestimentosDbContext))]
    [Migration("20251121144336_AddClienteTable")]
    partial class AddClienteTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("API_Investimentos.Domain.Entities.Cliente", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("Cpf")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Excluido")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("ExcluidoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("Cpf")
                        .IsUnique()
                        .HasDatabaseName("IX_Clientes_Cpf");

                    b.HasIndex("Nome")
                        .HasDatabaseName("IX_Clientes_Nome");

                    b.ToTable("Clientes", (string)null);
                });

            modelBuilder.Entity("API_Investimentos.Domain.Entities.HistoricoInvestimento", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<long>("ClienteId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataInvestimento")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataResgate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DataVencimento")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Excluido")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("ExcluidoEm")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Rentabilidade")
                        .HasColumnType("decimal(10,6)");

                    b.Property<bool>("Resgatado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("TipoProduto")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Valor")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId")
                        .HasDatabaseName("IX_HistoricoInvestimentos_ClienteId");

                    b.HasIndex("DataInvestimento")
                        .HasDatabaseName("IX_HistoricoInvestimentos_DataInvestimento");

                    b.HasIndex("TipoProduto")
                        .HasDatabaseName("IX_HistoricoInvestimentos_TipoProduto");

                    b.HasIndex("ClienteId", "Resgatado")
                        .HasDatabaseName("IX_HistoricoInvestimentos_ClienteId_Resgatado");

                    b.ToTable("HistoricoInvestimentos", (string)null);
                });

            modelBuilder.Entity("API_Investimentos.Domain.Entities.PerfilRisco", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<long>("ClienteId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataProximaAvaliacao")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<bool>("Excluido")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("ExcluidoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("FatoresCalculo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Perfil")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Pontuacao")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId")
                        .IsUnique()
                        .HasDatabaseName("IX_PerfisRisco_ClienteId");

                    b.HasIndex("DataProximaAvaliacao")
                        .HasDatabaseName("IX_PerfisRisco_DataProximaAvaliacao");

                    b.ToTable("PerfisRisco", (string)null);
                });

            modelBuilder.Entity("API_Investimentos.Domain.Entities.Produto", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool>("Ativo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("Excluido")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("ExcluidoEm")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsentoIR")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool>("LiquidezDiaria")
                        .HasColumnType("bit");

                    b.Property<string>("NivelRisco")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int?>("PrazoMaximoMeses")
                        .HasColumnType("int");

                    b.Property<int>("PrazoMinimoMeses")
                        .HasColumnType("int");

                    b.Property<decimal?>("TaxaAdministracao")
                        .HasColumnType("decimal(10,6)")
                        .HasColumnName("TaxaAdministracao");

                    b.Property<decimal?>("TaxaPerformance")
                        .HasColumnType("decimal(10,6)")
                        .HasColumnName("TaxaPerformance");

                    b.Property<decimal>("TaxaRentabilidade")
                        .HasColumnType("decimal(10,6)")
                        .HasColumnName("TaxaRentabilidade");

                    b.Property<string>("Tipo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("ValorMinimo")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("ValorMinimo");

                    b.HasKey("Id");

                    b.HasIndex("Ativo")
                        .HasDatabaseName("IX_Produtos_Ativo");

                    b.HasIndex("Nome")
                        .HasDatabaseName("IX_Produtos_Nome");

                    b.HasIndex("Tipo")
                        .HasDatabaseName("IX_Produtos_Tipo");

                    b.HasIndex("Tipo", "Ativo")
                        .HasDatabaseName("IX_Produtos_Tipo_Ativo");

                    b.ToTable("Produtos", (string)null);
                });

            modelBuilder.Entity("API_Investimentos.Domain.Entities.Simulacao", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<decimal>("AliquotaIR")
                        .HasColumnType("decimal(10,6)");

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<long?>("AtualizadoPorId")
                        .HasColumnType("bigint");

                    b.Property<long>("ClienteId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<long?>("CriadoPorId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DataVencimento")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Excluido")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateTime?>("ExcluidoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("Observacoes")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("PrazoMeses")
                        .HasColumnType("int");

                    b.Property<long>("ProdutoId")
                        .HasColumnType("bigint");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("TaxaRentabilidadeEfetiva")
                        .HasColumnType("decimal(10,6)");

                    b.Property<decimal>("ValorFinalBruto")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ValorFinalLiquido")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ValorIR")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("ValorInvestido")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId")
                        .HasDatabaseName("IX_Simulacoes_ClienteId");

                    b.HasIndex("CriadoEm")
                        .HasDatabaseName("IX_Simulacoes_CriadoEm");

                    b.HasIndex("ProdutoId")
                        .HasDatabaseName("IX_Simulacoes_ProdutoId");

                    b.HasIndex("Status")
                        .HasDatabaseName("IX_Simulacoes_Status");

                    b.HasIndex("ClienteId", "CriadoEm")
                        .HasDatabaseName("IX_Simulacoes_ClienteId_CriadoEm");

                    b.ToTable("Simulacoes", (string)null);
                });

            modelBuilder.Entity("API_Investimentos.Domain.Entities.Simulacao", b =>
                {
                    b.HasOne("API_Investimentos.Domain.Entities.Produto", "Produto")
                        .WithMany()
                        .HasForeignKey("ProdutoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Produto");
                });
#pragma warning restore 612, 618
        }
    }
}
