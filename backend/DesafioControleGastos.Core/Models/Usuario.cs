using System.ComponentModel.DataAnnotations;
using DesafioControleGastos.Core.Constants;

namespace DesafioControleGastos.Core.Models
{
    /// <summary>
    /// 🔒 Representa um usuário do sistema
    /// </summary>
    /// <remarks>
    /// Usado para autenticação e autorização
    /// </remarks>
    public class Usuario
    {
        /// <summary>
        /// Identificador único do usuário (GUID)
        /// </summary>
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Nome de usuário (único)
        /// </summary>
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "O usuário deve ter entre 3 e 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário (único)
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 🔒 Hash da senha (NUNCA armazenar em texto plano)
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// 🔒 Salt da senha (para segurança adicional)
        /// </summary>
        public string PasswordSalt { get; set; } = string.Empty;

        /// <summary>
        /// Role do usuário (Admin, User, Guest)
        /// </summary>
        [Required]
        public string Role { get; set; } = Roles.User;

        /// <summary>
        /// Indica se o usuário está ativo
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Data de criação do usuário
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Último login do usuário
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 🔒 Refresh Token para renovação de sessão
        /// </summary>
        public string? RefreshToken { get; set; }

        /// <summary>
        /// 🔒 Data de expiração do Refresh Token
        /// </summary>
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}