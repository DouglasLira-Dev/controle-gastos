using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DesafioControleGastos.Core.Models
{
    /// <summary>
    /// Representa uma pessoa cadastrada no sistema de controle de gastos
    /// </summary>
    public class Pessoa
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150 anos")]
        public int Idade { get; set; }

        [JsonIgnore]
        public bool IsMenorDeIdade => Idade < 18;

        public virtual ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();

        public Pessoa() { }

        public Pessoa(string nome, int idade)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Idade = idade;
        }
    }
}