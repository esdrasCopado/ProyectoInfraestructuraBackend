using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SolicitudServidores.Back;
using SolicitudServidores.Back.DTOs;
using SolicitudServidores.Back.Models;
using SolicitudServidores.Back.Services;
using SolicitudServidores.DTOs;
using SolicitudServidores.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using DotNetEnv;
using SolicitudServidores.Utilities;
using SolicitudServidores.Repositories.Interfaces;
using SolicitudServidores.Repositories.Implementaciones;
using SolicitudServidores.Services.Interfaces;
using SolicitudServidores.Services.Implementaciones;
using SolicitudServidores.DBContext;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var envCandidates = new[]
{
    Path.Combine(builder.Environment.ContentRootPath, "variables.env"),
    Path.Combine(Directory.GetCurrentDirectory(), "variables.env"),
    Path.Combine(AppContext.BaseDirectory, "variables.env")
};

var envPath = envCandidates.FirstOrDefault(File.Exists);
if (!string.IsNullOrWhiteSpace(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"Variables de entorno cargadas desde: {envPath}");
}

var databaseProvider = (Environment.GetEnvironmentVariable("DATABASE_PROVIDER")
    ?? builder.Configuration["DatabaseProvider"]
    ?? "sqlite").Trim().ToLowerInvariant();

var postgresConnection = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION")?.Trim()
    ?? builder.Configuration.GetConnectionString("PostgreSQLConnection")?.Trim();

var sqlitePath = Environment.GetEnvironmentVariable("SQLITE_DB_PATH")?.Trim();
if (string.IsNullOrWhiteSpace(sqlitePath))
{
    sqlitePath = Path.Combine(builder.Environment.ContentRootPath, "solicitud_servidores_dev.db");
}
else if (!Path.IsPathRooted(sqlitePath))
{
    sqlitePath = Path.Combine(builder.Environment.ContentRootPath, sqlitePath);
}

var sqliteConnection = $"Data Source={sqlitePath}";
var resetSqliteOnStart = string.Equals(
    Environment.GetEnvironmentVariable("RESET_SQLITE_ON_START"),
    "true",
    StringComparison.OrdinalIgnoreCase);

builder.Configuration["ConnectionStrings:SqliteConnection"] = sqliteConnection;

if (databaseProvider is "postgres" or "postgresql")
{
    if (string.IsNullOrWhiteSpace(postgresConnection))
    {
        throw new Exception("No se encontró una cadena de conexión válida para PostgreSQL.");
    }

    builder.Configuration["ConnectionStrings:PostgreSQLConnection"] = postgresConnection;
}

var jwtKey = Environment.GetEnvironmentVariable("JWT__key")
    ?? Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? builder.Configuration["JWT:key"]
    ?? builder.Configuration["JWT_SECRET"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("No se encontró la llave JWT en variables de entorno o appsettings.");
}

builder.Configuration["JWT:key"] = jwtKey;

var apiDUser = Environment.GetEnvironmentVariable("APID_USER");
var apiDUrl = Environment.GetEnvironmentVariable("APID_URL");
var apiCKey = Environment.GetEnvironmentVariable("APIC_KEY");
var apiCUrl = Environment.GetEnvironmentVariable("APIC_URL");

if (string.IsNullOrWhiteSpace(apiDUser) || string.IsNullOrWhiteSpace(apiDUrl) || string.IsNullOrWhiteSpace(apiCKey) || string.IsNullOrWhiteSpace(apiCUrl))
{
    Console.WriteLine("Advertencia: variables de APIs externas no configuradas; se continuará con valores locales.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//dotnet ef migrations remove -- project AgendaContactosSGD.csproj -- Borra los archivos que mapean la base de datos
//dotnet ef migrations add firstmigration  -- project AgendaContactosSGD.csproj -- Crea archivos para mapear la base de datos
//dotnet ef database update firstmigration --project AgendaContactosSGD.csproj  -- Crea la base de datos

// Agregar servicios al contenedor
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresá el token JWT. Ejemplo: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<DataContext>(options =>
{
    if (databaseProvider is "postgres" or "postgresql")
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection"));
    }
    else
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
    }
});

