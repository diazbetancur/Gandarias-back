# SSL/HTTPS Fix Verification Script - Windows PowerShell
# This script tests the fixed configuration for both development and production

param(
    [string]$TestMode = "dev" # "dev" or "prod"
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

function Test-Development {
    Write-ColorMessage "?? Testing DEVELOPMENT Configuration..." $Yellow
    Write-Host ""
    
    Write-ColorMessage "1. Building application..." $Blue
    dotnet build --configuration Debug
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Build successful" $Green
    } else {
        Write-ColorMessage "? Build failed" $Red
        return
    }
    
    Write-ColorMessage "2. Starting application in background..." $Blue
    $process = Start-Process "dotnet" -ArgumentList "run --project Api-Gandarias" -PassThru -NoNewWindow
    
    Write-ColorMessage "3. Waiting for application to start..." $Blue
    Start-Sleep -Seconds 10
    
    Write-ColorMessage "4. Testing endpoints..." $Blue
    
    # Test Health endpoint
    try {
        $health = Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Get -TimeoutSec 5
        Write-ColorMessage "? Health check: Working" $Green
    }
    catch {
        Write-ColorMessage "? Health check: Failed - $($_.Exception.Message)" $Red
    }
    
    # Test Swagger
    try {
        $swagger = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -Method Get -TimeoutSec 5 -UseBasicParsing
        if ($swagger.StatusCode -eq 200) {
            Write-ColorMessage "? Swagger UI: Working" $Green
        } else {
            Write-ColorMessage "? Swagger UI: Failed - Status $($swagger.StatusCode)" $Red
        }
    }
    catch {
        Write-ColorMessage "? Swagger UI: Failed - $($_.Exception.Message)" $Red
    }
    
    Write-ColorMessage "5. Stopping test application..." $Blue
    Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    
    Write-Host ""
    Write-ColorMessage "?? Development Test Results:" $Yellow
    Write-Host "   • HTTP on port 5000: Should be working"
    Write-Host "   • No SSL errors expected"
    Write-Host "   • Swagger accessible at http://localhost:5000/swagger"
}

function Test-ProductionBuild {
    Write-ColorMessage "?? Testing PRODUCTION Build..." $Yellow
    Write-Host ""
    
    Write-ColorMessage "1. Building for production..." $Blue
    dotnet build --configuration Release
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Production build successful" $Green
    } else {
        Write-ColorMessage "? Production build failed" $Red
        return
    }
    
    Write-ColorMessage "2. Building Docker image..." $Blue
    docker build -t gandarias-test .
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Docker image built successfully" $Green
    } else {
        Write-ColorMessage "? Docker build failed" $Red
        return
    }
    
    Write-ColorMessage "3. Testing Docker container..." $Blue
    $containerId = docker run -d -p 8080:80 gandarias-test
    
    Write-ColorMessage "4. Waiting for container to start..." $Blue
    Start-Sleep -Seconds 15
    
    # Test container endpoints
    try {
        $health = Invoke-RestMethod -Uri "http://localhost:8080/health" -Method Get -TimeoutSec 10
        Write-ColorMessage "? Container health check: Working" $Green
    }
    catch {
        Write-ColorMessage "? Container health check: Failed - $($_.Exception.Message)" $Red
    }
    
    try {
        $swagger = Invoke-WebRequest -Uri "http://localhost:8080/swagger" -Method Get -TimeoutSec 10 -UseBasicParsing
        if ($swagger.StatusCode -eq 200) {
            Write-ColorMessage "? Container Swagger: Working" $Green
        } else {
            Write-ColorMessage "? Container Swagger: Failed - Status $($swagger.StatusCode)" $Red
        }
    }
    catch {
        Write-ColorMessage "? Container Swagger: Failed - $($_.Exception.Message)" $Red
    }
    
    Write-ColorMessage "5. Cleaning up..." $Blue
    docker stop $containerId | Out-Null
    docker rm $containerId | Out-Null
    
    Write-Host ""
    Write-ColorMessage "?? Production Test Results:" $Yellow
    Write-Host "   • Docker container on port 8080: Should be working"
    Write-Host "   • Ready for App Runner deployment"
    Write-Host "   • No SSL protocol errors expected"
}

function Show-FixesSummary {
    Write-ColorMessage "?? SSL/HTTPS FIXES APPLIED:" $Green
    Write-Host ""
    Write-Host "? RequireHttpsMetadata = false (allows HTTP for development and App Runner)"
    Write-Host "? Enhanced JWT configuration with timezone tolerance"
    Write-Host "? Proper URL configuration for both environments"
    Write-Host "? Development: HTTP only on localhost:5000"
    Write-Host "? Production: HTTP on 0.0.0.0:80 (App Runner handles HTTPS)"
    Write-Host "? Added launchSettings.json for proper development configuration"
    Write-Host "? Conditional HTTPS redirection (not in App Runner)"
    Write-Host "? Enhanced error handling and debugging"
    Write-Host ""
    Write-ColorMessage "?? How to access after fixes:" $Blue
    Write-Host "   Development: http://localhost:5000"
    Write-Host "   Development Swagger: http://localhost:5000/swagger"
    Write-Host "   Production: https://your-app-runner-url.awsapprunner.com"
    Write-Host ""
    Write-ColorMessage "?? To run in development:" $Yellow
    Write-Host "   dotnet run --project Api-Gandarias"
    Write-Host "   OR"
    Write-Host "   F5 in Visual Studio"
}

# Main execution
function Main {
    Write-ColorMessage "?? SSL/HTTPS Configuration Fix Verification" $Green
    Write-ColorMessage "=========================================" $Blue
    Write-Host ""
    
    Show-FixesSummary
    
    if ($TestMode -eq "dev") {
        Test-Development
    } elseif ($TestMode -eq "prod") {
        Test-ProductionBuild
    } else {
        Write-ColorMessage "Usage: .\ssl-fix-test.ps1 -TestMode dev|prod" $Yellow
    }
}

# Run the main function
Main