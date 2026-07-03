using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace DesafioControleGastos.API.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Transações
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TransacoesController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ILogger<TransacoesController> _logger;

        public TransacoesController(ITransacaoService transacaoService, ILogger<TransacoesController> logger)
        {
            _transacaoService = transacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todas as transações do sistema
        /// </summary>
        /// <returns>Lista de transações com dados das pessoas</returns>
        /// <response code="200">Lista de transações retornada com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TransacaoResponseDTO>), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var transacoes = await _transacaoService.GetAllAsync();
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as transações");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Lista transações de uma pessoa específica
        /// </summary>
        /// <param name="pessoaId">ID da pessoa</param>
        /// <returns>Lista de transações da pessoa</returns>
        /// <response code="200">Lista de transações retornada com sucesso</response>
        /// <response code="404">Pessoa não encontrada</response>
        [HttpGet("pessoa/{pessoaId}")]
        [ProducesResponseType(typeof(IEnumerable<TransacaoResponseDTO>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetByPessoa(Guid pessoaId)
        {
            try
            {
                var transacoes = await _transacaoService.GetByPessoaIdAsync(pessoaId);
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações da pessoa {PessoaId}", pessoaId);
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Cria uma nova transação
        /// </summary>
        /// <param name="transacaoDto">Dados da transação a ser criada</param>
        /// <returns>Transação criada com ID gerado</returns>
        /// <response code="201">Transação criada com sucesso</response>
        /// <response code="400">Dados inválidos ou menor de idade tentando criar receita</response>
        [HttpPost]
        [ProducesResponseType(typeof(TransacaoResponseDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] TransacaoCreateDTO transacaoDto)
        {
            try
            {
                var transacao = await _transacaoService.CreateAsync(transacaoDto);
                return CreatedAtAction(nameof(GetAll), new { id = transacao.Id }, transacao);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar transação");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }
    }
}