#!/bin/bash
# AWS App Runner Deployment Script - Gandarias API
# Optimizado para la opción más económica y sencilla

set -e

# Configuration
PROJECT_NAME="gandarias-api"
REGION="us-east-1"
ECR_REPOSITORY="${PROJECT_NAME}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${GREEN}?? AWS App Runner Deployment - Gandarias API${NC}"
echo -e "${BLUE}===============================================${NC}"
echo ""

# Function to check prerequisites
check_prerequisites() {
    echo -e "${YELLOW}?? Checking prerequisites...${NC}"
    
    # Check AWS CLI
    if ! command -v aws &> /dev/null; then
        echo -e "${RED}? AWS CLI not installed. Please install it first.${NC}"
        echo -e "   Download from: https://aws.amazon.com/cli/"
        exit 1
    fi
    
    # Check Docker
    if ! command -v docker &> /dev/null; then
        echo -e "${RED}? Docker not installed. Please install it first.${NC}"
        echo -e "   Download from: https://docker.com/get-started"
        exit 1
    fi
    
    # Check AWS credentials
    if ! aws sts get-caller-identity &> /dev/null; then
        echo -e "${RED}? AWS credentials not configured.${NC}"
        echo -e "   Run: aws configure"
        exit 1
    fi
    
    echo -e "${GREEN}? All prerequisites met!${NC}"
}

# Function to get AWS Account ID
get_account_id() {
    ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text 2>/dev/null)
    if [ -z "$ACCOUNT_ID" ]; then
        echo -e "${RED}? Unable to get AWS Account ID${NC}"
        exit 1
    fi
    echo -e "${BLUE}?? AWS Account ID: ${ACCOUNT_ID}${NC}"
}

# Function to create ECR repository
create_ecr_repository() {
    echo -e "${YELLOW}?? Setting up ECR repository...${NC}"
    
    # Check if repository exists
    if aws ecr describe-repositories --repository-names ${ECR_REPOSITORY} --region ${REGION} &> /dev/null; then
        echo -e "${BLUE}??  ECR repository already exists${NC}"
    else
        echo -e "${YELLOW}Creating ECR repository...${NC}"
        aws ecr create-repository \
            --repository-name ${ECR_REPOSITORY} \
            --region ${REGION} \
            --image-scanning-configuration scanOnPush=true > /dev/null
        echo -e "${GREEN}? ECR repository created${NC}"
    fi
}

# Function to build and push Docker image
build_and_push_image() {
    echo -e "${YELLOW}?? Building Docker image...${NC}"
    
    ECR_URI="${ACCOUNT_ID}.dkr.ecr.${REGION}.amazonaws.com/${ECR_REPOSITORY}"
    
    # Build image
    docker build -t ${PROJECT_NAME} . --no-cache
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}? Docker image built successfully${NC}"
    else
        echo -e "${RED}? Docker build failed${NC}"
        exit 1
    fi
    
    # Tag for ECR
    docker tag ${PROJECT_NAME}:latest ${ECR_URI}:latest
    
    # Login to ECR
    echo -e "${YELLOW}?? Logging into ECR...${NC}"
    aws ecr get-login-password --region ${REGION} | \
        docker login --username AWS --password-stdin ${ECR_URI}
    
    # Push image
    echo -e "${YELLOW}??  Pushing image to ECR...${NC}"
    docker push ${ECR_URI}:latest
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}? Image pushed successfully to ECR${NC}"
        echo -e "${BLUE}?? Image URI: ${ECR_URI}:latest${NC}"
    else
        echo -e "${RED}? Failed to push image to ECR${NC}"
        exit 1
    fi
}

