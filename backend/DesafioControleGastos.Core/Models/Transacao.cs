using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DesafioControleGastos.Core.Models
{
    public enum TipoTransacao
    {
        Receita = 0,
        Despesa = 1
    }

    public class Transacao
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 200 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O tipo da transação é obrigatório")]
        public TipoTransacao Tipo { get; set; }

        [Required(ErrorMessage = "A pessoa é obrigatória")]
        public Guid PessoaId { get; set; }

        [ForeignKey(nameof(PessoaId))]
        public virtual Pessoa Pessoa { get; set; } = null!;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public Transacao() { }

        public Transacao(string descricao, decimal valor, TipoTransacao tipo, Guid pessoaId)
        {
            Id = Guid.NewGuid();
            Descricao = descricao;
            Valor = valor;
            Tipo = tipo;
            PessoaId = pessoaId;
            DataCriacao = DateTime.UtcNow;
        }

        [JsonIgnore]
        public bool IsReceita => Tipo == TipoTransacao.Receita;

        [JsonIgnore]
        public bool IsDespesa => Tipo == TipoTransacao.Despesa;
    }
}