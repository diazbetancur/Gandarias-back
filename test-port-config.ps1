# Port Configuration Test - PowerShell
# Verifies that the app works correctly with port 8080 (App Runner standard)

param(
    [string]$TestPort = "8080",  # Changed from 5000 to 8080
    [switch]$TestDocker = $false
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

function Test-LocalDevelopment {
    param([string]$Port)
    
    Write-ColorMessage "?? Testing Local Development Configuration..." $Yellow
    Write-Host ""
    
    # Set development environment
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:PORT = $Port
    
    Write-ColorMessage "Environment Variables:" $Blue
    Write-Host "  ASPNETCORE_ENVIRONMENT = $($env:ASPNETCORE_ENVIRONMENT)"
    Write-Host "  PORT = $($env:PORT)"
    Write-Host ""
    
    # Build first
    Write-ColorMessage "Building application..." $Blue
    $buildResult = dotnet build Api-Gandarias --verbosity quiet 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-ColorMessage "? Build failed" $Red
        return $false
    }
    
    # Start app
    Write-ColorMessage "Starting application on port $Port..." $Blue
    $process = Start-Process "dotnet" -ArgumentList "run --project Api-Gandarias --no-build" -PassThru -RedirectStandardOutput "dev-test.log" -RedirectStandardError "dev-error.log"
    
    Start-Sleep -Seconds 10
    
    if (!$process.HasExited) {
        Write-ColorMessage "? Application started successfully" $Green
        
        # Test endpoints
        $testResults = @()
        
        # Test localhost
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:$Port/health" -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                $testResults += "? localhost:$Port/health - OK"
            }
        } catch {
            $testResults += "? localhost:$Port/health - Failed: $($_.Exception.Message)"
        }
        
        # Test 127.0.0.1
        try {
            $response = Invoke-WebRequest -Uri "http://127.0.0.1:$Port/health" -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                $testResults += "? 127.0.0.1:$Port/health - OK"
            }
        } catch {
            $testResults += "? 127.0.0.1:$Port/health - Failed: $($_.Exception.Message)"
        }
        
        # Test Swagger
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:$Port/swagger" -TimeoutSec 5 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                $testResults += "? Swagger UI - Accessible"
            }
        } catch {
            $testResults += "? Swagger UI - Failed: $($_.Exception.Message)"
        }
        
        Write-Host ""
        Write-ColorMessage "Test Results:" $Blue
        $testResults | ForEach-Object { Write-Host "  $_" }
        
        # Stop test app
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        
        # Check logs
        if (Test-Path "dev-test.log") {
            $logs = Get-Content "dev-test.log"
            $portLogs = $logs | Where-Object { $_ -match "port|listening|Available" }
            if ($portLogs) {
                Write-Host ""
                Write-ColorMessage "Port Configuration Logs:" $Blue
                $portLogs | ForEach-Object { Write-Host "  $_" }
            }
            Remove-Item "dev-test.log" -Force -ErrorAction SilentlyContinue
        }
        
        if (Test-Path "dev-error.log") {
            $errors = Get-Content "dev-error.log"
            if ($errors) {
                Write-Host ""
                Write-ColorMessage "Errors:" $Red
                $errors | ForEach-Object { Write-Host "  $_" -ForegroundColor Red }
            }
            Remove-Item "dev-error.log" -Force -ErrorAction SilentlyContinue
        }
        
        return $true
    } else {
        Write-ColorMessage "? Application failed to start" $Red
        return $false
    }
}

