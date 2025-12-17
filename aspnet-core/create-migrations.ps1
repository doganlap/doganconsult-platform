# PowerShell script to create initial migrations for all services

Write-Host "=== Creating Initial Migrations ===" -ForegroundColor Green
Write-Host ""

$services = @(
    @{Name="Identity"; EfCore="DoganConsult.Identity.EntityFrameworkCore"; Migrator="DoganConsult.Identity.DbMigrator"},
    @{Name="Organization"; EfCore="DoganConsult.Organization.EntityFrameworkCore"; Migrator="DoganConsult.Organization.DbMigrator"},
    @{Name="Workspace"; EfCore="DoganConsult.Workspace.EntityFrameworkCore"; Migrator="DoganConsult.Workspace.DbMigrator"},
    @{Name="UserProfile"; EfCore="DoganConsult.UserProfile.EntityFrameworkCore"; Migrator="DoganConsult.UserProfile.DbMigrator"},
    @{Name="Audit"; EfCore="DoganConsult.Audit.EntityFrameworkCore"; Migrator="DoganConsult.Audit.DbMigrator"},
    @{Name="Document"; EfCore="DoganConsult.Document.EntityFrameworkCore"; Migrator="DoganConsult.Document.DbMigrator"},
    @{Name="AI"; EfCore="DoganConsult.AI.EntityFrameworkCore"; Migrator="DoganConsult.AI.DbMigrator"}
)

foreach ($service in $services) {
    Write-Host "Creating migration for $($service.Name) service..." -ForegroundColor Yellow
    $efCorePath = "src\$($service.EfCore)"
    $migratorPath = "src\$($service.Migrator)"
    
    if (Test-Path $efCorePath) {
        Push-Location $efCorePath
        try {
            # Build the startup project first to ensure all dependencies are resolved
            dotnet build "..\$($service.Migrator)\$($service.Migrator).csproj" --no-restore | Out-Null
            if ($LASTEXITCODE -ne 0) {
                Write-Host "✗ $($service.Name) build failed" -ForegroundColor Red
                Pop-Location
                continue
            }
            
            # Check if Initial migration already exists
            $migrationsPath = Join-Path $efCorePath "Migrations"
            $initialMigrationExists = $false
            if (Test-Path $migrationsPath) {
                $initialMigrationExists = Get-ChildItem -Path $migrationsPath -Filter "*_Initial.cs" -ErrorAction SilentlyContinue | Measure-Object | Select-Object -ExpandProperty Count
                $initialMigrationExists = $initialMigrationExists -gt 0
            }
            
            if ($initialMigrationExists) {
                Write-Host "⚠ $($service.Name) Initial migration already exists, skipping" -ForegroundColor Yellow
            } else {
                dotnet ef migrations add Initial --startup-project "..\$($service.Migrator)\$($service.Migrator).csproj"
                if ($LASTEXITCODE -eq 0) {
                    Write-Host "✓ $($service.Name) migration created" -ForegroundColor Green
                } else {
                    Write-Host "✗ $($service.Name) migration creation failed" -ForegroundColor Red
                }
            }
        } catch {
            Write-Host "✗ $($service.Name) migration creation failed: $_" -ForegroundColor Red
        } finally {
            Pop-Location
        }
    } else {
        Write-Host "✗ EF Core project not found for $($service.Name)" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "=== Migration Creation Complete ===" -ForegroundColor Green
Write-Host "Now run: .\run-migrations.ps1" -ForegroundColor Cyan
