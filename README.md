EcoSenseAPI

API RESTful desarrollada en ASP.NET Core 7 para monitoreo ambiental inteligente con sensores IoT y gestión de umbrales, usuarios y alertas.

Tecnologías
- ASP.NET Core 7
- Entity Framework Core
- MySQL
- Swagger
- JWT (próximamente)

Estructura
- `Controllers`: Endpoints RESTful
- `Models`: Entidades de dominio (Usuario, Dispositivo, etc.)
- `Data`: DbContext y configuración EF Core
- `DTOs`: Objetos de transferencia de datos

Seguridad
Actualmente sin JWT. Se integrará más adelante.

Configuración
Renombra el archivo `appsettings.template.json` a `appsettings.json` y coloca tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "cadenaMySQL": "server=localhost;database=BD_EcoSense;user=root;password=tu_clave;"
  }
}
