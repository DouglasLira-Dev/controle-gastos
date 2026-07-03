using DesafioControleGastos.Core.DTOs;
using FluentValidation;

namespace DesafioControleGastos.Core.Validators
{
    /// <summary>
    /// Validador para criação/atualização de pessoa
    /// </summary>
    /// <remarks>
    /// Regras de validação:
    /// - Nome: obrigatório, 2-100 caracteres, apenas letras e espaços
    /// - Idade: entre 0 e 150 anos
    /// </remarks>
    public class PessoaCreateValidator : AbstractValidator<PessoaCreateDTO>
    {
        public PessoaCreateValidator()
        {
            // ============================================
            // VALIDAÇÃO DO NOME
            // ============================================
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres")
                .Matches(@"^[a-zA-ZáàâãéèêíïóôõöúçñÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇÑ\s]+$")
                .WithMessage("O nome deve conter apenas letras e espaços")
                .Must(nome => !string.IsNullOrWhiteSpace(nome))
                .WithMessage("O nome não pode conter apenas espaços em branco");

            // ============================================
            // VALIDAÇÃO DA IDADE
            // ============================================
            RuleFor(p => p.Idade)
                .GreaterThanOrEqualTo(0).WithMessage("A idade deve ser maior ou igual a zero")
                .LessThanOrEqualTo(150).WithMessage("A idade deve ser menor ou igual a 150");
        }
    }
}