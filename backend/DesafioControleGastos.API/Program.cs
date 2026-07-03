using DesafioControleGastos.Core.Interfaces;
using DesafioControleGastos.Core.Mappings;
using DesafioControleGastos.Core.Models; 
using DesafioControleGastos.Core.Services;
using DesafioControleGastos.Core.Validators;
using DesafioControleGastos.Infra.Data;
using DesafioControleGastos.Infra.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CONFIGURAÇÃO DO SERILOG (LOGGING)
// ============================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================
// CONFIGURAÇÃO DO BANCO DE DADOS (SQLite)
// ============================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================
// CONFIGURAÇÃO DO REPOSITORY
// ============================================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();

// ============================================
// CONFIGURAÇÃO DOS SERVICES
// ============================================
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();

// ============================================
// CONFIGURAÇÃO DO AUTOMAPPER
// ============================================
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ============================================
// CONFIGURAÇÃO DOS VALIDATORS
// ============================================
builder.Services.AddValidatorsFromAssemblyContaining<PessoaCreateValidator>();

// ============================================
// CONFIGURAÇÃO DO CORS (SEGURANÇA)
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

// ============================================
// CONFIGURAÇÃO DOS CONTROLLERS
// ============================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

// ============================================
// PIPELINE DE REQUISIÇÕES
// ============================================

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

// ============================================
// INICIALIZAÇÃO DO BANCO DE DADOS
// ============================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();
    
    // Seed de dados para teste
    if (!context.Pessoas.Any())
    {
        var pessoa = new Pessoa("João Silva", 30); 
        context.Pessoas.Add(pessoa);
        await context.SaveChangesAsync();
        
        context.Transacoes.Add(new Transacao("Salário", 5000, TipoTransacao.Receita, pessoa.Id));
        context.Transacoes.Add(new Transacao("Aluguel", 1500, TipoTransacao.Despesa, pessoa.Id));
        context.Transacoes.Add(new Transacao("Supermercado", 800, TipoTransacao.Despesa, pessoa.Id));
        await context.SaveChangesAsync();
        
        Log.Information("✅ Dados iniciais criados com sucesso!");
    }
}

app.Run();