// Registro de repositorios
//builder.Services.AddScoped<IContactoRepository, ContactoRepository>();
//builder.Services.AddScoped<IEventoRepository, EventoRepository>();
//builder.Services.AddScoped<IObsequioRepository, ObsequioRepository>();
//builder.Services.AddScoped<IAccesoRepository, AccesoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<IServidorRepository, ServidorRepository>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
//builder.Services.AddScoped<IEstadoRepository, EstadoRepository>();
//builder.Services.AddScoped<IAgendaRepository, AgendaRepository>();
//builder.Services.AddScoped<IMunicipioRepository, MunicipioRepository>();
//builder.Services.AddScoped<ILocalidadRepository, LocalidadRepository>();
//builder.Services.AddScoped<ICodigoPostalRepository, CodigoPostalRepository>();
//builder.Services.AddScoped<IHandlerImage, HandlerImage>();
//builder.Services.AddScoped<ICorreoRepository, CorreoRepository>();
//builder.Services.AddScoped<IAlojamientoRepository, AlojamientoRepository>();
//builder.Services.AddScoped<IInvitadoRepository, InvitadoRepository>();


//builder.Services.AddHttpClient<ICorreoRepository, CorreoRepository>(client =>
//{
//    client.BaseAddress = new Uri(apiCUrl);
//    client.DefaultRequestHeaders.Add("ApiKey", apiCKey);
//});

// Configuración de JWT
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:key"]!))
    };
});

// Registro de servicios adicionales
builder.Services.AddSingleton<CrearJWT>();
builder.Services.AddSingleton<Encriptar>();
builder.Services.AddScoped<GlobalExceptionFilter>(); // Registrar el filtro de excepciones

var app = builder.Build();

