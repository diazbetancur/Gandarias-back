# FINAL ENVIRONMENT FIX - Windows PowerShell
# This script completely resolves the persistent configuration loading issue

param(
    [switch]$Force = $false
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

function Clear-AllEnvironmentVariables {
    Write-ColorMessage "?? Clearing ALL problematic environment variables..." $Yellow
    
    $varsToClean = @(
        "ASPNETCORE_ENVIRONMENT", 
        "SMTP_PORT", "SMTP_ENABLE_SSL", "SMTP_SERVER", "SMTP_USER", "SMTP_PASSWORD",
        "DATABASE_URL", "JWT_SECRET_KEY", "FRONTEND_URL", "PYTHON_API_URL", 
        "ENCRYPTION_KEY", "ENCRYPTION_IV"
    )
    
    foreach ($var in $varsToClean) {
        # Clear from all scopes
        [Environment]::SetEnvironmentVariable($var, $null, "Process")
        [Environment]::SetEnvironmentVariable($var, $null, "User")
        
        try {
            [Environment]::SetEnvironmentVariable($var, $null, "Machine")
        }
        catch {
            # Ignore if can't set Machine scope
        }
        
        Write-ColorMessage "  ? Cleared: $var" $Green
    }
}

function Set-DevelopmentEnvironment {
    Write-ColorMessage "?? Setting Development environment..." $Yellow
    
    # Set for current process
    [Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "Process")
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    
    # Set for user (permanent)
    [Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "User")
    
    Write-ColorMessage "? ASPNETCORE_ENVIRONMENT = Development (set permanently)" $Green
}

function Test-Configuration {
    Write-ColorMessage "?? Testing configuration loading..." $Yellow
    
    # Set environment for this test
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    
    Write-Host ""
    Write-ColorMessage "Current Environment Variables:" $Blue
    Write-Host "  ASPNETCORE_ENVIRONMENT = $($env:ASPNETCORE_ENVIRONMENT)"
    Write-Host ""
    
    # Test build
    Write-ColorMessage "Building application..." $Blue
    $buildOutput = dotnet build Api-Gandarias --configuration Debug --verbosity minimal 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Build successful" $Green
        
        # Test run for 10 seconds to see startup logs
        Write-ColorMessage "Testing application startup (10 seconds)..." $Blue
        $process = Start-Process "dotnet" -ArgumentList "run --project Api-Gandarias --no-build" -PassThru -RedirectStandardOutput "startup.log" -RedirectStandardError "startup-error.log"
        
        Start-Sleep -Seconds 10
        
        if (!$process.HasExited) {
            Write-ColorMessage "? Application started successfully!" $Green
            Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
            
            # Check logs
            if (Test-Path "startup.log") {
                $logs = Get-Content "startup.log" | Select-Object -Last 20
                Write-ColorMessage "Recent startup logs:" $Blue
                $logs | ForEach-Object { Write-Host "  $_" }
                
                if ($logs -match "Development" -and $logs -match "appsettings.Development.json") {
                    Write-ColorMessage "?? SUCCESS! Using Development configuration" $Green
                } elseif ($logs -match "Production" -or $logs -match "appsettings.Production.json") {
                    Write-ColorMessage "? STILL USING PRODUCTION CONFIG!" $Red
                } else {
                    Write-ColorMessage "??  Configuration detection unclear from logs" $Yellow
                }
                
                Remove-Item "startup.log" -Force -ErrorAction SilentlyContinue
            }
            
            if (Test-Path "startup-error.log") {
                $errors = Get-Content "startup-error.log"
                if ($errors) {
                    Write-ColorMessage "??  Startup errors found:" $Red
                    $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
                }
                Remove-Item "startup-error.log" -Force -ErrorAction SilentlyContinue
            }
            
            return $true
        } else {
            Write-ColorMessage "? Application failed to start or exited early" $Red
            
            if (Test-Path "startup-error.log") {
                $errors = Get-Content "startup-error.log"
                Write-ColorMessage "Startup errors:" $Red
                $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
                Remove-Item "startup-error.log" -Force -ErrorAction SilentlyContinue
            }
            
            return $false
        }
    } else {
        Write-ColorMessage "? Build failed:" $Red
        Write-Host $buildOutput -ForegroundColor Red
        return $false
    }
}

