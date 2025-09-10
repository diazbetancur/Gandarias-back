# Environment Setup and Testing Script - Windows PowerShell
# This script properly configures the development environment and tests configuration loading

param(
    [switch]$Fix = $false,
    [switch]$Test = $false
)

# Colors for PowerShell
$Green = "Green"
$Yellow = "Yellow"
$Red = "Red"
$Blue = "Cyan"

function Write-ColorMessage {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

function Check-EnvironmentConfiguration {
    Write-ColorMessage "?? Checking environment configuration..." $Yellow
    Write-Host ""
    
    # Check ASPNETCORE_ENVIRONMENT in different scopes
    $userEnv = [Environment]::GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "User")
    $machineEnv = [Environment]::GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Machine") 
    $processEnv = [Environment]::GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Process")
    
    Write-ColorMessage "Environment Variable Status:" $Blue
    Write-Host "  User Scope: $($userEnv ?? 'Not Set')" -ForegroundColor $(if($userEnv -eq "Development"){"Green"} elseif($userEnv){"Red"} else{"Yellow"})
    Write-Host "  Machine Scope: $($machineEnv ?? 'Not Set')" -ForegroundColor $(if($machineEnv -eq "Development"){"Green"} elseif($machineEnv){"Red"} else{"Yellow"})
    Write-Host "  Process Scope: $($processEnv ?? 'Not Set')" -ForegroundColor $(if($processEnv -eq "Development"){"Green"} elseif($processEnv){"Red"} else{"Yellow"})
    Write-Host ""
    
    # Check what .NET Core will actually use
    $effectiveEnv = $processEnv ?? $userEnv ?? $machineEnv ?? "Production"
    Write-ColorMessage "Effective Environment: $effectiveEnv" $(if($effectiveEnv -eq "Development"){"Green"} else{"Red"})
    
    return $effectiveEnv
}

function Check-ConfigurationFiles {
    Write-ColorMessage "?? Checking configuration files..." $Yellow
    Write-Host ""
    
    $files = @(
        "Api-Gandarias\appsettings.json",
        "Api-Gandarias\appsettings.Development.json", 
        "Api-Gandarias\appsettings.Production.json"
    )
    
    foreach ($file in $files) {
        if (Test-Path $file) {
            Write-ColorMessage "? Found: $file" $Green
            
            # Check for template variables in Production file
            if ($file.EndsWith("Production.json")) {
                $content = Get-Content $file -Raw
                if ($content -match '\$\{.*\}') {
                    Write-ColorMessage "??  Contains template variables (${...})" $Yellow
                }
            }
        } else {
            Write-ColorMessage "? Missing: $file" $Red
        }
    }
}

function Fix-EnvironmentConfiguration {
    Write-ColorMessage "?? Fixing environment configuration..." $Yellow
    Write-Host ""
    
    # Set ASPNETCORE_ENVIRONMENT for current process
    [Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "Process")
    Write-ColorMessage "? Set ASPNETCORE_ENVIRONMENT=Development for current process" $Green
    
    # Set for user scope (permanent)
    [Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "User")
    Write-ColorMessage "? Set ASPNETCORE_ENVIRONMENT=Development for user scope (permanent)" $Green
    
    # Clean up any problematic production environment variables
    $productionVars = @("DATABASE_URL", "JWT_SECRET_KEY", "SMTP_SERVER", "SMTP_USER", "SMTP_PASSWORD", "FRONTEND_URL", "PYTHON_API_URL", "ENCRYPTION_KEY", "ENCRYPTION_IV")
    
    foreach ($var in $productionVars) {
        $value = [Environment]::GetEnvironmentVariable($var, "User")
        if ($value -and $value.Contains('${')) {
            [Environment]::SetEnvironmentVariable($var, $null, "User")
            Write-ColorMessage "?? Cleaned up problematic variable: $var" $Yellow
        }
    }
    
    Write-Host ""
    Write-ColorMessage "?? Environment configuration fixed!" $Green
    Write-ColorMessage "Please restart your IDE/terminal for changes to take effect" $Yellow
}

