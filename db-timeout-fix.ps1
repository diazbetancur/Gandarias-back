# Database Timeout Diagnostic Script - PowerShell
# This script helps diagnose and fix PostgreSQL timeout issues

param(
    [switch]$TestConnection = $false,
    [switch]$Fix = $false
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

function Test-DatabaseConnection {
    Write-ColorMessage "?? Testing database connection and performance..." $Yellow
    Write-Host ""
    
    # Set development environment
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    
    # Build first
    Write-ColorMessage "Building application..." $Blue
    $buildResult = dotnet build Api-Gandarias --verbosity quiet 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-ColorMessage "? Build failed" $Red
        return $false
    }
    
    # Start app and test DB connection
    Write-ColorMessage "Starting application to test database..." $Blue
    $process = Start-Process "dotnet" -ArgumentList "run --project Api-Gandarias --no-build" -PassThru -RedirectStandardOutput "db-test.log" -RedirectStandardError "db-error.log"
    
    Start-Sleep -Seconds 30  # Give more time for DB connection
    
    if (!$process.HasExited) {
        Write-ColorMessage "? Application started - checking logs..." $Green
        
        # Stop the app
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        
        # Analyze logs
        if (Test-Path "db-test.log") {
            $logs = Get-Content "db-test.log"
            
            # Check for database-related messages
            $dbLogs = $logs | Where-Object { 
                $_ -match "database|PostgreSQL|connection|timeout|seeding|Configuration|Environment" 
            }
            
            Write-Host ""
            Write-ColorMessage "Database Configuration Logs:" $Blue
            if ($dbLogs) {
                $dbLogs | ForEach-Object { 
                    if ($_ -match "?|SUCCESS|completed") {
                        Write-Host "  $_" -ForegroundColor Green
                    } elseif ($_ -match "??|WARNING|timeout") {
                        Write-Host "  $_" -ForegroundColor Yellow
                    } elseif ($_ -match "?|ERROR|failed") {
                        Write-Host "  $_" -ForegroundColor Red
                    } else {
                        Write-Host "  $_" -ForegroundColor White
                    }
                }
            } else {
                Write-ColorMessage "  No database-related logs found" $Yellow
            }
            
            Remove-Item "db-test.log" -Force -ErrorAction SilentlyContinue
        }
        
        # Check for errors
        if (Test-Path "db-error.log") {
            $errors = Get-Content "db-error.log"
            if ($errors) {
                Write-Host ""
                Write-ColorMessage "Database Errors:" $Red
                $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
                
                # Check for specific timeout errors
                $timeoutErrors = $errors | Where-Object { $_ -match "TimeoutException|timeout" }
                if ($timeoutErrors) {
                    Write-Host ""
                    Write-ColorMessage "?? TIMEOUT ISSUES DETECTED!" $Red
                    Write-ColorMessage "The fixes applied should help resolve these issues." $Yellow
                }
            }
            Remove-Item "db-error.log" -Force -ErrorAction SilentlyContinue
        }
        
        return $true
    } else {
        Write-ColorMessage "? Application failed to start or exited early" $Red
        
        if (Test-Path "db-error.log") {
            $errors = Get-Content "db-error.log"
            Write-ColorMessage "Startup errors:" $Red
            $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
            Remove-Item "db-error.log" -Force -ErrorAction SilentlyContinue
        }
        
        return $false
    }
}

function Show-TimeoutFixes {
    Write-ColorMessage "?? DATABASE TIMEOUT FIXES APPLIED" $Green
    Write-Host ""
    Write-ColorMessage "Connection String Improvements:" $Blue
    Write-Host "  ? Command Timeout: 300 seconds (was 120)"
    Write-Host "  ? Connection Timeout: 60 seconds"
    Write-Host "  ? Connection Idle Lifetime: 300 seconds"
    Write-Host "  ? Keepalive: 30 seconds"
    Write-Host "  ? Max Pool Size: 100"
    Write-Host "  ? Connection Pooling: Enabled"
    Write-Host ""
    Write-ColorMessage "EF Core Optimizations:" $Blue
    Write-Host "  ? Query Splitting: Enabled"
    Write-Host "  ? Service Provider Caching: Enabled"
    Write-Host "  ? Enhanced Retry Policy: 5 attempts, 30s delay"
    Write-Host "  ? Extended Command Timeout: 300 seconds"
    Write-Host ""
    Write-ColorMessage "Seeding Improvements:" $Blue
    Write-Host "  ? Seed Timeout: 10 minutes (was 3)"
    Write-Host "  ? Better error handling and logging"
    Write-Host "  ? Graceful timeout handling"
    Write-Host ""
    Write-ColorMessage "Why These Fixes Help:" $Yellow
    Write-Host "  • Longer timeouts for complex operations"
    Write-Host "  • Connection pooling reduces overhead"
    Write-Host "  • Retry policy handles transient failures"
    Write-Host "  • Query splitting improves performance"
    Write-Host "  • Keepalive prevents connection drops"
}

function Show-TroubleshootingTips {
    Write-ColorMessage "???  TROUBLESHOOTING TIPS" $Yellow
    Write-Host ""
    Write-ColorMessage "If timeouts still occur:" $Blue
    Write-Host "1. Check Azure PostgreSQL performance metrics"
    Write-Host "2. Consider upgrading PostgreSQL tier (more vCores/memory)"
    Write-Host "3. Check for long-running queries in PostgreSQL logs"
    Write-Host "4. Monitor connection count vs. max connections"
    Write-Host "5. Consider adding database indexes for slow queries"
    Write-Host ""
    Write-ColorMessage "Azure PostgreSQL Optimization:" $Blue
    Write-Host "• Enable connection pooling (PgBouncer)"
    Write-Host "• Increase max_connections if needed"
    Write-Host "• Monitor CPU and memory usage"
    Write-Host "• Check slow query log"
    Write-Host ""
    Write-ColorMessage "Development Environment:" $Blue
    Write-Host "• Use ENABLE_SEED_DATA=true only when needed"
    Write-Host "• Consider seeding data separately in production"
    Write-Host "• Monitor EF Core query performance"
}

function Main {
    Write-ColorMessage "???  DATABASE TIMEOUT DIAGNOSTIC" $Green
    Write-ColorMessage "===============================" $Blue
    Write-Host ""
    
    Show-TimeoutFixes
    Write-Host ""
    
    if ($TestConnection) {
        $testResult = Test-DatabaseConnection
        
        Write-Host ""
        if ($testResult) {
            Write-ColorMessage "?? DATABASE CONNECTION TEST COMPLETED" $Green
            Write-ColorMessage "Check the logs above for any remaining issues" $Yellow
        } else {
            Write-ColorMessage "? DATABASE CONNECTION TEST FAILED" $Red
            Write-ColorMessage "Review the error messages above" $Yellow
        }
        Write-Host ""
    }
    
    Show-TroubleshootingTips
    
    Write-Host ""
    Write-ColorMessage "?? SUMMARY OF IMPROVEMENTS:" $Green
    Write-Host "  • Connection timeouts increased significantly"
    Write-Host "  • Better connection pooling configuration"
    Write-Host "  • Enhanced retry policies for Azure PostgreSQL"
    Write-Host "  • Improved error handling and logging"
    Write-Host "  • Query performance optimizations"
    Write-Host ""
    Write-ColorMessage "Run with -TestConnection to test the fixes" $Yellow
}

# Run the main function
Main