# AWS App Runner Deployment Script for Windows PowerShell
# Gandarias API Deployment

param(
    [string]$Region = "us-east-1",
    [string]$ProjectName = "gandarias-api"
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
    
    # Check AWS CLI
    try {
        aws --version | Out-Null
        Write-ColorMessage "? AWS CLI is installed" $Green
    }
    catch {
        Write-ColorMessage "? AWS CLI not installed. Please install it first." $Red
        Write-ColorMessage "   Download from: https://aws.amazon.com/cli/" $Blue
        exit 1
    }
    
    # Check Docker
    try {
        docker --version | Out-Null
        Write-ColorMessage "? Docker is installed" $Green
    }
    catch {
        Write-ColorMessage "? Docker not installed. Please install it first." $Red
        Write-ColorMessage "   Download from: https://docker.com/get-started" $Blue
        exit 1
    }
    
    # Check AWS credentials
    try {
        aws sts get-caller-identity | Out-Null
        Write-ColorMessage "? AWS credentials configured" $Green
    }
    catch {
        Write-ColorMessage "? AWS credentials not configured." $Red
        Write-ColorMessage "   Run: aws configure" $Blue
        exit 1
    }
}

function Get-AWSAccountId {
    try {
        $AccountId = aws sts get-caller-identity --query Account --output text
        Write-ColorMessage "?? AWS Account ID: $AccountId" $Blue
        return $AccountId
    }
    catch {
        Write-ColorMessage "? Unable to get AWS Account ID" $Red
        exit 1
    }
}

function New-ECRRepository {
    Write-ColorMessage "?? Setting up ECR repository..." $Yellow
    
    try {
        aws ecr describe-repositories --repository-names $ProjectName --region $Region | Out-Null
        Write-ColorMessage "??  ECR repository already exists" $Blue
    }
    catch {
        Write-ColorMessage "Creating ECR repository..." $Yellow
        aws ecr create-repository --repository-name $ProjectName --region $Region --image-scanning-configuration scanOnPush=true | Out-Null
        Write-ColorMessage "? ECR repository created" $Green
    }
}

function Build-AndPushImage {
    param([string]$AccountId)
    
    Write-ColorMessage "?? Building Docker image..." $Yellow
    
    $EcrUri = "$AccountId.dkr.ecr.$Region.amazonaws.com/$ProjectName"
    
    # Build image
    docker build -t $ProjectName . --no-cache
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Docker image built successfully" $Green
    } else {
        Write-ColorMessage "? Docker build failed" $Red
        exit 1
    }
    
    # Tag for ECR
    docker tag "${ProjectName}:latest" "${EcrUri}:latest"
    
    # Login to ECR
    Write-ColorMessage "?? Logging into ECR..." $Yellow
    aws ecr get-login-password --region $Region | docker login --username AWS --password-stdin $EcrUri
    
    # Push image
    Write-ColorMessage "??  Pushing image to ECR..." $Yellow
    docker push "${EcrUri}:latest"
    
    if ($LASTEXITCODE -eq 0) {
        Write-ColorMessage "? Image pushed successfully to ECR" $Green
        Write-ColorMessage "?? Image URI: ${EcrUri}:latest" $Blue
    } else {
        Write-ColorMessage "? Failed to push image to ECR" $Red
        exit 1
    }
    
    return $EcrUri
}

function New-RDSDatabase {
    Write-ColorMessage "???  Setting up RDS PostgreSQL database..." $Yellow
    
    $DbIdentifier = "$ProjectName-db"
    
    try {
        $DbInfo = aws rds describe-db-instances --db-instance-identifier $DbIdentifier --region $Region | ConvertFrom-Json
        Write-ColorMessage "??  RDS database already exists" $Blue
        $DbEndpoint = $DbInfo.DBInstances[0].Endpoint.Address
    }
    catch {
        Write-ColorMessage "Creating RDS PostgreSQL database..." $Yellow
        
        # Prompt for database password
        Write-ColorMessage "Please enter a password for the database (minimum 8 characters):" $Yellow
        $DbPassword = Read-Host -AsSecureString
        $DbPasswordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($DbPassword))
        
        if ($DbPasswordPlain.Length -lt 8) {
            Write-ColorMessage "? Password must be at least 8 characters" $Red
            exit 1
        }
        
        # Create database
        aws rds create-db-instance `
            --db-instance-identifier $DbIdentifier `
            --db-instance-class db.t3.micro `
            --engine postgres `
            --engine-version 15.4 `
            --master-username gandariasadmin `
            --master-user-password $DbPasswordPlain `
            --allocated-storage 20 `
            --storage-type gp2 `
            --backup-retention-period 7 `
            --no-multi-az `
            --publicly-accessible `
            --region $Region | Out-Null
        
        Write-ColorMessage "? Waiting for database to become available (this may take 5-10 minutes)..." $Yellow
        aws rds wait db-instance-available --db-instance-identifier $DbIdentifier --region $Region
        
        # Get database endpoint
        $DbInfo = aws rds describe-db-instances --db-instance-identifier $DbIdentifier --region $Region | ConvertFrom-Json
        $DbEndpoint = $DbInfo.DBInstances[0].Endpoint.Address
        
        Write-ColorMessage "? RDS database created successfully" $Green
    }
    
    Write-ColorMessage "???  Database endpoint: $DbEndpoint" $Blue
    return $DbEndpoint
}

