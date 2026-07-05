using AutoMapper;
using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using DesafioControleGastos.Core.Models;
using DesafioControleGastos.Core.Validators;
using FluentValidation;

namespace DesafioControleGastos.Core.Services
{
    /// <summary>
    /// Serviço para operações de negócio relacionadas a Transações
    /// </summary>
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository; // 🔧 MUDOU para ITransacaoRepository
        private readonly IPessoaService _pessoaService;
        private readonly IMapper _mapper;
        private readonly IValidator<TransacaoCreateDTO> _validator;

        public TransacaoService(
            ITransacaoRepository transacaoRepository, // 🔧 MUDOU
            IPessoaService pessoaService,
            IMapper mapper,
            IValidator<TransacaoCreateDTO> validator)
        {
            _transacaoRepository = transacaoRepository;
            _pessoaService = pessoaService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<TransacaoResponseDTO>> GetAllAsync()
        {
            var transacoes = await _transacaoRepository.GetAllWithPessoaAsync(); // 🔧 CORRIGIDO
            return _mapper.Map<IEnumerable<TransacaoResponseDTO>>(transacoes);
        }

        public async Task<IEnumerable<TransacaoResponseDTO>> GetByPessoaIdAsync(Guid pessoaId)
        {
            var transacoes = await _transacaoRepository.GetByPessoaIdWithPessoaAsync(pessoaId); // 🔧 CORRIGIDO
            return _mapper.Map<IEnumerable<TransacaoResponseDTO>>(transacoes);
        }

        public async Task<TransacaoResponseDTO> CreateAsync(TransacaoCreateDTO transacaoDto)
        {
            // 1. Validar os dados (inclui regra de menor de idade)
            var validationResult = await _validator.ValidateAsync(transacaoDto);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // 2. Mapear e criar a transação
            var transacao = _mapper.Map<Transacao>(transacaoDto);
            var created = await _transacaoRepository.AddAsync(transacao);

            // 3. Buscar a transação com a pessoa incluída para o response
            var createdWithPessoa = await _transacaoRepository
                .FindAsync(t => t.Id == created.Id);
            
            var result = _mapper.Map<TransacaoResponseDTO>(createdWithPessoa.FirstOrDefault());
            return result;
        }

        public async Task<TotalGeralDTO> GetTotaisGeraisAsync()
        {
            var pessoas = await _pessoaService.GetAllAsync();
            var totaisPorPessoa = new List<TotalPorPessoaDTO>();

            decimal totalReceitasGeral = 0;
            decimal totalDespesasGeral = 0;

            foreach (var pessoa in pessoas)
            {
                totalReceitasGeral += pessoa.TotalReceitas;
                totalDespesasGeral += pessoa.TotalDespesas;

                totaisPorPessoa.Add(new TotalPorPessoaDTO
                {
                    PessoaId = pessoa.Id,
                    NomePessoa = pessoa.Nome,
                    Idade = pessoa.Idade,
                    TotalReceitas = pessoa.TotalReceitas,
                    TotalDespesas = pessoa.TotalDespesas
                });
            }

            return new TotalGeralDTO
            {
                TotalReceitasGeral = totalReceitasGeral,
                TotalDespesasGeral = totalDespesasGeral,
                TotaisPorPessoa = totaisPorPessoa
            };
        }
    }
}