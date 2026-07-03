using DesafioControleGastos.Core.Models;

namespace DesafioControleGastos.Core.Interfaces
{
    /// <summary>
    /// Interface específica para operações de Pessoa no repositório
    /// </summary>
    public interface IPessoaRepository : IRepository<Pessoa>
    {
        Task<Pessoa?> GetByIdWithTransacoesAsync(Guid id);
        Task<IEnumerable<Pessoa>> GetAllWithTransacoesAsync();
    }
}