function New-EnvironmentTemplate {
    param([string]$DbEndpoint)
    
    Write-ColorMessage "?? Creating environment template..." $Yellow
    
    $EnvTemplate = @"
# App Runner Environment Variables Template
# Copy these to your App Runner service configuration

ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=Host=$DbEndpoint;Port=5432;Username=gandariasadmin;Password=YOUR_DATABASE_PASSWORD;Database=gandarias;Include Error Detail=true; SSL Mode=Require;Trust Server Certificate=true
JWT_SECRET_KEY=lk34j5l34jjk34h5kasadsf#`$%SfaetfASDfASDFA345345345##`$%#FASefaRQ@#`$%eFGEAY%`$SEVQ345wfw344tw4tqTW#Vw5gw45ytq%T@`$%DFASDFasdfasdASDFasdfASDF#`$%34534#`$SDF
SMTP_SERVER=restaurantegandarias-com.correoseguro.dinaserver.com
SMTP_PORT=587
SMTP_USER=horarios@restaurantegandarias.com
SMTP_PASSWORD=123Gandarias456!
FRONTEND_URL=https://calm-bush-00d363a0f.6.azurestaticapps.net
PYTHON_API_URL=https://gchlubzoicprv6c7ruoaoe6dmi0jqxpg.lambda-url.us-east-2.on.aws
ENCRYPTION_KEY=O383TLC7mk3/ow8MWglORMBc8GRWzAfdieLTQDbf7As=
ENCRYPTION_IV=KOOFbeLOvSo5li1ulRVuxA==
"@

    $EnvTemplate | Out-File -FilePath "app-runner-env-template.txt" -Encoding UTF8
    Write-ColorMessage "? Environment template created: app-runner-env-template.txt" $Green
}

function Show-AppRunnerInstructions {
    param([string]$EcrUri)
    
    Write-ColorMessage "?? Ready for App Runner deployment!" $Green
    Write-Host ""
    Write-ColorMessage "?? Next steps - Create App Runner service:" $Yellow
    Write-Host ""
    Write-ColorMessage "1. Go to AWS App Runner Console:" $Blue
    Write-Host "   https://console.aws.amazon.com/apprunner/home?region=$Region#/services"
    Write-Host ""
    Write-ColorMessage "2. Create service with these settings:" $Blue
    Write-Host "   • Source: Container registry ? Amazon ECR"
    Write-Host "   • Repository: $EcrUri"
    Write-Host "   • Tag: latest"
    Write-Host "   • Deployment trigger: Manual (recommended)"
    Write-Host ""
    Write-ColorMessage "3. Service settings:" $Blue
    Write-Host "   • Service name: $ProjectName"
    Write-Host "   • Virtual CPU: 0.25 vCPU"
    Write-Host "   • Virtual memory: 0.5 GB"
    Write-Host "   • Port: 80"
    Write-Host ""
    Write-ColorMessage "4. Environment variables (copy from app-runner-env-template.txt):" $Blue
    Write-Host ""
    Write-ColorMessage "?? Estimated monthly cost: `$40-60" $Green
    Write-ColorMessage "?? Your API will be available at the App Runner URL" $Green
    Write-ColorMessage "?? Swagger UI: https://your-app-runner-url/swagger" $Green
    Write-Host ""
}

# Main execution
function Main {
    Write-ColorMessage "?? AWS App Runner Deployment - Gandarias API" $Green
    Write-ColorMessage "===============================================" $Blue
    Write-Host ""
    
    Test-Prerequisites
    $AccountId = Get-AWSAccountId
    New-ECRRepository
    $EcrUri = Build-AndPushImage -AccountId $AccountId
    $DbEndpoint = New-RDSDatabase
    New-EnvironmentTemplate -DbEndpoint $DbEndpoint
    Show-AppRunnerInstructions -EcrUri $EcrUri
}

# Run the main function
Main