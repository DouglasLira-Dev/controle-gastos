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
            var headers = context.Response.Headers;

            // 🔒 Content Security Policy
            headers.Append("Content-Security-Policy", 
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "font-src 'self' data:;");

            // 🔒 Previne XSS
            headers.Append("X-XSS-Protection", "1; mode=block");

            // 🔒 Previne Clickjacking
            headers.Append("X-Frame-Options", "DENY");

            // 🔒 Previne MIME Type Sniffing
            headers.Append("X-Content-Type-Options", "nosniff");

            // 🔒 Referrer Policy
            headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // 🔒 Permissions Policy
            headers.Append("Permissions-Policy", 
                "geolocation=(), microphone=(), camera=(), payment=(), usb=()");

            await _next(context);
        }
    }
}