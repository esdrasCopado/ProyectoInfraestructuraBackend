$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$backPath = Join-Path $repoRoot 'Back'
$frontPath = Join-Path $repoRoot 'Front'

Write-Host 'Iniciando backend (.NET + SQLite) y frontend (Angular)...' -ForegroundColor Cyan

if (-not (Test-Path (Join-Path $frontPath 'node_modules'))) {
    Write-Host 'Instalando dependencias del frontend...' -ForegroundColor Yellow
    Push-Location $frontPath
    npm install
    Pop-Location
}

Start-Process powershell -ArgumentList '-NoExit', '-Command', "Set-Location '$backPath'; dotnet run --urls 'http://localhost:5055'"
Start-Process powershell -ArgumentList '-NoExit', '-Command', "Set-Location '$frontPath'; npm start"

Write-Host 'Backend:  http://localhost:5055' -ForegroundColor Green
Write-Host 'Frontend: http://localhost:4200' -ForegroundColor Green
Write-Host 'Swagger:  http://localhost:5055/swagger' -ForegroundColor Green