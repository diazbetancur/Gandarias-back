#!/bin/bash
# ECS Fargate Deployment Script for Gandarias API

set -e

# Configuration
PROJECT_NAME="gandarias-api"
REGION="us-east-1"
CLUSTER_NAME="${PROJECT_NAME}-cluster"
SERVICE_NAME="${PROJECT_NAME}-service"
REPOSITORY_NAME="${PROJECT_NAME}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${GREEN}?? Starting ECS Fargate deployment for Gandarias API...${NC}"

# Check if AWS CLI is installed and configured
if ! command -v aws &> /dev/null; then
    echo -e "${RED}? AWS CLI is not installed. Please install it first.${NC}"
    exit 1
fi

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo -e "${RED}? Docker is not installed. Please install it first.${NC}"
    exit 1
fi

# Get AWS Account ID
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
if [ -z "$ACCOUNT_ID" ]; then
    echo -e "${RED}? Unable to get AWS Account ID. Please check your AWS CLI configuration.${NC}"
    exit 1
fi

echo -e "${YELLOW}?? Using Account ID: ${ACCOUNT_ID}${NC}"

# Step 1: Create ECR repository if it doesn't exist
echo -e "${YELLOW}?? Creating ECR repository...${NC}"
aws ecr describe-repositories --repository-names ${REPOSITORY_NAME} --region ${REGION} 2>/dev/null || {
    echo "Repository does not exist. Creating..."
    aws ecr create-repository --repository-name ${REPOSITORY_NAME} --region ${REGION}
    echo -e "${GREEN}? ECR repository created successfully${NC}"
}

# Step 2: Build Docker image
echo -e "${YELLOW}?? Building Docker image...${NC}"
docker build -t ${REPOSITORY_NAME} .

# Step 3: Tag image for ECR
ECR_URI="${ACCOUNT_ID}.dkr.ecr.${REGION}.amazonaws.com/${REPOSITORY_NAME}"
docker tag ${REPOSITORY_NAME}:latest ${ECR_URI}:latest

# Step 4: Login to ECR
echo -e "${YELLOW}?? Logging into ECR...${NC}"
aws ecr get-login-password --region ${REGION} | docker login --username AWS --password-stdin ${ECR_URI}

# Step 5: Push image to ECR
echo -e "${YELLOW}?? Pushing image to ECR...${NC}"
docker push ${ECR_URI}:latest
echo -e "${GREEN}? Image pushed successfully to ECR${NC}"

# Step 6: Check if CloudFormation stack exists
STACK_NAME="${PROJECT_NAME}-infrastructure"
if aws cloudformation describe-stacks --stack-name ${STACK_NAME} --region ${REGION} 2>/dev/null; then
    echo -e "${YELLOW}?? Updating existing CloudFormation stack...${NC}"
    
    # Prompt for required parameters
    echo -e "${YELLOW}Please provide the following parameters:${NC}"
    read -s -p "Database Password (minimum 8 characters): " DB_PASSWORD
    echo
    read -s -p "JWT Secret Key: " JWT_SECRET
    echo
    
    aws cloudformation update-stack \
        --stack-name ${STACK_NAME} \
        --template-body file://ecs-infrastructure.yaml \
        --parameters \
            ParameterKey=DatabasePassword,ParameterValue=${DB_PASSWORD} \
            ParameterKey=JwtSecretKey,ParameterValue=${JWT_SECRET} \
        --capabilities CAPABILITY_NAMED_IAM \
        --region ${REGION}
    
    echo -e "${YELLOW}? Waiting for stack update to complete...${NC}"
    aws cloudformation wait stack-update-complete --stack-name ${STACK_NAME} --region ${REGION}
else
    echo -e "${YELLOW}??? Creating CloudFormation stack...${NC}"
    
    # Prompt for required parameters
    echo -e "${YELLOW}Please provide the following parameters:${NC}"
    read -s -p "Database Password (minimum 8 characters): " DB_PASSWORD
    echo
    read -s -p "JWT Secret Key: " JWT_SECRET
    echo
    
    aws cloudformation create-stack \
        --stack-name ${STACK_NAME} \
        --template-body file://ecs-infrastructure.yaml \
        --parameters \
            ParameterKey=DatabasePassword,ParameterValue=${DB_PASSWORD} \
            ParameterKey=JwtSecretKey,ParameterValue=${JWT_SECRET} \
        --capabilities CAPABILITY_NAMED_IAM \
        --region ${REGION}
    
    echo -e "${YELLOW}? Waiting for stack creation to complete (this may take 10-15 minutes)...${NC}"
    aws cloudformation wait stack-create-complete --stack-name ${STACK_NAME} --region ${REGION}
fi

# Step 7: Update task definition with new image
echo -e "${YELLOW}?? Updating ECS task definition...${NC}"

# Replace placeholders in task definition
sed -e "s/YOUR_ACCOUNT_ID/${ACCOUNT_ID}/g" \
    -e "s/us-east-1/${REGION}/g" \
    ecs-task-definition.json > ecs-task-definition-updated.json

# Register new task definition
TASK_DEFINITION_ARN=$(aws ecs register-task-definition \
    --cli-input-json file://ecs-task-definition-updated.json \
    --region ${REGION} \
    --query 'taskDefinition.taskDefinitionArn' \
    --output text)

echo -e "${GREEN}? Task definition updated: ${TASK_DEFINITION_ARN}${NC}"

# Step 8: Update ECS service
echo -e "${YELLOW}?? Updating ECS service...${NC}"
aws ecs update-service \
    --cluster ${CLUSTER_NAME} \
    --service ${SERVICE_NAME} \
    --task-definition ${TASK_DEFINITION_ARN} \
    --region ${REGION} > /dev/null

echo -e "${YELLOW}? Waiting for service deployment to complete...${NC}"
aws ecs wait services-stable \
    --cluster ${CLUSTER_NAME} \
    --services ${SERVICE_NAME} \
    --region ${REGION}

# Step 9: Get the load balancer URL
LOAD_BALANCER_URL=$(aws cloudformation describe-stacks \
    --stack-name ${STACK_NAME} \
    --region ${REGION} \
    --query 'Stacks[0].Outputs[?OutputKey==`LoadBalancerURL`].OutputValue' \
    --output text)

echo -e "${GREEN}? Deployment completed successfully!${NC}"
echo -e "${GREEN}?? Your API is available at: ${LOAD_BALANCER_URL}${NC}"
echo -e "${GREEN}?? Health check: ${LOAD_BALANCER_URL}/health${NC}"
echo -e "${GREEN}?? Swagger UI: ${LOAD_BALANCER_URL}/swagger${NC}"

# Cleanup temporary files
rm -f ecs-task-definition-updated.json

echo -e "${YELLOW}?? Useful commands:${NC}"
echo -e "  Monitor service: aws ecs describe-services --cluster ${CLUSTER_NAME} --services ${SERVICE_NAME} --region ${REGION}"
echo -e "  View logs: aws logs tail /ecs/${PROJECT_NAME} --follow --region ${REGION}"
echo -e "  Scale service: aws ecs update-service --cluster ${CLUSTER_NAME} --service ${SERVICE_NAME} --desired-count <number> --region ${REGION}"

echo -e "${GREEN}?? Happy coding!${NC}"