function Test-ApplicationConfiguration {
    Write-ColorMessage "?? Testing application configuration..." $Yellow
    Write-Host ""
    
    # Temporarily set environment for this test
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    
    try {
        Write-ColorMessage "Building application..." $Blue
        $buildOutput = dotnet build Api-Gandarias --configuration Debug --verbosity quiet 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Write-ColorMessage "? Build successful" $Green
            
            Write-ColorMessage "Testing configuration loading..." $Blue
            
            # Create a simple test to check which config is being loaded
            $testScript = @"
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(`$"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine(`$"ContentRoot: {builder.Environment.ContentRootPath}");

// Check which configuration files are loaded
var config = builder.Configuration as IConfigurationRoot;
if (config != null) {
    Console.WriteLine("Configuration sources:");
    foreach (var source in config.Providers) {
        Console.WriteLine(`$"  - {source.GetType().Name}");
    }
}

// Test a specific configuration value
var connectionString = builder.Configuration.GetConnectionString("PgSQL");
if (connectionString?.Contains("gandarias-db.postgres.database.azure.com") == true) {
    Console.WriteLine("? Using Development configuration (Azure PostgreSQL)");
} else if (connectionString?.Contains("`${DATABASE_URL}") == true) {
    Console.WriteLine("? Using Production configuration (template variables)");
} else {
    Console.WriteLine(`$"??  Unexpected connection string: {connectionString}");
}

var app = builder.Build();
return 0;
"@
            
            $testFile = "ConfigTest.cs"
            $testScript | Out-File -FilePath $testFile -Encoding UTF8
            
            $testOutput = dotnet script $testFile 2>&1
            Write-Host $testOutput
            
            Remove-Item $testFile -Force -ErrorAction SilentlyContinue
            
        } else {
            Write-ColorMessage "? Build failed: $buildOutput" $Red
        }
    }
    catch {
        Write-ColorMessage "? Test failed: $($_.Exception.Message)" $Red
    }
}

function Show-RecommendedActions {
    Write-ColorMessage "?? RECOMMENDED ACTIONS:" $Blue
    Write-Host ""
    Write-ColorMessage "1. Fix environment configuration:" $Green
    Write-Host "   .\env-setup.ps1 -Fix"
    Write-Host ""
    Write-ColorMessage "2. Restart your IDE/terminal completely" $Green
    Write-Host ""
    Write-ColorMessage "3. Verify environment:" $Green
    Write-Host "   .\env-setup.ps1 -Test"
    Write-Host ""
    Write-ColorMessage "4. Run application:" $Green
    Write-Host "   dotnet run --project Api-Gandarias"
    Write-Host ""
    Write-ColorMessage "Expected behavior after fix:" $Yellow
    Write-Host "   • Environment: Development"
    Write-Host "   • Uses appsettings.Development.json"
    Write-Host "   • No template variable errors"
    Write-Host "   • Connects to Azure PostgreSQL directly"
    Write-Host ""
    Write-ColorMessage "?? Why this happens:" $Red
    Write-Host "   • ASPNETCORE_ENVIRONMENT not set to 'Development'"
    Write-Host "   • Application defaults to 'Production'"
    Write-Host "   • Production config has template variables (${...})"
    Write-Host "   • These cause FormatException when parsed"
}

# Main execution
function Main {
    Write-ColorMessage "?? Environment Configuration Diagnostic" $Green
    Write-ColorMessage "======================================" $Blue
    Write-Host ""
    
    $currentEnv = Check-EnvironmentConfiguration
    Check-ConfigurationFiles
    
    Write-Host ""
    
    if ($currentEnv -ne "Development") {
        Write-ColorMessage "?? PROBLEM DETECTED!" $Red
        Write-ColorMessage "Your application is running in '$currentEnv' environment instead of 'Development'" $Red
        Write-ColorMessage "This causes it to load appsettings.Production.json with template variables" $Red
        Write-Host ""
        
        if ($Fix) {
            Fix-EnvironmentConfiguration
        } else {
            Write-ColorMessage "Run with -Fix to automatically configure Development environment:" $Yellow
            Write-ColorMessage ".\env-setup.ps1 -Fix" $Yellow
        }
    } else {
        Write-ColorMessage "? Environment correctly set to Development" $Green
        
        if ($Test) {
            Write-Host ""
            Test-ApplicationConfiguration
        }
    }
    
    Write-Host ""
    Show-RecommendedActions
}

# Run the main function
Main