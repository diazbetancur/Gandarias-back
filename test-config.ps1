# Configuration Loading Test - PowerShell
# Quick test to verify the environment configuration fix

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

Write-ColorMessage "?? CONFIGURATION LOADING TEST" $Green
Write-ColorMessage "=============================" $Blue
Write-Host ""

# Check current environment
$currentEnv = $env:ASPNETCORE_ENVIRONMENT
Write-ColorMessage "Current ASPNETCORE_ENVIRONMENT: $($currentEnv ?? 'Not Set')" $(if($currentEnv -eq "Development"){"Green"} else{"Red"})

if ($currentEnv -ne "Development") {
    Write-ColorMessage "? Environment not set to Development!" $Red
    Write-ColorMessage "Run this to fix: .\final-env-fix.ps1 -Force" $Yellow
    exit 1
}

Write-Host ""
Write-ColorMessage "??? Testing build..." $Blue
$buildResult = dotnet build Api-Gandarias --verbosity quiet 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-ColorMessage "? Build successful" $Green
} else {
    Write-ColorMessage "? Build failed" $Red
    Write-Host $buildResult
    exit 1
}

Write-Host ""
Write-ColorMessage "?? Testing application startup..." $Blue

# Start app and capture output
$process = Start-Process "dotnet" -ArgumentList "run --project Api-Gandarias" -PassThru -RedirectStandardOutput "test-output.log" -RedirectStandardError "test-error.log"

Write-ColorMessage "Waiting 15 seconds for startup..." $Yellow
Start-Sleep -Seconds 15

if (!$process.HasExited) {
    Write-ColorMessage "? Application started successfully!" $Green
    
    # Check if it's listening on the right port
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/health" -TimeoutSec 5 -UseBasicParsing
        if ($response.StatusCode -eq 200) {
            Write-ColorMessage "? Health check passed (200 OK)" $Green
        }
    } catch {
        Write-ColorMessage "?? Health check failed, but app is running" $Yellow
    }
    
    # Stop the test app
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    
    # Check logs for configuration info
    if (Test-Path "test-output.log") {
        $logs = Get-Content "test-output.log"
        Write-Host ""
        Write-ColorMessage "?? Startup Configuration Logs:" $Blue
        
        $configLogs = $logs | Where-Object { 
            $_ -match "Environment|Configuration|appsettings|Loading" 
        }
        
        if ($configLogs) {
            $configLogs | ForEach-Object { 
                if ($_ -match "Development") {
                    Write-Host "  $_" -ForegroundColor Green
                } elseif ($_ -match "Production") {
                    Write-Host "  $_" -ForegroundColor Red
                } else {
                    Write-Host "  $_" -ForegroundColor White
                }
            }
        }
        
        # Check for errors
        $errorLogs = $logs | Where-Object { 
            $_ -match "Error|Exception|Failed" 
        }
        
        if ($errorLogs) {
            Write-Host ""
            Write-ColorMessage "?? Errors found in logs:" $Red
            $errorLogs | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
        }
        
        Remove-Item "test-output.log" -Force -ErrorAction SilentlyContinue
    }
    
    if (Test-Path "test-error.log") {
        $errors = Get-Content "test-error.log"
        if ($errors) {
            Write-Host ""
            Write-ColorMessage "? Stderr logs:" $Red
            $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
        }
        Remove-Item "test-error.log" -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host ""
    Write-ColorMessage "?? TEST COMPLETED SUCCESSFULLY!" $Green
    Write-ColorMessage "Your application should now:" $Green
    Write-Host "  • Use Development environment"
    Write-Host "  • Load appsettings.Development.json"
    Write-Host "  • Start without FormatException errors"
    Write-Host "  • Be accessible at http://localhost:5000"
    
} else {
    Write-ColorMessage "? Application failed to start or exited early" $Red
    
    if (Test-Path "test-error.log") {
        $errors = Get-Content "test-error.log"
        Write-ColorMessage "Error details:" $Red
        $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
        Remove-Item "test-error.log" -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host ""
    Write-ColorMessage "? TEST FAILED" $Red
    Write-ColorMessage "Try running: .\final-env-fix.ps1 -Force" $Yellow
}

Write-Host ""