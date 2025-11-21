using API_Investimentos.Domain.Entities;
using API_Investimentos.Domain.Enums;
using API_Investimentos.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API_Investimentos.Infrastructure.Data;

/// <summary>
/// Classe para inicializar o banco de dados com dados seed
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(InvestimentosDbContext context, ILogger logger)
    {
        try
        {
            // Criar banco de dados se não existir
            await context.Database.MigrateAsync();
            logger.LogInformation("Iniciando seed do banco de dados...");
            
            await SeedProdutos(context, logger);
            await SeedClientes(context, logger); // Realiza seed de clientes e perfis de risco

            logger.LogInformation("Seed do banco de dados concluído com sucesso.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao realizar seed do banco de dados");
            throw;
        }
    }

    public static async Task SeedProdutos(InvestimentosDbContext context, ILogger logger)
    {
        // Verificar se já existem produtos
        if (await context.Produtos.AnyAsync())
        {
            logger.LogInformation("Banco de dados já contém produtos, seed ignorado.");
            return;
        }

        // Criar produtos de exemplo
        var produtos = new List<Produto>
            {
                // CDBs
                new Produto(
                    nome: "CDB Banco XYZ 2026",
                    tipo: TipoProduto.CDB,
                    nivelRisco: NivelRisco.Baixo,
                    taxaRentabilidade: Percentual.CriarDePercentual(12.5m),
                    valorMinimo: Dinheiro.Criar(1000m),
                    prazoMinimoMeses: 6,
                    liquidezDiaria: false,
                    isentoIR: false),

                new Produto(
                    nome: "CDB Alta Rentabilidade",
                    tipo: TipoProduto.CDB,
                    nivelRisco: NivelRisco.Medio,
                    taxaRentabilidade: Percentual.CriarDePercentual(14.0m),
                    valorMinimo: Dinheiro.Criar(5000m),
                    prazoMinimoMeses: 12,
                    liquidezDiaria: false,
                    isentoIR: false),

                // Tesouro Direto
                new Produto(
                    nome: "Tesouro Selic 2027",
                    tipo: TipoProduto.TesouroSelic,
                    nivelRisco: NivelRisco.MuitoBaixo,
                    taxaRentabilidade: Percentual.CriarDePercentual(11.75m),
                    valorMinimo: Dinheiro.Criar(100m),
                    prazoMinimoMeses: 1,
                    liquidezDiaria: true,
                    isentoIR: false),

                new Produto(
                    nome: "Tesouro Prefixado 2029",
                    tipo: TipoProduto.TesouroPrefixado,
                    nivelRisco: NivelRisco.Baixo,
                    taxaRentabilidade: Percentual.CriarDePercentual(12.0m),
                    valorMinimo: Dinheiro.Criar(100m),
                    prazoMinimoMeses: 12,
                    liquidezDiaria: false,
                    isentoIR: false),

                new Produto(
                    nome: "Tesouro IPCA+ 2035",
                    tipo: TipoProduto.TesouroIPCA,
                    nivelRisco: NivelRisco.Baixo,
                    taxaRentabilidade: Percentual.CriarDePercentual(6.5m), // Taxa real
                    valorMinimo: Dinheiro.Criar(100m),
                    prazoMinimoMeses: 12,
                    liquidezDiaria: false,
                    isentoIR: false),

                // LCI/LCA
                new Produto(
                    nome: "LCI Banco ABC",
                    tipo: TipoProduto.LCI,
                    nivelRisco: NivelRisco.Baixo,
                    taxaRentabilidade: Percentual.CriarDePercentual(10.5m),
                    valorMinimo: Dinheiro.Criar(10000m),
                    prazoMinimoMeses: 12,
                    liquidezDiaria: false,
                    isentoIR: true),

                new Produto(
                    nome: "LCA Agronegócio Plus",
                    tipo: TipoProduto.LCA,
                    nivelRisco: NivelRisco.Baixo,
                    taxaRentabilidade: Percentual.CriarDePercentual(10.8m),
                    valorMinimo: Dinheiro.Criar(15000m),
                    prazoMinimoMeses: 18,
                    liquidezDiaria: false,
                    isentoIR: true),

                // Fundos
                new Produto(
                    nome: "Fundo Renda Fixa Premium",
                    tipo: TipoProduto.Fundo,
                    nivelRisco: NivelRisco.Medio,
                    taxaRentabilidade: Percentual.CriarDePercentual(11.0m),
                    valorMinimo: Dinheiro.Criar(500m),
                    prazoMinimoMeses: 1,
                    liquidezDiaria: true,
                    isentoIR: false),

                new Produto(
                    nome: "Fundo Multimercado Estratégia",
                    tipo: TipoProduto.Fundo,
                    nivelRisco: NivelRisco.Alto,
                    taxaRentabilidade: Percentual.CriarDePercentual(15.0m),
                    valorMinimo: Dinheiro.Criar(1000m),
                    prazoMinimoMeses: 6,
                    liquidezDiaria: true,
                    isentoIR: false)
            };

        // Configurar taxas de administração para fundos
        produtos[7].DefinirTaxaAdministracao(Percentual.CriarDePercentual(1.5m));
        produtos[8].DefinirTaxaAdministracao(Percentual.CriarDePercentual(2.0m));
        produtos[8].DefinirTaxaPerformance(Percentual.CriarDePercentual(20.0m));

        // Adicionar produtos ao contexto
        await context.Produtos.AddRangeAsync(produtos);
        await context.SaveChangesAsync();

        logger.LogInformation("{Quantidade} produtos criados.", produtos.Count);
    }

    public static async Task SeedClientes(InvestimentosDbContext context, ILogger logger)
    {
        // Verificar se já existem clientes
        if (await context.Clientes.AnyAsync())
        {
            logger.LogInformation("Banco de dados já contém clientes, seed ignorado.");
            return;
        }

        // Criar clientes de exemplo
        var clientes = new List<Cliente>
            {
                new Cliente(
                    nome: "João Silva Santos",
                    cpf: "12345678901",
                    telefone: "(11) 99999-1111"),

                new Cliente(
                    nome: "Maria Oliveira Costa",
                    cpf: "23456789012",
                    telefone: "(11) 99999-2222"),

                new Cliente(
                    nome: "Pedro Souza Lima",
                    cpf: "34567890123",
                    telefone: "(21) 98888-3333"),

                new Cliente(
                    nome: "Ana Paula Ferreira",
                    cpf: "45678901234",
                    telefone: "(31) 97777-4444"),

                new Cliente(
                    nome: "Carlos Eduardo Mendes",
                    cpf: "56789012345",
                    telefone: "(41) 96666-5555")
            };

        await context.Clientes.AddRangeAsync(clientes);
        await context.SaveChangesAsync();

        logger.LogInformation("{Quantidade} clientes criados.", clientes.Count);

        await SeedPerfisRisco(context, logger, clientes);
    }

    public static async Task SeedPerfisRisco(InvestimentosDbContext context, ILogger logger, List<Cliente> clientes)
    {
        // Carregar clientes se não fornecidos
        if (clientes == null)
        {
            clientes = await context.Clientes.ToListAsync();
        }

        if (clientes.Count < 5)
        {
            logger.LogWarning("Número insuficiente de clientes para criar perfis de risco. Perfis não criados.");
            return;
        }
    
        // Verificar se já existem perfis de risco
        if (await context.PerfisRisco.AnyAsync())
        {
            logger.LogInformation("Banco de dados já contém perfis de risco, seed ignorado.");
            return;
        }

        // Criar perfis de risco para os clientes
        var perfisRisco = new List<PerfilRisco>
            {
                new PerfilRisco(
                    clienteId: clientes[0].Id,
                    pontuacao: 25,
                    fatoresCalculo: "{\"experiencia\": \"baixa\", \"objetivos\": \"preservacao\", \"horizonte\": \"curto\"}"),

                new PerfilRisco(
                    clienteId: clientes[1].Id,
                    pontuacao: 50,
                    fatoresCalculo: "{\"experiencia\": \"media\", \"objetivos\": \"crescimento\", \"horizonte\": \"medio\"}"),

                new PerfilRisco(
                    clienteId: clientes[2].Id,
                    pontuacao: 75,
                    fatoresCalculo: "{\"experiencia\": \"alta\", \"objetivos\": \"maximizar\", \"horizonte\": \"longo\"}"),

                new PerfilRisco(
                    clienteId: clientes[3].Id,
                    pontuacao: 40,
                    fatoresCalculo: "{\"experiencia\": \"media\", \"objetivos\": \"equilibrio\", \"horizonte\": \"medio\"}"),

                new PerfilRisco(
                    clienteId: clientes[4].Id,
                    pontuacao: 85,
                    fatoresCalculo: "{\"experiencia\": \"alta\", \"objetivos\": \"maximizar\", \"horizonte\": \"longo\"}")
            };

        await context.PerfisRisco.AddRangeAsync(perfisRisco);
        await context.SaveChangesAsync();

        logger.LogInformation("{Quantidade} perfis de risco criados.", perfisRisco.Count);
    }
}
