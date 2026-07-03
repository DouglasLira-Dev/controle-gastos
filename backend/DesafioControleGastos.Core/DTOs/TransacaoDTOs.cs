using System;
using DesafioControleGastos.Core.Models;

namespace DesafioControleGastos.Core.DTOs
{
    public class TransacaoCreateDTO
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public Guid PessoaId { get; set; }
    }

    public class TransacaoResponseDTO
    {
        public Guid Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public TipoTransacao Tipo { get; set; }
        public string TipoDescricao => Tipo == TipoTransacao.Receita ? "Receita" : "Despesa";
        public Guid PessoaId { get; set; }
        public string NomePessoa { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
    }
}