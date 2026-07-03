using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DesafioControleGastos.API.Controllers
{
    /// <summary>
    /// Controller para consulta de totais financeiros
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TotaisController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;
        private readonly ILogger<TotaisController> _logger;

        public TotaisController(ITransacaoService transacaoService, ILogger<TotaisController> logger)
        {
            _transacaoService = transacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém totais gerais do sistema e por pessoa
        /// </summary>
        /// <returns>Totais por pessoa e totais gerais</returns>
        /// <response code="200">Totais calculados com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(TotalGeralDTO), 200)]
        public async Task<IActionResult> GetTotais()
        {
            try
            {
                var totais = await _transacaoService.GetTotaisGeraisAsync();
                return Ok(totais);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar totais gerais");
                return StatusCode(500, "Erro interno ao processar a requisição");
            }
        }
    }
}