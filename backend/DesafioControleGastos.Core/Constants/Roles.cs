namespace DesafioControleGastos.Core.Constants
{
    /// <summary>
    /// 🔒 Constantes para roles de usuários
    /// </summary>
    /// <remarks>
    /// Usado para controlar autorização nos endpoints
    /// </remarks>
    public static class Roles
    {
        /// <summary>
        /// Administrador - Tem acesso a todas as funcionalidades
        /// </summary>
        public const string Admin = "Admin";
        
        /// <summary>
        /// Usuário comum - Apenas funcionalidades básicas
        /// </summary>
        public const string User = "User";
        
        /// <summary>
        /// Visitante - Apenas leitura
        /// </summary>
        public const string Guest = "Guest";
    }
}