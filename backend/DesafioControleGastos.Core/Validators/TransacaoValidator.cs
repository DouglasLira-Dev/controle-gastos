using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using FluentValidation;

namespace DesafioControleGastos.Core.Validators
{
    /// <summary>
    /// Validador para criação de transação
    /// </summary>
    /// <remarks>
    /// Regras de validação:
    /// - Descrição: obrigatória, 3-200 caracteres
    /// - Valor: maior que zero, máximo 2 casas decimais
    /// - PessoaId: deve existir no sistema
    /// - ⚠️ REGRA DE NEGÓCIO: Menores de 18 só podem ter despesas
    /// </remarks>
    public class TransacaoCreateValidator : AbstractValidator<TransacaoCreateDTO>
    {
        private readonly IPessoaService _pessoaService;

        public TransacaoCreateValidator(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;

            // ============================================
            // VALIDAÇÃO DA DESCRIÇÃO
            // ============================================
            RuleFor(t => t.Descricao)
                .NotEmpty().WithMessage("A descrição é obrigatória")
                .Length(3, 200).WithMessage("A descrição deve ter entre 3 e 200 caracteres")
                .Must(descricao => !descricao.Contains("<") && !descricao.Contains(">"))
                .WithMessage("A descrição contém caracteres inválidos")
                .Must(descricao => !string.IsNullOrWhiteSpace(descricao))
                .WithMessage("A descrição não pode conter apenas espaços em branco");

            // ============================================
            // VALIDAÇÃO DO VALOR
            // ============================================
            RuleFor(t => t.Valor)
                .GreaterThan(0).WithMessage("O valor deve ser maior que zero")
                .LessThanOrEqualTo(999999999.99M)
                .WithMessage("O valor não pode exceder R$ 999.999.999,99")
                .Must(valor => decimal.Round(valor, 2) == valor)
                .WithMessage("O valor deve ter no máximo 2 casas decimais");

            // ============================================
            // VALIDAÇÃO DA PESSOA
            // ============================================
            RuleFor(t => t.PessoaId)
                .NotEmpty().WithMessage("A pessoa é obrigatória")
                .MustAsync(BeValidPessoa)
                .WithMessage("A pessoa informada não existe no sistema");

            // ============================================
            // ⚠️ REGRA DE NEGÓCIO CRÍTICA
            // Menores de idade só podem ter despesas
            // ============================================
            RuleFor(t => t)
                .MustAsync(BeValidTipoParaIdade)
                .WithMessage("Menores de idade (menos de 18 anos) só podem cadastrar despesas");
        }

        /// <summary>
        /// Valida se a pessoa existe no sistema
        /// </summary>
        private async Task<bool> BeValidPessoa(Guid pessoaId, CancellationToken cancellationToken)
        {
            try
            {
                var pessoa = await _pessoaService.GetByIdAsync(pessoaId);
                return pessoa != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ⚠️ REGRA DE NEGÓCIO: Verifica se a transação é válida para a idade da pessoa
        /// </summary>
        private async Task<bool> BeValidTipoParaIdade(TransacaoCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                var pessoa = await _pessoaService.GetByIdAsync(dto.PessoaId);
                
                // Se a pessoa existe e é menor de idade (< 18)
                if (pessoa != null && pessoa.IsMenorDeIdade)
                {
                    // ⚠️ BLOQUEIA: Menores de idade só podem ter despesas
                    if (dto.Tipo == Core.Models.TipoTransacao.Receita)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}