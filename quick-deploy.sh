#!/bin/bash
# Simple ECS Fargate Deployment - Quick Start

PROJECT_NAME="gandarias-api"
REGION="us-east-1"

echo "?? Quick ECS Fargate Deployment for Gandarias API"
echo "================================================"

# Step 1: Test locally first
echo "?? Testing locally with Docker Compose..."
docker-compose up --build -d
sleep 10

# Test health endpoint
if curl -f http://localhost:5000/health > /dev/null 2>&1; then
    echo "? Local test passed!"
    docker-compose down
else
    echo "? Local test failed. Please check your application."
    docker-compose logs api
    exit 1
fi

# Step 2: Get AWS Account ID
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
echo "?? AWS Account ID: ${ACCOUNT_ID}"

# Step 3: Create ECR repository
echo "?? Creating ECR repository..."
aws ecr create-repository --repository-name ${PROJECT_NAME} --region ${REGION} 2>/dev/null || echo "Repository already exists"

# Step 4: Build and push Docker image
echo "?? Building and pushing Docker image..."
ECR_URI="${ACCOUNT_ID}.dkr.ecr.${REGION}.amazonaws.com/${PROJECT_NAME}"

docker build -t ${PROJECT_NAME} .
docker tag ${PROJECT_NAME}:latest ${ECR_URI}:latest

aws ecr get-login-password --region ${REGION} | docker login --username AWS --password-stdin ${ECR_URI}
docker push ${ECR_URI}:latest

echo "? Docker image pushed successfully!"
echo ""
echo "Next steps:"
echo "1. Run the full deployment: ./deploy-ecs.sh"
echo "2. Or deploy manually through AWS Console using:"
echo "   - ECR Image: ${ECR_URI}:latest"
echo "   - Task Definition: ecs-task-definition.json"
echo "   - Infrastructure: ecs-infrastructure.yaml"
echo ""
echo "?? Your image is ready at: ${ECR_URI}:latest"