// Preparar base de datos automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();

    try
    {
        Console.WriteLine($"Preparando la base de datos ({databaseProvider})...");

        if (databaseProvider == "sqlite" && resetSqliteOnStart)
        {
            db.Database.EnsureDeleted();
        }

        db.Database.EnsureCreated();

        if (db.Database.IsNpgsql())
        {
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS public.cartas (
                    id                              BIGINT GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY,
                    folio                           TEXT NOT NULL,
                    creado_en                       TIMESTAMP NOT NULL,
                    ar_sector                       TEXT NOT NULL,
                    ar_dependencia                  TEXT NOT NULL,
                    ar_responsable                  TEXT NOT NULL,
                    ar_cargo                        TEXT NOT NULL,
                    ar_telefono                     TEXT NOT NULL,
                    ar_correo                       TEXT NOT NULL,
                    as_proveedor                    TEXT NOT NULL,
                    as_dependencia                  TEXT NOT NULL,
                    as_responsable                  TEXT NOT NULL,
                    as_cargo                        TEXT NOT NULL,
                    as_telefono                     TEXT NOT NULL,
                    as_correo                       TEXT NOT NULL,
                    desc_descripcion_servidor       TEXT NOT NULL,
                    desc_nombre_servidor            TEXT NOT NULL,
                    desc_nombre_aplicacion          TEXT NOT NULL,
                    desc_tipo_uso                   TEXT NOT NULL,
                    desc_fecha_arranque             TEXT NOT NULL,
                    desc_vigencia                   TEXT NOT NULL,
                    desc_caracteristicas_especiales TEXT,
                    specs_tipo_requerimiento        TEXT NOT NULL,
                    specs_modalidad                 TEXT NOT NULL,
                    specs_sistema_operativo         TEXT NOT NULL,
                    specs_sistema_operativo_otro    TEXT,
                    specs_vcores                    INT NOT NULL,
                    specs_memoria_ram               INT NOT NULL,
                    specs_almacenamiento            INT NOT NULL,
                    specs_motor_bd                  TEXT,
                    specs_puertos                   TEXT,
                    specs_integraciones             TEXT,
                    specs_otras_specs               TEXT,
                    infra_subdominio_solicitado     TEXT,
                    infra_puerto                    TEXT,
                    infra_requiere_ssl              BOOLEAN NOT NULL,
                    infra_vpn_responsable           TEXT NOT NULL,
                    infra_vpn_cargo                 TEXT NOT NULL,
                    infra_vpn_telefono              TEXT NOT NULL,
                    infra_vpn_correo                TEXT NOT NULL,
                    resp_firmante                   TEXT NOT NULL,
                    resp_num_empleado               TEXT NOT NULL,
                    resp_puesto_firmante            TEXT NOT NULL,
                    resp_acepta_terminos            BOOLEAN NOT NULL
                );
                CREATE UNIQUE INDEX IF NOT EXISTS IX_cartas_folio ON public.cartas(folio);

                ALTER TABLE IF EXISTS public.usuarios
                    ADD COLUMN IF NOT EXISTS rol TEXT NOT NULL DEFAULT 'Dependencia / Cliente',
                    ADD COLUMN IF NOT EXISTS puesto TEXT NULL,
                    ADD COLUMN IF NOT EXISTS celular TEXT NULL,
                    ADD COLUMN IF NOT EXISTS numero_puesto TEXT NULL;

                ALTER TABLE IF EXISTS public.solicitud
                    ADD COLUMN IF NOT EXISTS titulo TEXT NOT NULL DEFAULT '',
                    ADD COLUMN IF NOT EXISTS folio TEXT NOT NULL DEFAULT '',
                    ADD COLUMN IF NOT EXISTS etapa_actual TEXT NOT NULL DEFAULT 'Registro',
                    ADD COLUMN IF NOT EXISTS prioridad TEXT NOT NULL DEFAULT 'Media',
                    ADD COLUMN IF NOT EXISTS responsable_actual TEXT NULL,
                    ADD COLUMN IF NOT EXISTS usuario_ultima_actualizacion TEXT NULL,
                    ADD COLUMN IF NOT EXISTS fecha_actualizacion DATE NULL,
                    ADD COLUMN IF NOT EXISTS fecha_requerida DATE NULL,
                    ADD COLUMN IF NOT EXISTS carta_responsiva_folio TEXT NULL,
                    ADD COLUMN IF NOT EXISTS comentarios_seguimiento TEXT NULL,
                    ADD COLUMN IF NOT EXISTS notificacion_nueva BOOLEAN NOT NULL DEFAULT TRUE,
                    ADD COLUMN IF NOT EXISTS tareas_pendientes TEXT NULL;

                ALTER TABLE IF EXISTS public.servidor
                    ADD COLUMN IF NOT EXISTS tipo_uso TEXT NOT NULL DEFAULT 'Interno',
                    ADD COLUMN IF NOT EXISTS requiere_llave_licencia BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS plantilla_recursos TEXT NOT NULL DEFAULT 'General',
                    ADD COLUMN IF NOT EXISTS etapa_operativa TEXT NOT NULL DEFAULT 'Provisionamiento',
                    ADD COLUMN IF NOT EXISTS responsable_infraestructura TEXT NULL,
                    ADD COLUMN IF NOT EXISTS usuario_ultima_actualizacion TEXT NULL,
                    ADD COLUMN IF NOT EXISTS fecha_ultima_actualizacion DATE NULL,
                    ADD COLUMN IF NOT EXISTS fecha_asignacion_ip DATE NULL,
                    ADD COLUMN IF NOT EXISTS tareas_pendientes TEXT NULL,
                    ADD COLUMN IF NOT EXISTS observaciones_seguimiento TEXT NULL,
                    ADD COLUMN IF NOT EXISTS etapa_vulnerabilidades TEXT NULL,
                    ADD COLUMN IF NOT EXISTS requiere_revision_anual BOOLEAN NOT NULL DEFAULT TRUE,
                    ADD COLUMN IF NOT EXISTS ultima_revision_anual DATE NULL,
                    ADD COLUMN IF NOT EXISTS comunicacion_validada BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS fecha_validacion_comunicacion DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_validacion_comunicacion TEXT NULL,
                    ADD COLUMN IF NOT EXISTS parches_aplicados BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS fecha_parches DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_parches TEXT NULL,
                    ADD COLUMN IF NOT EXISTS xdr_instalado BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS fecha_xdr DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_xdr TEXT NULL,
                    ADD COLUMN IF NOT EXISTS credenciales_entregadas BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS fecha_entrega_credenciales DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_credenciales TEXT NULL,
                    ADD COLUMN IF NOT EXISTS waf_configurado BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS fecha_configuracion_waf DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_waf TEXT NULL,
                    ADD COLUMN IF NOT EXISTS evidencia_validada BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS fecha_validacion_evidencia DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_validacion_evidencia TEXT NULL,
                    ADD COLUMN IF NOT EXISTS solicitud_publicacion BOOLEAN NOT NULL DEFAULT FALSE,
                    ADD COLUMN IF NOT EXISTS fecha_publicacion DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_publicacion TEXT NULL,
                    ADD COLUMN IF NOT EXISTS fecha_vulnerabilidades DATE NULL,
                    ADD COLUMN IF NOT EXISTS usuario_vulnerabilidades TEXT NULL;

                ALTER TABLE IF EXISTS public.servidor
                    ALTER COLUMN ""llaveOS"" DROP NOT NULL;

                ALTER TABLE IF EXISTS public.vpn
                    ALTER COLUMN id_usuario DROP NOT NULL;

                ALTER TABLE IF EXISTS public.vpn
                    ADD COLUMN IF NOT EXISTS folio TEXT NOT NULL DEFAULT '';

                ALTER TABLE IF EXISTS public.subdominio
                    ALTER COLUMN id_usuario DROP NOT NULL;

                ALTER TABLE IF EXISTS public.evidencias_pruebas
                    ALTER COLUMN id_usuario DROP NOT NULL,
                    ADD COLUMN IF NOT EXISTS estado_validacion TEXT NOT NULL DEFAULT 'Pendiente',
                    ADD COLUMN IF NOT EXISTS observaciones TEXT NULL;

                ALTER TABLE IF EXISTS public.waf
                    ADD COLUMN IF NOT EXISTS estado TEXT NULL,
                    ADD COLUMN IF NOT EXISTS observaciones TEXT NULL;

                ALTER TABLE IF EXISTS public.cartas
                    ADD COLUMN IF NOT EXISTS id_solicitud BIGINT NULL,
                    ADD COLUMN IF NOT EXISTS solicitud_folio TEXT NULL,
                    ADD COLUMN IF NOT EXISTS specs_arquitectura TEXT NULL,
                    ADD COLUMN IF NOT EXISTS specs_discos_duros TEXT NULL,
                    ADD COLUMN IF NOT EXISTS specs_ip_actual TEXT NULL,
                    ADD COLUMN IF NOT EXISTS specs_nombre_servidor_actual TEXT NULL,
                    ADD COLUMN IF NOT EXISTS specs_tipo_renovacion TEXT NULL,
                    ADD COLUMN IF NOT EXISTS infra_subdominios TEXT NULL,
                    ADD COLUMN IF NOT EXISTS infra_vpns TEXT NULL;

                ALTER TABLE IF EXISTS public.cartas
                    ALTER COLUMN infra_vpn_responsable DROP NOT NULL,
                    ALTER COLUMN infra_vpn_cargo DROP NOT NULL,
                    ALTER COLUMN infra_vpn_telefono DROP NOT NULL,
                    ALTER COLUMN infra_vpn_correo DROP NOT NULL;
            ");
        }

        if (db.Database.IsSqlite())
        {
            EnsureSqliteCompatibilityColumns(db);
        }

        SeedAdminUser(db);
        Console.WriteLine("Base de datos lista para usarse.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al preparar la base de datos ({databaseProvider}): {ex.Message}");
        throw;
    }
}

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}





