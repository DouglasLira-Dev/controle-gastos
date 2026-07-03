using DesafioControleGastos.Core.Interfaces;
using DesafioControleGastos.Core.Models;
using DesafioControleGastos.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace DesafioControleGastos.Infra.Repositories
{
    /// <summary>
    /// Repository específico para Pessoa
    /// </summary>
    public class PessoaRepository : Repository<Pessoa>, IPessoaRepository
    {
        public PessoaRepository(AppDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtém uma pessoa com suas transações incluídas
        /// </summary>
        public async Task<Pessoa?> GetByIdWithTransacoesAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Transacoes)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Obtém todas as pessoas com suas transações
        /// </summary>
        public async Task<IEnumerable<Pessoa>> GetAllWithTransacoesAsync()
        {
            return await _dbSet
                .Include(p => p.Transacoes)
                .ToListAsync();
        }
    }
}