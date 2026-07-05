using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DesafioControleGastos.API.Configurations
{
    /// <summary>
    /// Configuração de autenticação JWT
    /// </summary>
    /// <remarks>
    /// 🔒 SEGURANÇA:
    /// - Tokens assinados com chave secreta
    /// - Validação de emissor e audiência
    /// - Expiração configurável
    /// - HTTPS obrigatório em produção
    /// </remarks>
    public static class JwtConfiguration
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key não configurada"));

            // 🔒 Configura a autenticação
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            // 🔒 Configura a autorização (ADICIONAR ESTA LINHA!)
            services.AddAuthorization();
        }
    }
}