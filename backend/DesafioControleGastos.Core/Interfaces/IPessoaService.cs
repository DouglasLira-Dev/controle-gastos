using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DesafioControleGastos.Core.DTOs;

namespace DesafioControleGastos.Core.Interfaces
{
    public interface IPessoaService
    {
        Task<IEnumerable<PessoaResponseDTO>> GetAllAsync();
        Task<PessoaResponseDTO?> GetByIdAsync(Guid id);
        Task<PessoaResponseDTO> CreateAsync(PessoaCreateDTO pessoaDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}