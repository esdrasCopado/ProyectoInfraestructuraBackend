using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SolicitudServidores.Back;
using SolicitudServidores.Back.DTOs;
using SolicitudServidores.Back.Models;
using SolicitudServidores.Back.Services;
using SolicitudServidores.DTOs;
using SolicitudServidores.Models;

var builder = WebApplication.CreateBuilder(args);

// Cargar JWT_SECRET desde variables.env o appsettings
var jwtSecret = builder.Configuration["JWT_SECRET"] ?? "ReplaceWithSecureSecretKeyChangeInProduction";

// Registrar servicios
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddDbContext<DataContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Data Source=solicitud_servidores.db")
);

// Configurar JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "SolicitudServidores",
            ValidAudience = "SolicitudServidoresAPI",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// Inicializar BD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.EnsureCreated();
}

// ========== ENDPOINTS ==========

// Login
app.MapPost("/api/auth/login", async (LoginRequest req, DataContext db, IAuthService auth) =>
{
    var user = await db.Usuarios.FirstOrDefaultAsync(u => u.Correo == req.Email);
    if (user is null || !auth.VerifyPassword(req.Password ?? "", user.Password ?? ""))
        return Results.Unauthorized();

    var token = auth.GenerateToken(user);
    var response = new LoginResponse
    {
        Token = token,
        User = new UsuarioDTO
        {
            Id = user.Id,
            NombreCompleto = user.NombreCompleto,
            Correo = user.Correo,
            Permisos = user.Permisos
        }
    };

    return Results.Ok(response);
}).WithName("Login");

// Listar solicitudes (autenticado)
app.MapGet("/api/requests", async (DataContext db) =>
{

    var reqs = await db.Servidores.Include(r => r.Solicitud).ToListAsync();
    //var reqs = await db.SolicitudServidores.Include(r => r.Servers).ToListAsync();
    return Results.Ok(reqs);
}).WithName("GetRequests").RequireAuthorization();

// Crear solicitud
app.MapPost("/api/requests", async (CreateServerRequestDTO dto, DataContext db) =>
{
    var req = new Solicitud
    {
        ProjectName = dto.ProjectName,
        RequestedBy = dto.RequestedBy,
        Description = dto.Description,
        Architecture = dto.Architecture,
        RequiredServices = dto.RequiredServices,
        TargetDate = dto.TargetDate,
        Status = "Pendiente",
        CreatedAt = DateTime.UtcNow,
        Servers = dto.Servers?.Select(s => new Servidor
        {
            Hostname = s.Hostname,
            Ip = s.Ip,
            Role = s.Role,
            Os = s.Os,
            Cpu = s.Cpu,
            Ram = s.Ram,
            Disk = s.Disk,
            Purpose = s.Purpose
        }).ToList()
    };

    db.Servidores.Add(req);
    await db.SaveChangesAsync();
    return Results.Created($"/api/requests/{req.Id}", req);
}).WithName("CreateRequest").RequireAuthorization();

// Obtener solicitud por ID
app.MapGet("/api/requests/{id}", async (long id, DataContext db) =>
{
    var req = await db.Servidores.Include(r => r.Solicitud).FirstOrDefaultAsync(r => r.Id == id);
    //var req = await db.SolicitudServidores.Include(r => r.Servers).FirstOrDefaultAsync(r => r.Id == id);
    return req is null ? Results.NotFound() : Results.Ok(req);
}).WithName("GetRequestById").RequireAuthorization();

// Actualizar estado de solicitud (solo revisor/admin)
app.MapPut("/api/requests/{id}/status", async (long id, string status, DataContext db) =>
{
    var req = await db.Servidores.FindAsync(id);
    if (req is null) return Results.NotFound();
    req.Estado = status;
    await db.SaveChangesAsync();
    return Results.Ok(req);
}).WithName("UpdateRequestStatus").RequireAuthorization();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).WithName("Health");

app.Run();
