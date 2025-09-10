#!/bin/bash
# AWS Deployment Script for Gandarias API

echo "?? Starting AWS deployment for Gandarias API..."

# Set variables
APP_NAME="gandarias-api"
REGION="us-east-1"  # Change to your preferred region
ECR_REPOSITORY_URI="your-account-id.dkr.ecr.${REGION}.amazonaws.com/${APP_NAME}"

# Build Docker image
echo "?? Building Docker image..."
docker build -t $APP_NAME .

# Tag for ECR
docker tag $APP_NAME:latest $ECR_REPOSITORY_URI:latest

# Login to ECR (requires AWS CLI configured)
echo "?? Logging into ECR..."
aws ecr get-login-password --region $REGION | docker login --username AWS --password-stdin $ECR_REPOSITORY_URI

# Push to ECR
echo "?? Pushing image to ECR..."
docker push $ECR_REPOSITORY_URI:latest

echo "? Docker image pushed successfully!"
echo "Next steps:"
echo "1. Create RDS PostgreSQL instance"
echo "2. Create App Runner service pointing to: $ECR_REPOSITORY_URI:latest"
echo "3. Set environment variables in App Runner"