app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok", provider = databaseProvider, utc = DateTime.UtcNow }));
app.MapControllers();
app.MapFallbackToFile("index.html");

// 🧩 Crear carpetas automáticamente si no existen
string[] imageFolders = new[]
{
    Path.Combine(Directory.GetCurrentDirectory(), "Img", "Sesiones"),
    Path.Combine(Directory.GetCurrentDirectory(), "Img", "Contactos"),
    Path.Combine(Directory.GetCurrentDirectory(), "Img", "Obsequios"),
    Path.Combine(Directory.GetCurrentDirectory(), "Img", "EventosLogo"),
    Path.Combine(Directory.GetCurrentDirectory(), "Img", "Croquis")
};

foreach (var folder in imageFolders)
{
    if (!Directory.Exists(folder))
    {
        Directory.CreateDirectory(folder);
        Console.WriteLine($"✅ Carpeta creada: {folder}");
    }
}

// 🖼 Configuración de archivos estáticos
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Img", "Sesiones")),
    RequestPath = "/imagenes/sesiones",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Img", "Contactos")),
    RequestPath = "/imagenes/contactos",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Img", "Obsequios")),
    RequestPath = "/imagenes/obsequios",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Img", "EventosLogo")),
    RequestPath = "/imagenes/eventosLogo",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Img", "Croquis")),
    RequestPath = "/imagenes/croquis",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});

