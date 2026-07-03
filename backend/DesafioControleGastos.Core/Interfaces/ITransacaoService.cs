using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesafioControleGastos.Core.DTOs;

namespace DesafioControleGastos.Core.Interfaces
{
    public interface ITransacaoService
    {
        Task<IEnumerable<TransacaoResponseDTO>> GetAllAsync();
        Task<IEnumerable<TransacaoResponseDTO>> GetByPessoaIdAsync(Guid pessoaId);
        Task<TransacaoResponseDTO> CreateAsync(TransacaoCreateDTO transacaoDto);
        Task<TotalGeralDTO> GetTotaisGeraisAsync();
    }
}