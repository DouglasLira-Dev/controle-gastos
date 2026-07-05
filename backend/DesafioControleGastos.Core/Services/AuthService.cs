using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DesafioControleGastos.Core.Constants;
using DesafioControleGastos.Core.DTOs;
using DesafioControleGastos.Core.Interfaces;
using DesafioControleGastos.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace DesafioControleGastos.Core.Services
{
    /// <summary>
    /// 🔒 Serviço de autenticação JWT
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IRepository<Usuario> _usuarioRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IRepository<Usuario> usuarioRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 🔒 Realiza login do usuário
        /// </summary>
        public async Task<AuthResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            // 1. Buscar usuário pelo username
            var usuario = (await _usuarioRepository.FindAsync(u => 
                u.Username == request.Username || u.Email == request.Username))
                .FirstOrDefault();

            if (usuario == null)
            {
                _logger.LogWarning($"❌ Tentativa de login falhou: Usuário não encontrado - {request.Username}");
                throw new UnauthorizedAccessException("Usuário ou senha inválidos");
            }

            // 2. Verificar se usuário está ativo
            if (!usuario.IsActive)
            {
                _logger.LogWarning($"❌ Tentativa de login em conta inativa: {usuario.Username}");
                throw new UnauthorizedAccessException("Conta desativada");
            }

            // 3. 🔒 Verificar senha (com hash e salt)
            if (!VerifyPasswordHash(request.Password, usuario.PasswordHash, usuario.PasswordSalt))
            {
                _logger.LogWarning($"❌ Tentativa de login falhou: Senha incorreta - {usuario.Username}");
                throw new UnauthorizedAccessException("Usuário ou senha inválidos");
            }

            // 4. Atualizar último login
            usuario.LastLoginAt = DateTime.UtcNow;
            await _usuarioRepository.UpdateAsync(usuario);

            // 5. 🔒 Gerar tokens
            var token = GenerateJwtToken(usuario);
            var refreshToken = GenerateRefreshToken();

            // 6. Salvar refresh token
            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation($"✅ Login bem-sucedido: {usuario.Username} ({usuario.Role})");

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Username = usuario.Username,
                Email = usuario.Email,
                Role = usuario.Role
            };
        }

        /// <summary>
        /// 🔒 Registra um novo usuário
        /// </summary>
        public async Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO request)
        {
            // 1. Validar se as senhas coincidem
            if (request.Password != request.ConfirmPassword)
            {
                throw new ArgumentException("As senhas não coincidem");
            }

            // 2. Verificar se usuário já existe
            var existingUser = (await _usuarioRepository.FindAsync(u => 
                u.Username == request.Username || u.Email == request.Email))
                .FirstOrDefault();

            if (existingUser != null)
            {
                throw new ArgumentException("Usuário ou email já cadastrado");
            }

            // 3. 🔒 Criar hash da senha com salt
            var (hash, salt) = CreatePasswordHash(request.Password);

            // 4. Criar novo usuário
            var usuario = new Usuario
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = Roles.User,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // 5. Salvar no banco
            await _usuarioRepository.AddAsync(usuario);

            _logger.LogInformation($"✅ Novo usuário registrado: {usuario.Username} ({usuario.Role})");

            // 6. 🔒 Gerar tokens
            var token = GenerateJwtToken(usuario);
            var refreshToken = GenerateRefreshToken();

            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _usuarioRepository.UpdateAsync(usuario);

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Username = usuario.Username,
                Email = usuario.Email,
                Role = usuario.Role
            };
        }

        /// <summary>
        /// 🔒 Renova o token JWT
        /// </summary>
        public async Task<AuthResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO request)
        {
            // 1. Buscar usuário pelo refresh token
            var usuario = (await _usuarioRepository.FindAsync(u => 
                u.RefreshToken == request.RefreshToken))
                .FirstOrDefault();

            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Refresh token inválido");
            }

            // 2. Verificar se refresh token expirou
            if (usuario.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Refresh token expirado");
            }

            // 3. 🔒 Gerar novo token
            var token = GenerateJwtToken(usuario);
            var refreshToken = GenerateRefreshToken();

            // 4. Atualizar refresh token
            usuario.RefreshToken = refreshToken;
            usuario.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _usuarioRepository.UpdateAsync(usuario);

            _logger.LogInformation($"✅ Token renovado para: {usuario.Username}");

            return new AuthResponseDTO
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Username = usuario.Username,
                Email = usuario.Email,
                Role = usuario.Role
            };
        }

        /// <summary>
        /// 🔒 Realiza logout (invalida refresh token)
        /// </summary>
        public async Task LogoutAsync(string refreshToken)
        {
            var usuario = (await _usuarioRepository.FindAsync(u => 
                u.RefreshToken == refreshToken))
                .FirstOrDefault();

            if (usuario != null)
            {
                usuario.RefreshToken = null;
                usuario.RefreshTokenExpiryTime = null;
                await _usuarioRepository.UpdateAsync(usuario);
                _logger.LogInformation($"✅ Logout realizado para: {usuario.Username}");
            }
        }

        // ============================================
        // 🔒 MÉTODOS PRIVADOS DE SEGURANÇA
        // ============================================

        /// <summary>
        /// 🔒 Gera token JWT
        /// </summary>
        private string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada")));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 🔒 Gera refresh token aleatório
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// 🔒 Cria hash da senha com salt
        /// </summary>
        private (string Hash, string Salt) CreatePasswordHash(string password)
        {
            using var hmac = new HMACSHA512();
            var salt = Convert.ToBase64String(hmac.Key);
            var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return (hash, salt);
        }

        /// <summary>
        /// 🔒 Verifica se a senha corresponde ao hash
        /// </summary>
        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            return computedHash == storedHash;
        }
    }
}