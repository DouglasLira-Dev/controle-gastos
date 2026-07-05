using DesafioControleGastos.API.Configurations;
using DesafioControleGastos.API.Middlewares;
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
// 🔒 CONFIGURAÇÃO DE SEGURANÇA
// ============================================

// 🔒 AddJwtAuthentication já inclui AddAuthentication + AddAuthorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// 🔒 Rate Limiting
builder.Services.AddRateLimiting();

// ============================================
// CONFIGURAÇÃO DO SERILOG (LOGGING)
// ============================================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// ============================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ============================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================
// CONFIGURAÇÃO DO REPOSITORY
// ============================================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<IRepository<Usuario>, Repository<Usuario>>();

// ============================================
// CONFIGURAÇÃO DOS SERVICES
// ============================================
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ============================================
// CONFIGURAÇÃO DO AUTOMAPPER
// ============================================
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ============================================
// CONFIGURAÇÃO DOS VALIDATORS
// ============================================
builder.Services.AddValidatorsFromAssemblyContaining<PessoaCreateValidator>();

// ============================================
// 🔒 CONFIGURAÇÃO DO CORS
// ============================================
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) 
    ?? new[] { "http://localhost:5173", "http://localhost:3000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(allowedOrigins)
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
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ============================================
// CONFIGURAÇÃO DO SWAGGER
// ============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================================
// 🔒 PIPELINE DE SEGURANÇA
// ============================================

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}

app.UseRateLimiter();

// ============================================
// PIPELINE DE REQUISIÇÕES
// ============================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

// 🔒 MIDDLEWARES DE AUTENTICAÇÃO E AUTORIZAÇÃO
app.UseAuthentication(); // 🔒 Verifica o token
app.UseAuthorization();  // 🔒 Verifica as permissões

app.MapControllers();

// ============================================
// INICIALIZAÇÃO DO BANCO DE DADOS
// ============================================
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        
        if (!context.Pessoas.Any())
        {
            var pessoa = new Pessoa("João Silva", 30);
            context.Pessoas.Add(pessoa);
            context.SaveChanges();
            
            context.Transacoes.Add(new Transacao("Salário", 5000, TipoTransacao.Receita, pessoa.Id));
            context.Transacoes.Add(new Transacao("Aluguel", 1500, TipoTransacao.Despesa, pessoa.Id));
            context.Transacoes.Add(new Transacao("Supermercado", 800, TipoTransacao.Despesa, pessoa.Id));
            context.SaveChanges();
            
            Log.Information("✅ Dados iniciais criados com sucesso!");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Erro ao inicializar o banco de dados");
    }
}

app.Run();