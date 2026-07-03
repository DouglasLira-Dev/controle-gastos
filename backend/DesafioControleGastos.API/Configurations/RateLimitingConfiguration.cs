using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace DesafioControleGastos.API.Configurations
{
    /// <summary>
    /// Configuração de Rate Limiting
    /// </summary>
    /// <remarks>
    /// 🔒 SEGURANÇA:
    /// - Limita requisições por IP
    /// - Previne ataques de força bruta
    /// - Protege contra DDoS
    /// </remarks>
    public static class RateLimitingConfiguration
    {
        public static void AddRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // 🔒 Limite global por IP
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext => RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 100, // 100 requisições
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1) // por minuto
                        }));

                // 🔒 Limites específicos para endpoints sensíveis
                options.AddPolicy("CreateUser", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 10, // 10 criações
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(5) // por 5 minutos
                        }));

                options.AddPolicy("DeleteUser", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5, // 5 deleções
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(10) // por 10 minutos
                        }));

                // 🔒 Retorna mensagem amigável quando limite é excedido
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new 
                    { 
                        error = "Muitas requisições. Tente novamente em alguns minutos.",
                        retryAfter = "60"
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
                };
            });
        }
    }
}