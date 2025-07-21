using EcoSenseAPI.Data;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Configuración del DbContext con MySQL
var connectionString = builder.Configuration.GetConnectionString("cadenaMySQL");
builder.Services.AddDbContext<EcoSenseContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Para evitar ciclos en las relaciones
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.PropertyNamingPolicy = null;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Habilitar CORS para que el frontend en Angular pueda acceder
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaLibre", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// AddControllers
builder.Services.AddControllers();

var app = builder.Build();

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware estándar
app.UseHttpsRedirection();
app.UseCors("PoliticaLibre");
app.UseAuthorization();
app.MapControllers();

app.Run();
