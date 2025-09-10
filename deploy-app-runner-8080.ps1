# AWS App Runner Deployment with Port 8080 - Windows PowerShell
# This script deploys the application with correct port configuration for App Runner

param(
    [string]$Region = "eu-central-1",
    [string]$ProjectName = "back-gandarias-api",
    [string]$AccountId = "752862029990",
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

function Test-Prerequisites {
    Write-ColorMessage "?? Checking prerequisites..." $Yellow
    
    try {
        docker --version | Out-Null
        aws --version | Out-Null
        Write-ColorMessage "? All prerequisites met" $Green
        return $true
    }
    catch {
        Write-ColorMessage "? Missing prerequisites. Please install Docker and AWS CLI." $Red
        return $false
    }
}

function Build-AppRunnerImage {
    Write-ColorMessage "??? Building App Runner compatible image (Port 8080)..." $Yellow
    
    # Build with no cache to ensure fresh build
    $buildOutput = docker build -t $ProjectName . --no-cache 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? App Runner compatible image built successfully" $Green
        
        # Verify the image exposes port 8080
        $imageInfo = docker inspect $ProjectName | ConvertFrom-Json
        $exposedPorts = $imageInfo[0].Config.ExposedPorts
        
        if ($exposedPorts -and $exposedPorts.PSObject.Properties.Name -contains "8080/tcp") {
            Write-ColorMessage "? Image correctly exposes port 8080" $Green
        } else {
            Write-ColorMessage "?? Warning: Image may not expose port 8080 correctly" $Yellow
        }
        
        return $true
    } else {
        Write-ColorMessage "? Docker build failed:" $Red
        Write-Host $buildOutput -ForegroundColor Red
        return $false
    }
}

function Test-LocalContainer {
    Write-ColorMessage "?? Testing container locally on port 8080..." $Yellow
    
    # Run container temporarily to test
    $containerId = docker run -d -p 8080:8080 $ProjectName 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "Container started, waiting for initialization..." $Blue
        Start-Sleep -Seconds 15
        
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:8080/health" -TimeoutSec 10 -UseBasicParsing
            if ($response.StatusCode -eq 200) {
                Write-ColorMessage "? Local container test successful (Port 8080)" $Green
                $testResult = $true
            } else {
                Write-ColorMessage "? Local container test failed (Status: $($response.StatusCode))" $Red
                $testResult = $false
            }
        } catch {
            Write-ColorMessage "? Local container test failed: $($_.Exception.Message)" $Red
            $testResult = $false
        }
        
        # Stop and remove test container
        docker stop $containerId | Out-Null
        docker rm $containerId | Out-Null
        
        return $testResult
    } else {
        Write-ColorMessage "? Failed to start test container" $Red
        return $false
    }
}

function Push-ImageToECR {
    $EcrUri = "$AccountId.dkr.ecr.$Region.amazonaws.com/$ProjectName"
    
    Write-ColorMessage "??? Tagging image for ECR..." $Yellow
    docker tag "${ProjectName}:latest" "${EcrUri}:latest"
    
    Write-ColorMessage "?? Logging into ECR..." $Yellow
    $loginResult = aws ecr get-login-password --region $Region | docker login --username AWS --password-stdin $EcrUri 2>&1
    
    if ($LASTEXITCODE -ne 0) {
        Write-ColorMessage "? ECR login failed" $Red
        Write-Host $loginResult -ForegroundColor Red
        return $false
    }
    
    Write-ColorMessage "?? Pushing App Runner compatible image to ECR..." $Yellow
    $pushResult = docker push "${EcrUri}:latest" 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Image pushed successfully to ECR" $Green
        Write-ColorMessage "?? Image URI: ${EcrUri}:latest" $Blue
        return $EcrUri
    } else {
        Write-ColorMessage "? Failed to push image to ECR" $Red
        Write-Host $pushResult -ForegroundColor Red
        return $false
    }
}

function Show-AppRunnerConfiguration {
    param([string]$EcrUri)
    
    Write-ColorMessage "?? APP RUNNER CONFIGURATION GUIDE" $Green
    Write-Host ""
    Write-ColorMessage "Your image is now ready for App Runner deployment!" $Green
    Write-Host ""
    Write-ColorMessage "?? App Runner Service Configuration:" $Blue
    Write-Host "   Image URI: $EcrUri:latest"
    Write-Host "   Port: 8080"
    Write-Host "   Protocol: HTTP"
    Write-Host "   Health Check Path: /health"
    Write-Host ""
    Write-ColorMessage "?? Environment Variables (Required):" $Yellow
    Write-Host "   DATABASE_URL=<your-postgres-connection-string>"
    Write-Host "   JWT_SECRET_KEY=<your-jwt-secret>"
    Write-Host "   SMTP_SERVER=restaurantegandarias-com.correoseguro.dinaserver.com"
    Write-Host "   SMTP_PORT=587"
    Write-Host "   SMTP_USER=horarios@restaurantegandarias.com"
    Write-Host "   SMTP_PASSWORD=<your-smtp-password>"
    Write-Host "   FRONTEND_URL=https://your-frontend-url"
    Write-Host "   PYTHON_API_URL=https://your-python-api-url"
    Write-Host "   ENCRYPTION_KEY=<your-encryption-key>"
    Write-Host "   ENCRYPTION_IV=<your-encryption-iv>"
    Write-Host ""
    Write-ColorMessage "? Performance Settings:" $Blue
    Write-Host "   CPU: 1 vCPU (or higher)"
    Write-Host "   Memory: 2 GB (or higher)"
    Write-Host "   Max Concurrency: 100"
    Write-Host "   Max Size: 10"
    Write-Host ""
    Write-ColorMessage "?? Next Steps:" $Yellow
    Write-Host "1. Go to App Runner Console: https://console.aws.amazon.com/apprunner/home?region=$Region#/services"
    Write-Host "2. Update your existing service or create a new one"
    Write-Host "3. Use the image URI above"
    Write-Host "4. Configure port 8080 (should be automatic)"
    Write-Host "5. Set the environment variables listed above"
    Write-Host "6. Deploy and test at your App Runner URL"
    Write-Host ""
    Write-ColorMessage "? Port 8080 Benefits:" $Green
    Write-Host "   • App Runner's preferred port"
    Write-Host "   • Consistent across all environments"
    Write-Host "   • No more port mapping issues"
    Write-Host "   • Better container compatibility"
}

function Main {
    Write-ColorMessage "?? AWS App Runner Deployment (Port 8080)" $Green
    Write-ColorMessage "========================================" $Blue
    Write-Host ""
    
    if (!(Test-Prerequisites)) {
        exit 1
    }
    
    Write-Host ""
    if (!(Build-AppRunnerImage)) {
        exit 1
    }
    
    Write-Host ""
    if (!(Test-LocalContainer)) {
        if (!$Force) {
            Write-ColorMessage "? Local container test failed. Use -Force to continue anyway." $Red
            exit 1
        } else {
            Write-ColorMessage "?? Local test failed but continuing due to -Force flag" $Yellow
        }
    }
    
    Write-Host ""
    $EcrUri = Push-ImageToECR
    
    if (!$EcrUri) {
        exit 1
    }
    
    Write-Host ""
    Show-AppRunnerConfiguration -EcrUri $EcrUri
    
    Write-Host ""
    Write-ColorMessage "?? DEPLOYMENT READY!" $Green
    Write-ColorMessage "Your App Runner compatible image (Port 8080) has been deployed to ECR" $Green
}

# Run the main function
Main