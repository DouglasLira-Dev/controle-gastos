using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DesafioControleGastos.API.Controllers
{
    /// <summary>
    /// 🔒 Controller para autenticação
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// 🔒 Realiza login do usuário
        /// </summary>
        /// <response code="200">Login realizado com sucesso</response>
        /// <response code="401">Credenciais inválidas</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDTO), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao realizar login");
                return StatusCode(500, new { error = "Erro interno ao processar login" });
            }
        }

        /// <summary>
        /// 🔒 Registra um novo usuário
        /// </summary>
        /// <response code="200">Usuário registrado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDTO), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao registrar usuário");
                return StatusCode(500, new { error = "Erro interno ao registrar usuário" });
            }
        }

        /// <summary>
        /// 🔒 Renova o token JWT
        /// </summary>
        /// <response code="200">Token renovado com sucesso</response>
        /// <response code="401">Refresh token inválido</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(AuthResponseDTO), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao renovar token");
                return StatusCode(500, new { error = "Erro interno ao renovar token" });
            }
        }

        /// <summary>
        /// 🔒 Realiza logout
        /// </summary>
        /// <response code="200">Logout realizado com sucesso</response>
        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            try
            {
                await _authService.LogoutAsync(refreshToken);
                return Ok(new { message = "Logout realizado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao realizar logout");
                return StatusCode(500, new { error = "Erro interno ao realizar logout" });
            }
        }
    }
}