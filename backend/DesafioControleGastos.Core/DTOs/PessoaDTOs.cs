using System;
using System.Collections.Generic;

namespace DesafioControleGastos.Core.DTOs
{
    public class PessoaCreateDTO
    {
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
    }

    public class PessoaResponseDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Idade { get; set; }
        public bool IsMenorDeIdade { get; set; }
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo => TotalReceitas - TotalDespesas;
        public List<TransacaoResponseDTO> Transacoes { get; set; } = new();
    }
}