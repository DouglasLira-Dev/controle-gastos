namespace DesafioControleGastos.Core.DTOs
{
    /// <summary>
    /// 🔒 DTO para requisição de login
    /// </summary>
    public class LoginRequestDTO
    {
        /// <summary>
        /// Nome de usuário ou email
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// 🔒 DTO para requisição de registro
    /// </summary>
    public class RegisterRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// 🔒 DTO para resposta de autenticação
    /// </summary>
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    /// <summary>
    /// 🔒 DTO para renovação de token
    /// </summary>
    public class RefreshTokenRequestDTO
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}