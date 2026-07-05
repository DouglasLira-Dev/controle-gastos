using DesafioControleGastos.Core.Interfaces;
using DesafioControleGastos.Core.Models;
using DesafioControleGastos.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DesafioControleGastos.Infra.Repositories
{
    /// <summary>
    /// Repository específico para Transação
    /// </summary>
    public class TransacaoRepository : Repository<Transacao>, ITransacaoRepository
    {
        public TransacaoRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtém todas as transações com a pessoa incluída
        /// </summary>
        public async Task<IEnumerable<Transacao>> GetAllWithPessoaAsync()
        {
            return await _dbSet
                .Include(t => t.Pessoa) 
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();
        }

        /// <summary>
        /// Obtém transações de uma pessoa específica com a pessoa incluída
        /// </summary>
        public async Task<IEnumerable<Transacao>> GetByPessoaIdWithPessoaAsync(Guid pessoaId)
        {
            return await _dbSet
                .Include(t => t.Pessoa) 
                .Where(t => t.PessoaId == pessoaId)
                .OrderByDescending(t => t.DataCriacao)
                .ToListAsync();
        }
    }
}