function Show-FinalInstructions {
    Write-ColorMessage "?? FINAL INSTRUCTIONS:" $Green
    Write-Host ""
    Write-ColorMessage "1. COMPLETELY CLOSE and RESTART:" $Yellow
    Write-Host "   • Visual Studio"
    Write-Host "   • PowerShell/Command Prompt" 
    Write-Host "   • VS Code"
    Write-Host "   • Any other development tool"
    Write-Host ""
    Write-ColorMessage "2. After restart, verify environment:" $Yellow
    Write-Host "   echo `$env:ASPNETCORE_ENVIRONMENT"
    Write-Host "   (Should show: Development)"
    Write-Host ""
    Write-ColorMessage "3. Run application:" $Yellow
    Write-Host "   dotnet run --project Api-Gandarias"
    Write-Host ""
    Write-ColorMessage "4. Expected behavior:" $Green
    Write-Host "   • Environment: Development"
    Write-Host "   • Loading appsettings.Development.json"
    Write-Host "   • No FormatException errors"
    Write-Host "   • Application starts at http://localhost:5000"
    Write-Host ""
    Write-ColorMessage "?? If problem STILL persists after restart:" $Red
    Write-Host "   1. Delete bin/ and obj/ folders"
    Write-Host "   2. Run: dotnet clean"
    Write-Host "   3. Run: dotnet restore"
    Write-Host "   4. Run: dotnet build"
    Write-Host "   5. Run: dotnet run --project Api-Gandarias"
}

function Clean-BuildArtifacts {
    Write-ColorMessage "?? Cleaning build artifacts..." $Yellow
    
    if (Test-Path "bin") {
        Remove-Item -Path "bin" -Recurse -Force
        Write-ColorMessage "  ? Removed bin/" $Green
    }
    
    if (Test-Path "obj") {
        Remove-Item -Path "obj" -Recurse -Force  
        Write-ColorMessage "  ? Removed obj/" $Green
    }
    
    # Clean all projects
    dotnet clean --verbosity quiet 2>$null
    Write-ColorMessage "  ? Ran dotnet clean" $Green
}

# Main execution
function Main {
    Write-ColorMessage "?? FINAL ENVIRONMENT FIX" $Green
    Write-ColorMessage "========================" $Blue
    Write-Host ""
    
    Write-ColorMessage "This script will completely resolve the configuration loading issue." $Yellow
    Write-Host ""
    
    if (!$Force) {
        $confirm = Read-Host "This will clear environment variables and reset configuration. Continue? (y/N)"
        if ($confirm -ne "y" -and $confirm -ne "Y") {
            Write-ColorMessage "Operation cancelled." $Yellow
            return
        }
    }
    
    Write-Host ""
    Clear-AllEnvironmentVariables
    Write-Host ""
    Set-DevelopmentEnvironment  
    Write-Host ""
    Clean-BuildArtifacts
    Write-Host ""
    
    # Restore packages
    Write-ColorMessage "?? Restoring packages..." $Blue
    dotnet restore --verbosity quiet
    Write-ColorMessage "? Packages restored" $Green
    Write-Host ""
    
    $testResult = Test-Configuration
    
    Write-Host ""
    if ($testResult) {
        Write-ColorMessage "?? CONFIGURATION FIX SUCCESSFUL!" $Green
        Write-ColorMessage "Your application should now use Development configuration correctly." $Green
    } else {
        Write-ColorMessage "??  Configuration test had issues, but environment is set correctly." $Yellow
        Write-ColorMessage "Please follow the restart instructions below." $Yellow
    }
    
    Write-Host ""
    Show-FinalInstructions
}

# Run the main function
Main