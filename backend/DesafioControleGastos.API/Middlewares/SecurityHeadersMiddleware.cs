namespace DesafioControleGastos.API.Middlewares
{
    /// <summary>
    /// Middleware para adicionar headers de segurança
    /// </summary>
    /// <remarks>
    /// 🔒 SEGURANÇA:
    /// - CSP (Content Security Policy)
    /// - XSS Protection
    /// - Clickjacking Protection
    /// - MIME Type Sniffing Protection
    /// </remarks>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 🔒 Content Security Policy
            context.Response.Headers.Add("Content-Security-Policy", 
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' data:;");

            // 🔒 Previne XSS
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

            // 🔒 Previne Clickjacking
            context.Response.Headers.Add("X-Frame-Options", "DENY");

            // 🔒 Previne MIME Type Sniffing
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            // 🔒 Referrer Policy
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

            // 🔒 Permissions Policy
            context.Response.Headers.Add("Permissions-Policy", 
                "geolocation=(), microphone=(), camera=(), payment=(), usb=()");

            await _next(context);
        }
    }
}