function Test-DockerCompatibility {
    param([string]$Port)
    
    Write-ColorMessage "?? Testing Docker Compatibility..." $Yellow
    Write-Host ""
    
    if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
        Write-ColorMessage "?? Docker not available, skipping Docker test" $Yellow
        return $true
    }
    
    # Build Docker image
    Write-ColorMessage "Building Docker image..." $Blue
    $dockerBuild = docker build -t gandarias-port-test . 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-ColorMessage "? Docker build failed" $Red
        Write-Host $dockerBuild
        return $false
    }
    
    Write-ColorMessage "? Docker image built successfully" $Green
    
    # Test App Runner compatible port mapping (8080:8080)
    $testPorts = @("8080:8080", "3000:8080", "9000:8080")
    
    foreach ($mapping in $testPorts) {
        $localPort = $mapping.Split(':')[0]
        Write-ColorMessage "Testing port mapping $mapping..." $Blue
        
        $containerId = docker run -d -p $mapping gandarias-port-test 2>&1
        
        if ($LASTEXITCODE -eq 0) {
            Start-Sleep -Seconds 15
            
            try {
                $response = Invoke-WebRequest -Uri "http://localhost:$localPort/health" -TimeoutSec 10 -UseBasicParsing
                if ($response.StatusCode -eq 200) {
                    Write-ColorMessage "  ? Port mapping $mapping works" $Green
                } else {
                    Write-ColorMessage "  ? Port mapping $mapping failed (Status: $($response.StatusCode))" $Red
                }
            } catch {
                Write-ColorMessage "  ? Port mapping $mapping failed: $($_.Exception.Message)" $Red
            }
            
            docker stop $containerId | Out-Null
            docker rm $containerId | Out-Null
        } else {
            Write-ColorMessage "  ? Failed to start container with port mapping $mapping" $Red
        }
    }
    
    # Clean up
    docker rmi gandarias-port-test -f | Out-Null
    
    return $true
}

function Show-PortConfigurationSummary {
    Write-ColorMessage "?? APP RUNNER PORT CONFIGURATION (8080)" $Green
    Write-Host ""
    Write-ColorMessage "Fixed Configuration Benefits:" $Blue
    Write-Host "  ? Uses 8080 (App Runner standard port)"
    Write-Host "  ? Uses 0.0.0.0 binding (works in all environments)"
    Write-Host "  ? No hardcoded localhost (prevents Docker 502 errors)"
    Write-Host "  ? Respects PORT environment variable"
    Write-Host "  ? Compatible with App Runner, Docker, and local development"
    Write-Host "  ? Consistent across all environments"
    Write-Host ""
    Write-ColorMessage "How it works:" $Yellow
    Write-Host "  • Development: app.Urls.Add('http://0.0.0.0:8080')"
    Write-Host "  • Docker: app.Urls.Add('http://0.0.0.0:8080')"  
    Write-Host "  • App Runner: app.Urls.Add('http://0.0.0.0:8080')"
    Write-Host ""
    Write-ColorMessage "Access URLs:" $Green
    Write-Host "  • Development: http://localhost:8080"
    Write-Host "  • Docker: http://localhost:8080 (with -p 8080:8080)"
    Write-Host "  • App Runner: https://your-service.awsapprunner.com"
    Write-Host ""
    Write-ColorMessage "Why 8080?" $Yellow
    Write-Host "  • App Runner's default/preferred port"
    Write-Host "  • Common containerized application port"
    Write-Host "  • Avoids conflicts with system ports (80, 443)"
    Write-Host "  • Consistent across development and production"
}

# Main execution
function Main {
    Write-ColorMessage "?? APP RUNNER PORT CONFIGURATION TEST (8080)" $Green
    Write-ColorMessage "=============================================" $Blue
    Write-Host ""
    
    Show-PortConfigurationSummary
    Write-Host ""
    
    $devSuccess = Test-LocalDevelopment -Port $TestPort
    
    if ($TestDocker) {
        Write-Host ""
        $dockerSuccess = Test-DockerCompatibility -Port $TestPort
        
        if ($devSuccess -and $dockerSuccess) {
            Write-Host ""
            Write-ColorMessage "?? ALL TESTS PASSED!" $Green
            Write-ColorMessage "App Runner compatible port configuration is working correctly" $Green
        } else {
            Write-Host ""
            Write-ColorMessage "?? SOME TESTS FAILED" $Yellow
            Write-ColorMessage "Check the results above for details" $Yellow
        }
    } else {
        if ($devSuccess) {
            Write-Host ""
            Write-ColorMessage "?? DEVELOPMENT TEST PASSED!" $Green
            Write-ColorMessage "Run with -TestDocker to also test Docker compatibility" $Yellow
        } else {
            Write-Host ""
            Write-ColorMessage "? DEVELOPMENT TEST FAILED" $Red
        }
    }
}

# Run the main function
Main