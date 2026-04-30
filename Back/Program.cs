using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SolicitudServidores.DBContext;
using SolicitudServidores.Models;
using SolicitudServidores.Repositories.Implementaciones;
using SolicitudServidores.Repositories.Interfaces;
using SolicitudServidores.Services.Implementaciones;
using SolicitudServidores.Services.Interfaces;
using SolicitudServidores.Utilities;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ── Variables de entorno ──────────────────────────────────────────────────────
var envPath = new[]
{
    Path.Combine(builder.Environment.ContentRootPath, "variables.env"),
    Path.Combine(Directory.GetCurrentDirectory(), "variables.env"),
    Path.Combine(AppContext.BaseDirectory, "variables.env")
}.FirstOrDefault(File.Exists);

if (!string.IsNullOrWhiteSpace(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"Variables de entorno cargadas desde: {envPath}");
}

// ── Conexión PostgreSQL ───────────────────────────────────────────────────────
var postgresConnection = (Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION")
    ?? builder.Configuration.GetConnectionString("PostgreSQLConnection"))?.Trim();

if (string.IsNullOrWhiteSpace(postgresConnection))
    throw new Exception("No se encontró la cadena de conexión PostgreSQL (POSTGRESQL_CONNECTION).");

// ── JWT ───────────────────────────────────────────────────────────────────────
var jwtKey = (Environment.GetEnvironmentVariable("JWT__key")
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["JWT:key"]
    ?? builder.Configuration["JWT_SECRET"])?.Trim();

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("No se encontró la llave JWT en variables de entorno o appsettings.");

builder.Configuration["JWT:key"] = jwtKey;

// ── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// ── Controladores ─────────────────────────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// ── Swagger ───────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "SRIS API",
        Version     = "v1",
        Description = "Sistema de Resguardo de Infraestructura de Servidores.",
        Contact     = new OpenApiContact { Name = "Centro de Datos", Email = "soporte@centrodatos.gob.mx" }
    });

    c.TagActionsBy(api =>
    {
        var controller = api.ActionDescriptor.RouteValues["controller"];
        return controller == "Reporte" ? new[] { "Reportes" } : new[] { controller! };
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Ingresa el token JWT. Ejemplo: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ── Base de datos (PostgreSQL) ────────────────────────────────────────────────
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(postgresConnection));

// ── Repositorios y servicios ──────────────────────────────────────────────────
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<IServidorRepository, ServidorRepository>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<ISeguimientoRepository, SeguimientoRepository>();
builder.Services.AddScoped<ISeguimientoService, SeguimientoService>();
builder.Services.AddScoped<IVpnRepository, VpnRepository>();
builder.Services.AddScoped<IVpnService, VpnService>();
builder.Services.AddScoped<ICartaRepository, CartaRepository>();
builder.Services.AddScoped<ICartaService, CartaService>();

// ── Autenticación JWT ─────────────────────────────────────────────────────────
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer           = false,
        ValidateAudience         = false,
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero,
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// ── Utilidades ────────────────────────────────────────────────────────────────
builder.Services.AddSingleton<CrearJWT>();
builder.Services.AddSingleton<Encriptar>();
builder.Services.AddScoped<GlobalExceptionFilter>();

var app = builder.Build();

// ── Migraciones y seed ────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    try
    {
        Console.WriteLine("Aplicando migraciones PostgreSQL...");
        db.Database.Migrate();
        SeedAdminUser(db);
        Console.WriteLine("Base de datos lista.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
        throw;
    }
}

// ── Pipeline HTTP ─────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(ui =>
    {
        ui.SwaggerEndpoint("/swagger/v1/swagger.json", "SRIS API v1");
        ui.DocumentTitle           = "SRIS — API Docs";
        ui.DefaultModelsExpandDepth(-1);
        ui.EnableFilter();
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok", utc = DateTime.UtcNow }));
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

// ── Seed inicial ──────────────────────────────────────────────────────────────
static void SeedAdminUser(DataContext db)
{
    const string emailAdmin     = "admin@local";
    const string nombreRolAdmin = "admin_centro_datos";

    var rol = db.Roles.FirstOrDefault(r => r.Nombre == nombreRolAdmin);
    if (rol == null)
    {
        rol = new Roles
        {
            Nombre      = nombreRolAdmin,
            Descripcion = "Responsable de validar solicitudes y supervisar el ciclo de aprovisionamiento."
        };
        db.Roles.Add(rol);
        db.SaveChanges();
    }

    if (!db.Usuarios.Any(u => u.Email == emailAdmin))
    {
        db.Usuarios.Add(new Usuario
        {
            Nombre         = "Administrador",
            Apellidos      = "General",
            Email          = emailAdmin,
            PasswordHash   = Encriptar.EncriptarSHA256("admin"),
            RoleId         = rol.RoleId,
            Cargo          = "Administrador de Sistemas",
            Phone          = "6440000000",
            NumeroEmpleado = "1001",
            Activo         = true
        });
        db.SaveChanges();
    }
}
