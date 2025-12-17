# PowerShell script to run database migrations for all services

Write-Host "=== Running Database Migrations ===" -ForegroundColor Green
Write-Host ""

$services = @(
    "Identity",
    "Organization",
    "Workspace",
    "UserProfile",
    "Audit",
    "Document",
    "AI"
)

foreach ($service in $services) {
    Write-Host "Running migrations for $service service..." -ForegroundColor Yellow
    $migratorPath = "src\DoganConsult.$service.DbMigrator"
    
    if (Test-Path $migratorPath) {
        Push-Location $migratorPath
        try {
            dotnet run
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✓ $service migrations completed" -ForegroundColor Green
            } else {
                Write-Host "✗ $service migrations failed" -ForegroundColor Red
                Pop-Location
                exit 1
            }
        } catch {
            Write-Host "✗ $service migrations failed: $_" -ForegroundColor Red
            Pop-Location
            exit 1
        } finally {
            Pop-Location
        }
    } else {
        Write-Host "✗ Migrator not found for $service" -ForegroundColor Red
        exit 1
    }
    Write-Host ""
}

Write-Host "=== All Migrations Complete ===" -ForegroundColor Green
