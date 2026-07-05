using DesafioControleGastos.Core.DTOs;

namespace DesafioControleGastos.Core.Interfaces
{
    /// <summary>
    /// 🔒 Interface para serviço de autenticação
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request);

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request);

        /// <summary>
        /// Renova o token JWT usando refresh token
        /// </summary>
        Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request);

        /// <summary>
        /// Realiza logout (invalida refresh token)
        /// </summary>
        Task LogoutAsync(string refreshToken);
    }
}