# Function to create RDS database
create_rds_database() {
    echo -e "${YELLOW}???  Setting up RDS PostgreSQL database...${NC}"
    
    DB_IDENTIFIER="${PROJECT_NAME}-db"
    
    # Check if database exists
    if aws rds describe-db-instances --db-instance-identifier ${DB_IDENTIFIER} --region ${REGION} &> /dev/null; then
        echo -e "${BLUE}??  RDS database already exists${NC}"
        DB_ENDPOINT=$(aws rds describe-db-instances \
            --db-instance-identifier ${DB_IDENTIFIER} \
            --region ${REGION} \
            --query 'DBInstances[0].Endpoint.Address' \
            --output text)
    else
        echo -e "${YELLOW}Creating RDS PostgreSQL database...${NC}"
        
        # Prompt for database password
        echo -e "${YELLOW}Please enter a password for the database (minimum 8 characters):${NC}"
        read -s DB_PASSWORD
        echo ""
        
        if [ ${#DB_PASSWORD} -lt 8 ]; then
            echo -e "${RED}? Password must be at least 8 characters${NC}"
            exit 1
        fi
        
        # Create database
        aws rds create-db-instance \
            --db-instance-identifier ${DB_IDENTIFIER} \
            --db-instance-class db.t3.micro \
            --engine postgres \
            --engine-version 15.4 \
            --master-username gandariasadmin \
            --master-user-password ${DB_PASSWORD} \
            --allocated-storage 20 \
            --storage-type gp2 \
            --backup-retention-period 7 \
            --no-multi-az \
            --publicly-accessible \
            --region ${REGION} > /dev/null
        
        echo -e "${YELLOW}? Waiting for database to become available (this may take 5-10 minutes)...${NC}"
        aws rds wait db-instance-available \
            --db-instance-identifier ${DB_IDENTIFIER} \
            --region ${REGION}
        
        # Get database endpoint
        DB_ENDPOINT=$(aws rds describe-db-instances \
            --db-instance-identifier ${DB_IDENTIFIER} \
            --region ${REGION} \
            --query 'DBInstances[0].Endpoint.Address' \
            --output text)
        
        echo -e "${GREEN}? RDS database created successfully${NC}"
    fi
    
    echo -e "${BLUE}???  Database endpoint: ${DB_ENDPOINT}${NC}"
}

# Function to display App Runner setup instructions
display_app_runner_instructions() {
    echo -e "${GREEN}?? Ready for App Runner deployment!${NC}"
    echo ""
    echo -e "${YELLOW}?? Next steps - Create App Runner service:${NC}"
    echo ""
    echo -e "${BLUE}1. Go to AWS App Runner Console:${NC}"
    echo -e "   https://console.aws.amazon.com/apprunner/home?region=${REGION}#/services"
    echo ""
    echo -e "${BLUE}2. Create service with these settings:${NC}"
    echo -e "   • Source: Container registry ? Amazon ECR"
    echo -e "   • Repository: ${ECR_URI}"
    echo -e "   • Tag: latest"
    echo -e "   • Deployment trigger: Manual (recommended)"
    echo ""
    echo -e "${BLUE}3. Service settings:${NC}"
    echo -e "   • Service name: ${PROJECT_NAME}"
    echo -e "   • Virtual CPU: 0.25 vCPU"
    echo -e "   • Virtual memory: 0.5 GB"
    echo -e "   • Port: 80"
    echo ""
    echo -e "${BLUE}4. Environment variables (IMPORTANT):${NC}"
    echo -e "   ASPNETCORE_ENVIRONMENT=Production"
    echo -e "   DATABASE_URL=Host=${DB_ENDPOINT};Port=5432;Username=gandariasadmin;Password=YOUR_PASSWORD;Database=gandarias;Include Error Detail=true; SSL Mode=Require;Trust Server Certificate=true"
    echo -e "   JWT_SECRET_KEY=YOUR_JWT_SECRET_KEY"
    echo -e "   SMTP_SERVER=your-smtp-server"
    echo -e "   SMTP_PORT=587"
    echo -e "   SMTP_USER=your-smtp-user"
    echo -e "   SMTP_PASSWORD=your-smtp-password"
    echo -e "   FRONTEND_URL=https://your-frontend-domain.com"
    echo -e "   PYTHON_API_URL=https://gchlubzoicprv6c7ruoaoe6dmi0jqxpg.lambda-url.us-east-2.on.aws"
    echo -e "   ENCRYPTION_KEY=O383TLC7mk3/ow8MWglORMBc8GRWzAfdieLTQDbf7As="
    echo -e "   ENCRYPTION_IV=KOOFbeLOvSo5li1ulRVuxA=="
    echo ""
    echo -e "${GREEN}?? Estimated monthly cost: \$40-60${NC}"
    echo -e "${GREEN}?? Your API will be available at the App Runner URL${NC}"
    echo -e "${GREEN}?? Swagger UI: https://your-app-runner-url/swagger${NC}"
    echo ""
}

# Function to create environment file template
create_env_template() {
    echo -e "${YELLOW}?? Creating environment template...${NC}"
    
    cat > app-runner-env-template.txt << EOF
# App Runner Environment Variables Template
# Copy these to your App Runner service configuration

ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=Host=${DB_ENDPOINT};Port=5432;Username=gandariasadmin;Password=YOUR_DATABASE_PASSWORD;Database=gandarias;Include Error Detail=true; SSL Mode=Require;Trust Server Certificate=true
JWT_SECRET_KEY=lk34j5l34jjk34h5kasadsf#\$%SfaetfASDfASDFA345345345##\$%#FASefaRQ@#\$%eFGEAY%\$SEVQ345wfw344tw4tqTW#Vw5gw45ytq%T@\$%DFASDFasdfasdASDFasdfASDF#\$%34534#\$SDF
SMTP_SERVER=restaurantegandarias-com.correoseguro.dinaserver.com
SMTP_PORT=587
SMTP_USER=horarios@restaurantegandarias.com
SMTP_PASSWORD=123Gandarias456!
FRONTEND_URL=https://calm-bush-00d363a0f.6.azurestaticapps.net
PYTHON_API_URL=https://gchlubzoicprv6c7ruoaoe6dmi0jqxpg.lambda-url.us-east-2.on.aws
ENCRYPTION_KEY=O383TLC7mk3/ow8MWglORMBc8GRWzAfdieLTQDbf7As=
ENCRYPTION_IV=KOOFbeLOvSo5li1ulRVuxA==
EOF

    echo -e "${GREEN}? Environment template created: app-runner-env-template.txt${NC}"
}

# Main execution
main() {
    check_prerequisites
    get_account_id
    create_ecr_repository
    build_and_push_image
    create_rds_database
    create_env_template
    display_app_runner_instructions
}

# Run main function
main