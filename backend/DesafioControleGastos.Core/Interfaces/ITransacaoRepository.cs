using DesafioControleGastos.Core.Models;

namespace DesafioControleGastos.Core.Interfaces
{
    /// <summary>
    /// Interface específica para operações de Transação no repositório
    /// </summary>
    public interface ITransacaoRepository : IRepository<Transacao>
    {
        /// <summary>
        /// Obtém todas as transações com a pessoa incluída
        /// </summary>
        Task<IEnumerable<Transacao>> GetAllWithPessoaAsync();
        
        /// <summary>
        /// Obtém transações de uma pessoa específica com a pessoa incluída
        /// </summary>
        Task<IEnumerable<Transacao>> GetByPessoaIdWithPessoaAsync(Guid pessoaId);
    }
}