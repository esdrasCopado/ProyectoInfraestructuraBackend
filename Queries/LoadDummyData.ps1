# Script PowerShell para popular la base de datos con dummy data
# Uso: .\LoadDummyData.ps1 -ServerName "localhost" -Port 5432 -Database "nombre_db" -User "postgres" -Password "password"

param(
    [string]$ServerName = "localhost",
    [int]$Port = 5432,
    [string]$Database = "agendabd",
    [string]$User = "postgres",
    [string]$Password = "root",
    [string]$SqlFile = "DummyData_Clean.sql"
)

# Validar que psql está disponible
$psqlPath = Get-Command psql -ErrorAction SilentlyContinue
if (-not $psqlPath) {
    Write-Host "❌ Error: psql no está disponible en el PATH" -ForegroundColor Red
    Write-Host "Por favor, instala PostgreSQL o agrega su ruta al PATH" -ForegroundColor Yellow
    exit 1
}

Write-Host "🔄 Cargando datos de prueba en la base de datos..." -ForegroundColor Cyan
Write-Host "Servidor: $ServerName`:$Port" -ForegroundColor White
Write-Host "Base de datos: $Database" -ForegroundColor White
Write-Host "Usuario: $User" -ForegroundColor White

# Construir la conexión
$env:PGPASSWORD = $Password

# Ejecutar el script SQL
try {
    # Se usa $ServerName en lugar de $Host
    psql -h $ServerName -p $Port -U $User -d $Database -f $SqlFile
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Datos cargados exitosamente!" -ForegroundColor Green
        Write-Host "La base de datos está lista para pruebas." -ForegroundColor Green
    } else {
        Write-Host "❌ Error al ejecutar el script SQL" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    exit 1
}
finally {
    # Limpiar variable de contraseña
    Remove-Item env:PGPASSWORD -ErrorAction SilentlyContinue
}