using System.Text.RegularExpressions;

namespace DesafioControleGastos.Core.Utils
{
    /// <summary>
    /// Utilitário para sanitização de inputs
    /// </summary>
    /// <remarks>
    /// 🔒 SEGURANÇA:
    /// - Remove tags HTML
    /// - Remove scripts maliciosos
    /// - Remove caracteres perigosos
    /// - Previne XSS e injeção de código
    /// </remarks>
    public static class InputSanitizer
    {
        /// <summary>
        /// Sanitiza um input removendo caracteres e tags perigosas
        /// </summary>
        public static string Sanitize(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 🔒 Remove HTML tags
            input = Regex.Replace(input, @"<[^>]*>", string.Empty);
            
            // 🔒 Remove scripts (qualquer tag script)
            input = Regex.Replace(input, @"<script.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            // 🔒 Remove eventos JavaScript (onclick, onload, etc)
            input = Regex.Replace(input, @"\bon\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);
            
            // 🔒 Remove caracteres perigosos
            input = Regex.Replace(input, @"[<>""'%;()&+]", string.Empty);
            
            // 🔒 Remove múltiplos espaços
            input = Regex.Replace(input, @"\s+", " ");

            return input.Trim();
        }

        /// <summary>
        /// Sanitiza uma descrição (permitindo mais caracteres)
        /// </summary>
        public static string SanitizeDescription(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 🔒 Remove HTML tags e scripts
            input = Regex.Replace(input, @"<[^>]*>", string.Empty);
            input = Regex.Replace(input, @"<script.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            // 🔒 Remove eventos JavaScript
            input = Regex.Replace(input, @"\bon\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);
            
            // 🔒 Permite caracteres especiais comuns (vírgula, ponto, etc)
            input = Regex.Replace(input, @"[<>""'%&+]", string.Empty);

            return input.Trim();
        }
    }
}