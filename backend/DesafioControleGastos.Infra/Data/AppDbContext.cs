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
        public DbSet<Usuario> Usuarios { get; set; }

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

                entity.HasOne(t => t.Pessoa)
                    .WithMany(p => p.Transacoes)
                    .HasForeignKey(t => t.PessoaId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================================
            // 🔒 CONFIGURAÇÃO DA ENTIDADE USUARIO
            // ============================================
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.PasswordSalt).IsRequired();
                entity.Property(u => u.Role).IsRequired().HasMaxLength(20);
                entity.Property(u => u.IsActive).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.RefreshToken).HasMaxLength(100);
                
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}