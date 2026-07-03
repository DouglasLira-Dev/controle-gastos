using AutoMapper;
using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using DesafioControleGastos.Core.Models;
using DesafioControleGastos.Core.Validators;
using FluentValidation;

namespace DesafioControleGastos.Core.Services
{
    /// <summary>
    /// Serviço para operações de negócio relacionadas a Pessoas
    /// </summary>
    public class PessoaService : IPessoaService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<PessoaCreateDTO> _validator;

        public PessoaService(
            IPessoaRepository pessoaRepository,
            IMapper mapper,
            IValidator<PessoaCreateDTO> validator)
        {
            _pessoaRepository = pessoaRepository;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<PessoaResponseDTO>> GetAllAsync()
        {
            var pessoas = await _pessoaRepository.GetAllWithTransacoesAsync();
            return _mapper.Map<IEnumerable<PessoaResponseDTO>>(pessoas);
        }

        public async Task<PessoaResponseDTO?> GetByIdAsync(Guid id)
        {
            var pessoa = await _pessoaRepository.GetByIdWithTransacoesAsync(id);
            return pessoa == null ? null : _mapper.Map<PessoaResponseDTO>(pessoa);
        }

        public async Task<PessoaResponseDTO> CreateAsync(PessoaCreateDTO pessoaDto)
        {
            // Validar os dados
            var validationResult = await _validator.ValidateAsync(pessoaDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var pessoa = _mapper.Map<Pessoa>(pessoaDto);
            var created = await _pessoaRepository.AddAsync(pessoa);
            
            return _mapper.Map<PessoaResponseDTO>(created);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            if (!await _pessoaRepository.ExistsAsync(id))
                return false;

            await _pessoaRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _pessoaRepository.ExistsAsync(id);
        }
    }
}