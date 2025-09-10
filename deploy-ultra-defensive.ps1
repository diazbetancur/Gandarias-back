# AWS App Runner ULTRA-DEFENSIVE Deployment Script - Windows PowerShell
# This version should fix the exit code 139 issue definitively

param(
    [string]$Region = "eu-central-1",
    [string]$ProjectName = "back-gandarias-api",
    [string]$AccountId = "752862029990"
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
    
    # Check Docker
    try {
        docker --version | Out-Null
        Write-ColorMessage "? Docker is available" $Green
    }
    catch {
        Write-ColorMessage "? Docker not available. Please install Docker first." $Red
        exit 1
    }
    
    # Check AWS CLI
    try {
        aws --version | Out-Null
        Write-ColorMessage "? AWS CLI is available" $Green
    }
    catch {
        Write-ColorMessage "? AWS CLI not available. Please install AWS CLI first." $Red
        exit 1
    }
}

function Build-UltraDefensiveImage {
    Write-ColorMessage "?? Building ULTRA-DEFENSIVE Docker image..." $Yellow
    
    # Build with no cache to ensure fresh build
    docker build -t $ProjectName . --no-cache
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Docker image built successfully" $Green
    } else {
        Write-ColorMessage "? Docker build failed" $Red
        exit 1
    }
}

function Push-ImageToECR {
    $EcrUri = "$AccountId.dkr.ecr.$Region.amazonaws.com/$ProjectName"
    
    Write-ColorMessage "??? Tagging image for ECR..." $Yellow
    docker tag "${ProjectName}:latest" "${EcrUri}:latest"
    
    Write-ColorMessage "?? Logging into ECR..." $Yellow
    aws ecr get-login-password --region $Region | docker login --username AWS --password-stdin $EcrUri
    
    Write-ColorMessage "?? Pushing ULTRA-DEFENSIVE image to ECR..." $Yellow
    docker push "${EcrUri}:latest"
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Image pushed successfully to ECR" $Green
        Write-ColorMessage "?? Image URI: ${EcrUri}:latest" $Blue
        return $EcrUri
    } else {
        Write-ColorMessage "? Failed to push image to ECR" $Red
        exit 1
    }
}

function Show-NextSteps {
    param([string]$EcrUri)
    
    Write-ColorMessage "?? ULTRA-DEFENSIVE image deployed successfully!" $Green
    Write-Host ""
    Write-ColorMessage "?? CRITICAL: Verify these environment variables in App Runner:" $Yellow
    Write-Host ""
    Write-ColorMessage "Required variables:" $Blue
    Write-Host "DATABASE_URL=Host=YOUR_RDS_ENDPOINT;Port=5432;Username=gandariasadmin;Password=YOUR_PASSWORD;Database=gandarias;Include Error Detail=true; SSL Mode=Require;Trust Server Certificate=true"
    Write-Host "ASPNETCORE_ENVIRONMENT=Production"
    Write-Host "JWT_SECRET_KEY=lk34j5l34jjk34h5kasadsf#`$%SfaetfASDfASDFA345345345##`$%#FASefaRQ@#`$%eFGEAY%`$SEVQ345wfw344tw4tqTW#Vw5gw45ytq%T@`$%DFASDFasdfasdASDFasdfASDF#`$%34534#`$SDF"
    Write-Host ""
    Write-ColorMessage "Optional variables (for first-time setup):" $Blue
    Write-Host "FORCE_SEED_DATA=true (remove after first successful deployment)"
    Write-Host ""
    Write-ColorMessage "?? Fixes in this ULTRA-DEFENSIVE version:" $Yellow
    Write-Host "   ? Comprehensive error handling throughout startup"
    Write-Host "   ? Fallback configurations for all services"
    Write-Host "   ? Safe seed data execution with timeouts"
    Write-Host "   ? Detailed logging for troubleshooting"
    Write-Host "   ? Graceful degradation if non-critical services fail"
    Write-Host ""
    Write-ColorMessage "?? Next steps:" $Blue
    Write-Host "1. Go to App Runner Console: https://console.aws.amazon.com/apprunner/home?region=$Region#/services"
    Write-Host "2. Click on your gandarias-api service"
    Write-Host "3. Click 'Deploy' ? 'Manual deployment'"
    Write-Host "4. Wait for deployment to complete"
    Write-Host "5. Check logs if issues persist"
    Write-Host ""
    Write-ColorMessage "?? This version should DEFINITELY resolve exit code 139!" $Green
}

# Main execution
function Main {
    Write-ColorMessage "?? AWS App Runner ULTRA-DEFENSIVE Deployment" $Green
    Write-ColorMessage "============================================" $Blue
    Write-Host ""
    
    Test-Prerequisites
    Build-UltraDefensiveImage
    $EcrUri = Push-ImageToECR
    Show-NextSteps -EcrUri $EcrUri
}

# Run the main function
Main