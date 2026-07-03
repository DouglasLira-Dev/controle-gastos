using System.Net;
using System.Text.Json;
using Serilog;

namespace DesafioControleGastos.API.Middlewares
{
    /// <summary>
    /// Middleware para tratamento seguro de erros
    /// </summary>
    /// <remarks>
    /// 🔒 SEGURANÇA:
    /// - NUNCA expõe stack traces para o cliente
    /// - Loga todos os erros internamente
    /// - Retorna mensagens genéricas para o usuário
    /// </remarks>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 🔒 LOG SEGURO: Registra o erro internamente com detalhes
            _logger.LogError(exception, 
                "Erro não tratado. Path: {Path}, Method: {Method}, IP: {IP}", 
                context.Request.Path,
                context.Request.Method,
                context.Connection.RemoteIpAddress?.ToString() ?? "Desconhecido");

            // 🔒 NUNCA expõe stack traces ou detalhes internos
            context.Response.ContentType = "application/json";
            
            var response = new 
            { 
                error = "Ocorreu um erro interno. Tente novamente mais tarde.",
                requestId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow
            };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var jsonResponse = JsonSerializer.Serialize(response);
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}