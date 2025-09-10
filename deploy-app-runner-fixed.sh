#!/bin/bash
# AWS App Runner FIXED Deployment Script - Gandarias API
# This version fixes the container exit code 139 issue

set -e

# Configuration
PROJECT_NAME="back-gandarias-api"  # Using the name that worked
REGION="eu-central-1"  # Use the region from your logs
ECR_REPOSITORY="${PROJECT_NAME}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${GREEN}?? AWS App Runner FIXED Deployment - Gandarias API${NC}"
echo -e "${BLUE}================================================${NC}"
echo ""

# Get AWS Account ID
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text 2>/dev/null)
if [ -z "$ACCOUNT_ID" ]; then
    echo -e "${RED}? Unable to get AWS Account ID${NC}"
    exit 1
fi
echo -e "${BLUE}?? AWS Account ID: ${ACCOUNT_ID}${NC}"

# Build and push fixed Docker image
echo -e "${YELLOW}?? Building FIXED Docker image...${NC}"

ECR_URI="${ACCOUNT_ID}.dkr.ecr.${REGION}.amazonaws.com/${ECR_REPOSITORY}"

# Build image with fixes
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
echo -e "${YELLOW}??  Pushing FIXED image to ECR...${NC}"
docker push ${ECR_URI}:latest

if [ $? -eq 0 ]; then
    echo -e "${GREEN}? Image pushed successfully to ECR${NC}"
    echo -e "${BLUE}?? Image URI: ${ECR_URI}:latest${NC}"
else
    echo -e "${RED}? Failed to push image to ECR${NC}"
    exit 1
fi

echo ""
echo -e "${GREEN}?? FIXED image deployed to ECR successfully!${NC}"
echo ""
echo -e "${YELLOW}?? Next steps for App Runner:${NC}"
echo ""
echo -e "${BLUE}1. Go to your App Runner service in AWS Console:${NC}"
echo -e "   https://console.aws.amazon.com/apprunner/home?region=${REGION}#/services"
echo ""
echo -e "${BLUE}2. Click on your gandarias-api service${NC}"
echo ""
echo -e "${BLUE}3. Click 'Deploy' ? 'Manual deployment'${NC}"
echo ""
echo -e "${BLUE}4. IMPORTANT: Make sure these environment variables are set:${NC}"
echo -e "${GREEN}   DATABASE_URL=Host=YOUR_RDS_ENDPOINT;Port=5432;Username=gandariasadmin;Password=YOUR_PASSWORD;Database=gandarias;Include Error Detail=true; SSL Mode=Require;Trust Server Certificate=true${NC}"
echo -e "${GREEN}   ASPNETCORE_ENVIRONMENT=Production${NC}"
echo -e "${GREEN}   JWT_SECRET_KEY=YOUR_JWT_SECRET${NC}"
echo -e "${GREEN}   SMTP_SERVER=restaurantegandarias-com.correoseguro.dinaserver.com${NC}"
echo -e "${GREEN}   SMTP_PORT=587${NC}"
echo -e "${GREEN}   SMTP_USER=horarios@restaurantegandarias.com${NC}"
echo -e "${GREEN}   SMTP_PASSWORD=123Gandarias456!${NC}"
echo ""
echo -e "${BLUE}5. Optional: If you want to seed initial data, add this variable:${NC}"
echo -e "${GREEN}   FORCE_SEED_DATA=true${NC}"
echo ""
echo -e "${YELLOW}?? Fixes applied in this version:${NC}"
echo -e "   ? Fixed seed data execution to prevent production crashes"
echo -e "   ? Improved database connection handling"
echo -e "   ? Better error handling and logging"
echo -e "   ? Environment variable configuration fixes"
echo -e "   ? Removed HTTPS redirection in development"
echo ""
echo -e "${GREEN}The container should now start successfully without exit code 139!${NC}"