using DesafioControleGastos.Core.Interfaces;
using DesafioControleGastos.Core.Mappings;
using DesafioControleGastos.Core.Validators;
using DesafioControleGastos.Infra.Data;
using DesafioControleGastos.Infra.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DesafioControleGastos.Core.Services;

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
// CONFIGURAÇÃO DO BANCO DE DADOS
// ============================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

    // ============================================
    // CONFIGURAÇÃO DOS SERVICES
    // ============================================
    builder.Services.AddScoped<IPessoaService, PessoaService>();
    builder.Services.AddScoped<ITransacaoService, TransacaoService>();

    // ============================================
    // SWAGGER DESABILITADO TEMPORARIAMENTE
    // ============================================
    // builder.Services.AddEndpointsApiExplorer();
    // builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // ============================================
    // PIPELINE DE REQUISIÇÕES
    // ============================================

    // Swagger desabilitado
    // if (app.Environment.IsDevelopment())
    // {
    //     app.UseSwagger();
    //     app.UseSwaggerUI();
    // }

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
        context.Database.EnsureCreated();
    }

    app.Run();