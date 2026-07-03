using DesafioControleGastos.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DesafioControleGastos.Infra.Data
{
    /// <summary>
    /// Contexto do Entity Framework Core para o sistema de gastos
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }

        /// <summary>
        /// Configuração do modelo com regras de negócio e relacionamentos
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================================
            // CONFIGURAÇÃO DA ENTIDADE PESSOA
            // ============================================
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nome).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Idade).IsRequired();

                // Índice para busca por nome (performance)
                entity.HasIndex(p => p.Nome);
            });

            // ============================================
            // CONFIGURAÇÃO DA ENTIDADE TRANSACAO
            // ============================================
            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Descricao).IsRequired().HasMaxLength(200);
                entity.Property(t => t.Valor).IsRequired().HasPrecision(18, 2);
                entity.Property(t => t.Tipo).IsRequired();
                entity.Property(t => t.DataCriacao).IsRequired();

                // ============================================
                // ⚠️ REGRA DE NEGÓCIO: DELETE EM CASCATA
                // Ao deletar uma pessoa, todas as transações são deletadas
                // ============================================
                entity.HasOne(t => t.Pessoa)
                    .WithMany(p => p.Transacoes)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}