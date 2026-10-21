using DigitalBank.Api.Middlewares;
using DigitalBank.Application;
using DigitalBank.Infrastructure;
using DigitalBank.Infrastructure.Persistence;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Inyección de dependencias de las capas de la arquitectura
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controladores y manejo global de excepciones
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Documentación OpenAPI y Scalar
builder.Services.AddOpenApi();

var app = builder.Build();

// Inicialización automática de la base de datos SQLite si no existe
using (var scope = app.Services.CreateScope())
{
    var repositoryContext = scope.ServiceProvider.GetRequiredService<RepositoryContext>();
    repositoryContext.Database.EnsureCreated();
}

// Configuración del pipeline HTTP
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
