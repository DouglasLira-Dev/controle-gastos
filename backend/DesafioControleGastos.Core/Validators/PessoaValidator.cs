using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Utils;
using FluentValidation;

namespace DesafioControleGastos.Core.Validators
{
    public class PessoaCreateValidator : AbstractValidator<PessoaCreateDTO>
    {
        public PessoaCreateValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome é obrigatório")
                .Length(2, 100).WithMessage("O nome deve ter entre 2 e 100 caracteres")
                .Matches(@"^[a-zA-ZáàâãéèêíïóôõöúçñÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇÑ\s]+$")
                .WithMessage("O nome deve conter apenas letras e espaços")
                .Must(nome => !string.IsNullOrWhiteSpace(nome))
                .WithMessage("O nome não pode conter apenas espaços em branco")
                // 🔒 SANITIZAÇÃO: Aplica sanitização antes da validação
                .Must(nome => InputSanitizer.Sanitize(nome) == nome)
                .WithMessage("O nome contém caracteres inválidos");

            RuleFor(p => p.Idade)
                .GreaterThanOrEqualTo(0).WithMessage("A idade deve ser maior ou igual a zero")
                .LessThanOrEqualTo(150).WithMessage("A idade deve ser menor ou igual a 150");
        }
    }
}