app.Run();

static void SeedAdminUser(DataContext db)
{
    const string correoAdmin = "admin@local";
    var admin = db.Usuarios
        .Include(u => u.PermisoCategorias)
        .FirstOrDefault(u => u.Correo == correoAdmin);

    if (admin == null)
    {
        admin = new Usuario
        {
            NombreCompleto = "Administrador General",
            Correo = correoAdmin,
            Password = Encriptar.EncriptarSHA256("admin"),
            Rol = "Administrador General",
            Permisos = "Administrador General",
            Puesto = "Administrador de Sistemas",
            Celular = "6440000000",
            NumeroPuesto = "1001"
        };

        db.Usuarios.Add(admin);
        db.SaveChanges();
    }

    admin.Rol = string.IsNullOrWhiteSpace(admin.Rol) || string.Equals(admin.Rol, "Administrador", StringComparison.OrdinalIgnoreCase)
        ? "Administrador General"
        : admin.Rol;
    admin.Permisos = string.IsNullOrWhiteSpace(admin.Permisos) || string.Equals(admin.Permisos, "Administrador", StringComparison.OrdinalIgnoreCase)
        ? admin.Rol
        : admin.Permisos;

    if (!db.PermisoCategorias.Any(p => p.IdUsuario == admin.Id && p.Categoria == "Administrador General"))
    {
        db.PermisoCategorias.Add(new PermisoCategoria
        {
            IdUsuario = admin.Id,
            Categoria = "Administrador General"
        });
    }

    db.SaveChanges();
}

