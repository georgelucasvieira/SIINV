using API_Investimentos.Application.Commands.Simulacao;
using FluentValidation;

namespace API_Investimentos.Application.Validators;

/// <summary>
/// Validator para SimularInvestimentoCommand
/// </summary>
public class SimularInvestimentoCommandValidator : AbstractValidator<SimularInvestimentoCommand>
{
    public SimularInvestimentoCommandValidator()
    {
        RuleFor(x => x.ClienteId)
            .GreaterThan(0)
            .WithMessage("ClienteId deve ser maior que zero");

        RuleFor(x => x.Valor)
            .GreaterThan(0)
            .WithMessage("Valor deve ser maior que zero")
            .LessThanOrEqualTo(999_999_999_999.99m)
            .WithMessage("Valor não pode exceder R$ 999.999.999.999,99");

        RuleFor(x => x.PrazoMeses)
            .GreaterThan(0)
            .WithMessage("Prazo deve ser maior que zero")
            .LessThanOrEqualTo(600)
            .WithMessage("Prazo não pode ser maior que 600 meses (50 anos)");

        RuleFor(x => x.TipoProduto)
            .NotEmpty()
            .WithMessage("Tipo de produto é obrigatório")
            .Must(BeValidTipoProduto)
            .WithMessage("Tipo de produto inválido. Valores aceitos: CDB, TesouroSelic, TesouroPrefixado, TesouroIPCA, LCI, LCA, Fundo");
    }

    private bool BeValidTipoProduto(string tipoProduto)
    {
        var tiposValidos = new[] { "CDB", "TesouroSelic", "TesouroPrefixado", "TesouroIPCA", "LCI", "LCA", "Fundo" };
        return tiposValidos.Contains(tipoProduto, StringComparer.OrdinalIgnoreCase);
    }
}
