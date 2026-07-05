using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DesafioControleGastos.Core.Constants;


namespace DesafioControleGastos.API.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de Pessoas
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔒 TODOS os endpoints exigem autenticação
    [Produces("application/json")]
    public class PessoasController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;
        private readonly ILogger<PessoasController> _logger;

        public PessoasController(IPessoaService pessoaService, ILogger<PessoasController> logger)
        {
            _pessoaService = pessoaService;
            _logger = logger;
        }

        /// <summary>
        /// Lista todas as pessoas cadastradas
        /// </summary>
        /// <returns>Lista de pessoas com seus totais</returns>
        /// <response code="200">Lista de pessoas retornada com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PessoaResponseDTO>), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pessoas = await _pessoaService.GetAllAsync();
                return Ok(pessoas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as pessoas");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Obtém uma pessoa específica por ID
        /// </summary>
        /// <param name="id">ID da pessoa</param>
        /// <returns>Dados da pessoa com suas transações</returns>
        /// <response code="200">Pessoa encontrada</response>
        /// <response code="404">Pessoa não encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PessoaResponseDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var pessoa = await _pessoaService.GetByIdAsync(id);
                if (pessoa == null)
                    return NotFound($"Pessoa com ID {id} não encontrada");

                return Ok(pessoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pessoa {Id}", id);
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Cria uma nova pessoa
        /// </summary>
        /// <param name="pessoaDto">Dados da pessoa a ser criada</param>
        /// <returns>Pessoa criada com ID gerado</returns>
        /// <response code="201">Pessoa criada com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(PessoaResponseDTO), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] PessoaCreateDTO pessoaDto)
        {
            try
            {
                var pessoa = await _pessoaService.CreateAsync(pessoaDto);
                return CreatedAtAction(nameof(GetById), new { id = pessoa.Id }, pessoa);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pessoa");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }

        /// <summary>
        /// Deleta uma pessoa e todas as suas transações
        /// </summary>
        /// <param name="id">ID da pessoa a ser deletada</param>
        /// <response code="204">Pessoa deletada com sucesso</response>
        /// <response code="404">Pessoa não encontrada</response>
        [Authorize(Roles = Roles.Admin)] //🔒 Apenas Admin pode deletar
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _pessoaService.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"Pessoa com ID {id} não encontrada");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar pessoa {Id}", id);
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }
    }
}