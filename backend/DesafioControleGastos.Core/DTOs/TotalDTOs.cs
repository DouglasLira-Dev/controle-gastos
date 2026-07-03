using System;
using System.Collections.Generic;

namespace DesafioControleGastos.Core.DTOs
{
    public class TotalPorPessoaDTO
    {
        public Guid PessoaId { get; set; }
        public string NomePessoa { get; set; } = string.Empty;
        public int Idade { get; set; }
        public decimal TotalReceitas { get; set; }
        public decimal TotalDespesas { get; set; }
        public decimal Saldo => TotalReceitas - TotalDespesas;
    }

    public class TotalGeralDTO
    {
        public decimal TotalReceitasGeral { get; set; }
        public decimal TotalDespesasGeral { get; set; }
        public decimal SaldoLiquido => TotalReceitasGeral - TotalDespesasGeral;
        public List<TotalPorPessoaDTO> TotaisPorPessoa { get; set; } = new();
    }
}