using EcoSenseAPI.Data;
using EcoSenseAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
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

// Configuración JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EcoSense-SuperSecretKey-32Characters!!")),
            ValidateIssuer = true,
            ValidIssuer = "EcoSenseAPI",
            ValidateAudience = true,
            ValidAudience = "EcoSenseFront",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Registro del servicio MQTT
builder.Services.AddSingleton<MqttLecturaService>();

var app = builder.Build();

// Inicializa el servicio MQTT al arrancar
var mqttService = app.Services.GetRequiredService<MqttLecturaService>();
await mqttService.InitAsync();


// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware estándar
app.UseHttpsRedirection();
app.UseStaticFiles(); // Para servir archivos desde wwwroot
app.UseCors("PoliticaLibre");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();