# Playwright Browser Installation Guide

## ✅ E2E Test Infrastructure Complete

The Playwright E2E test infrastructure is fully set up with:
- `PlaywrightFixture` - Browser management
- `LoginFlowTests` - User login and dashboard tests
- `OrganizationWorkflowTests` - Navigation and workflow tests

## Install Playwright Browsers

Before running E2E tests, you must install Playwright browsers:

### Option 1: Using Playwright CLI (Recommended)
```powershell
cd d:\test\aspnet-core\test\DoganConsult.Web.Blazor.Tests
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install
```

### Option 2: Using Global Playwright Installation
```powershell
# Install Playwright globally
dotnet tool install -g Microsoft.Playwright.CLI

# Install browsers
playwright install chromium
```

### Option 3: Manual Installation
```powershell
# Navigate to test project
cd d:\test\aspnet-core\test\DoganConsult.Web.Blazor.Tests

# Build the project first
dotnet build

# Run the Playwright install script
pwsh bin/Debug/net10.0/playwright.ps1 install --with-deps
```

## Run E2E Tests

After installing browsers:

```powershell
cd d:\test\aspnet-core
dotnet test test/DoganConsult.Web.Blazor.Tests/DoganConsult.Web.Blazor.Tests.csproj --filter "FullyQualifiedName~E2E"
```

## Run Component Tests Only (No Browser Required)

```powershell
cd d:\test\aspnet-core
dotnet test test/DoganConsult.Web.Blazor.Tests/DoganConsult.Web.Blazor.Tests.csproj --filter "FullyQualifiedName!~E2E"
```

## Troubleshooting

If you encounter "Browser not found" errors:
1. Ensure the project is built: `dotnet build`
2. Run the install script from the bin directory
3. Check that browsers are installed in: `%USERPROFILE%\.cache\ms-playwright\`

## Note

E2E tests require:
- The application to be running (services started)
- Playwright browsers installed
- Network access to `https://localhost:44373`

Component tests can run independently without browsers or running services.
