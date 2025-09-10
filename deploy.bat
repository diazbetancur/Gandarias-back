@echo off

rem Tu Account ID y región (según tus logs)
set ACCOUNT_ID=752862029990
set REGION=eu-central-1
set PROJECT_NAME=back-gandarias-api

echo Building new image with fixes...
docker build -t %PROJECT_NAME% . --no-cache

if %errorlevel% neq 0 (
echo ERROR: Docker build failed. Exiting.
exit /b 1
)

echo Tagging image...
docker tag %PROJECT_NAME%:latest %ACCOUNT_ID%.dkr.ecr.%REGION%https://www.google.com/search?q=.amazonaws.com/%25PROJECT_NAME%25:latest

if %errorlevel% neq 0 (
echo ERROR: Docker tag failed. Exiting.
exit /b 1
)

echo Logging in to ECR...
aws ecr get-login-password --region %REGION% | docker login --username AWS --password-stdin %ACCOUNT_ID%.dkr.ecr.%REGION%.amazonaws.com

if %errorlevel% neq 0 (
echo ERROR: ECR login failed. Exiting.
exit /b 1
)

echo Pushing image...
docker push %ACCOUNT_ID%.dkr.ecr.%REGION%https://www.google.com/search?q=.amazonaws.com/%25PROJECT_NAME%25:latest

if %errorlevel% neq 0 (
echo ERROR: Docker push failed. Exiting.
exit /b 1
)

echo Deployment script finished successfully.