static void EnsureSqliteCompatibilityColumns(DataContext db)
{
    var statements = new[]
    {
        new { Table = "solicitud", Column = "folio", Sql = "ALTER TABLE solicitud ADD COLUMN folio TEXT NOT NULL DEFAULT '';" },
        new { Table = "solicitud", Column = "etapa_actual", Sql = "ALTER TABLE solicitud ADD COLUMN etapa_actual TEXT NOT NULL DEFAULT 'Registro';" },
        new { Table = "solicitud", Column = "prioridad", Sql = "ALTER TABLE solicitud ADD COLUMN prioridad TEXT NOT NULL DEFAULT 'Media';" },
        new { Table = "solicitud", Column = "responsable_actual", Sql = "ALTER TABLE solicitud ADD COLUMN responsable_actual TEXT NULL;" },
        new { Table = "solicitud", Column = "usuario_ultima_actualizacion", Sql = "ALTER TABLE solicitud ADD COLUMN usuario_ultima_actualizacion TEXT NULL;" },
        new { Table = "solicitud", Column = "fecha_actualizacion", Sql = "ALTER TABLE solicitud ADD COLUMN fecha_actualizacion TEXT NULL;" },
        new { Table = "solicitud", Column = "fecha_requerida", Sql = "ALTER TABLE solicitud ADD COLUMN fecha_requerida TEXT NULL;" },
        new { Table = "solicitud", Column = "carta_responsiva_folio", Sql = "ALTER TABLE solicitud ADD COLUMN carta_responsiva_folio TEXT NULL;" },
        new { Table = "solicitud", Column = "comentarios_seguimiento", Sql = "ALTER TABLE solicitud ADD COLUMN comentarios_seguimiento TEXT NULL;" },
        new { Table = "servidor", Column = "etapa_operativa", Sql = "ALTER TABLE servidor ADD COLUMN etapa_operativa TEXT NOT NULL DEFAULT 'Provisionamiento';" },
        new { Table = "servidor", Column = "responsable_infraestructura", Sql = "ALTER TABLE servidor ADD COLUMN responsable_infraestructura TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_ultima_actualizacion", Sql = "ALTER TABLE servidor ADD COLUMN usuario_ultima_actualizacion TEXT NULL;" },
        new { Table = "servidor", Column = "fecha_ultima_actualizacion", Sql = "ALTER TABLE servidor ADD COLUMN fecha_ultima_actualizacion TEXT NULL;" },
        new { Table = "servidor", Column = "fecha_asignacion_ip", Sql = "ALTER TABLE servidor ADD COLUMN fecha_asignacion_ip TEXT NULL;" },
        new { Table = "servidor", Column = "observaciones_seguimiento", Sql = "ALTER TABLE servidor ADD COLUMN observaciones_seguimiento TEXT NULL;" },
        new { Table = "servidor", Column = "comunicacion_validada", Sql = "ALTER TABLE servidor ADD COLUMN comunicacion_validada INTEGER NOT NULL DEFAULT 0;" },
        new { Table = "servidor", Column = "fecha_validacion_comunicacion", Sql = "ALTER TABLE servidor ADD COLUMN fecha_validacion_comunicacion TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_validacion_comunicacion", Sql = "ALTER TABLE servidor ADD COLUMN usuario_validacion_comunicacion TEXT NULL;" },
        new { Table = "servidor", Column = "parches_aplicados", Sql = "ALTER TABLE servidor ADD COLUMN parches_aplicados INTEGER NOT NULL DEFAULT 0;" },
        new { Table = "servidor", Column = "fecha_parches", Sql = "ALTER TABLE servidor ADD COLUMN fecha_parches TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_parches", Sql = "ALTER TABLE servidor ADD COLUMN usuario_parches TEXT NULL;" },
        new { Table = "servidor", Column = "xdr_instalado", Sql = "ALTER TABLE servidor ADD COLUMN xdr_instalado INTEGER NOT NULL DEFAULT 0;" },
        new { Table = "servidor", Column = "fecha_xdr", Sql = "ALTER TABLE servidor ADD COLUMN fecha_xdr TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_xdr", Sql = "ALTER TABLE servidor ADD COLUMN usuario_xdr TEXT NULL;" },
        new { Table = "servidor", Column = "credenciales_entregadas", Sql = "ALTER TABLE servidor ADD COLUMN credenciales_entregadas INTEGER NOT NULL DEFAULT 0;" },
        new { Table = "servidor", Column = "fecha_entrega_credenciales", Sql = "ALTER TABLE servidor ADD COLUMN fecha_entrega_credenciales TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_credenciales", Sql = "ALTER TABLE servidor ADD COLUMN usuario_credenciales TEXT NULL;" },
        new { Table = "servidor", Column = "waf_configurado", Sql = "ALTER TABLE servidor ADD COLUMN waf_configurado INTEGER NOT NULL DEFAULT 0;" },
        new { Table = "servidor", Column = "fecha_configuracion_waf", Sql = "ALTER TABLE servidor ADD COLUMN fecha_configuracion_waf TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_waf", Sql = "ALTER TABLE servidor ADD COLUMN usuario_waf TEXT NULL;" },
        new { Table = "servidor", Column = "evidencia_validada", Sql = "ALTER TABLE servidor ADD COLUMN evidencia_validada INTEGER NOT NULL DEFAULT 0;" },
        new { Table = "servidor", Column = "fecha_validacion_evidencia", Sql = "ALTER TABLE servidor ADD COLUMN fecha_validacion_evidencia TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_validacion_evidencia", Sql = "ALTER TABLE servidor ADD COLUMN usuario_validacion_evidencia TEXT NULL;" },
        new { Table = "servidor", Column = "solicitud_publicacion", Sql = "ALTER TABLE servidor ADD COLUMN solicitud_publicacion INTEGER NOT NULL DEFAULT 0;" },
        new { Table = "servidor", Column = "fecha_publicacion", Sql = "ALTER TABLE servidor ADD COLUMN fecha_publicacion TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_publicacion", Sql = "ALTER TABLE servidor ADD COLUMN usuario_publicacion TEXT NULL;" },
        new { Table = "servidor", Column = "fecha_vulnerabilidades", Sql = "ALTER TABLE servidor ADD COLUMN fecha_vulnerabilidades TEXT NULL;" },
        new { Table = "servidor", Column = "usuario_vulnerabilidades", Sql = "ALTER TABLE servidor ADD COLUMN usuario_vulnerabilidades TEXT NULL;" },
        new { Table = "waf", Column = "estado", Sql = "ALTER TABLE waf ADD COLUMN estado TEXT NULL;" },
        new { Table = "waf", Column = "observaciones", Sql = "ALTER TABLE waf ADD COLUMN observaciones TEXT NULL;" },
        new { Table = "evidencias_pruebas", Column = "estado_validacion", Sql = "ALTER TABLE evidencias_pruebas ADD COLUMN estado_validacion TEXT NOT NULL DEFAULT 'Pendiente';" },
        new { Table = "evidencias_pruebas", Column = "observaciones", Sql = "ALTER TABLE evidencias_pruebas ADD COLUMN observaciones TEXT NULL;" },
        new { Table = "cartas", Column = "id_solicitud", Sql = "ALTER TABLE cartas ADD COLUMN id_solicitud INTEGER NULL;" },
        new { Table = "cartas", Column = "solicitud_folio", Sql = "ALTER TABLE cartas ADD COLUMN solicitud_folio TEXT NULL;" },
        new { Table = "vpn",    Column = "folio",                      Sql = "ALTER TABLE vpn ADD COLUMN folio TEXT NOT NULL DEFAULT '';" },
        new { Table = "cartas", Column = "specs_arquitectura",           Sql = "ALTER TABLE cartas ADD COLUMN specs_arquitectura TEXT NULL;" },
        new { Table = "cartas", Column = "specs_discos_duros",           Sql = "ALTER TABLE cartas ADD COLUMN specs_discos_duros TEXT NULL;" },
        new { Table = "cartas", Column = "specs_ip_actual",              Sql = "ALTER TABLE cartas ADD COLUMN specs_ip_actual TEXT NULL;" },
        new { Table = "cartas", Column = "specs_nombre_servidor_actual", Sql = "ALTER TABLE cartas ADD COLUMN specs_nombre_servidor_actual TEXT NULL;" },
        new { Table = "cartas", Column = "specs_tipo_renovacion",        Sql = "ALTER TABLE cartas ADD COLUMN specs_tipo_renovacion TEXT NULL;" },
        new { Table = "cartas", Column = "infra_subdominios",            Sql = "ALTER TABLE cartas ADD COLUMN infra_subdominios TEXT NULL;" },
        new { Table = "cartas", Column = "infra_vpns",                   Sql = "ALTER TABLE cartas ADD COLUMN infra_vpns TEXT NULL;" }
    };

    foreach (var statement in statements)
    {
        if (!SqliteColumnExists(db, statement.Table, statement.Column))
        {
            db.Database.ExecuteSqlRaw(statement.Sql);
        }
    }
}

static bool SqliteColumnExists(DataContext db, string tableName, string columnName)
{
    var connection = db.Database.GetDbConnection();
    var shouldClose = connection.State != System.Data.ConnectionState.Open;

    if (shouldClose)
    {
        connection.Open();
    }

    try
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName});";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
    finally
    {
        if (shouldClose)
        {
            connection.Close();
        }
    }
}
