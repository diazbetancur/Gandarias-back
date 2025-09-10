# AWS Deployment Guide - Gandarias API

## Prerequisites
- AWS CLI installed and configured
- Docker installed locally
- AWS account with appropriate permissions

## Option 1: AWS App Runner (Recommended - Most Economical)

### Step 1: Create ECR Repository
```bash
aws ecr create-repository --repository-name gandarias-api --region us-east-1
```

### Step 2: Build and Push Docker Image
```bash
# Build the image
docker build -t gandarias-api .

# Tag for ECR
docker tag gandarias-api:latest YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/gandarias-api:latest

# Login to ECR
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com

# Push image
docker push YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/gandarias-api:latest
```

### Step 3: Create RDS PostgreSQL Database
```bash
aws rds create-db-instance \
    --db-instance-identifier gandarias-db \
    --db-instance-class db.t3.micro \
    --engine postgres \
    --master-username gandariasdmin \
    --master-user-password YourStrongPassword123! \
    --allocated-storage 20 \
    --vpc-security-group-ids sg-xxxxxxxx \
    --publicly-accessible \
    --backup-retention-period 7
```

### Step 4: Create App Runner Service
1. Go to AWS App Runner console
2. Create service
3. Source: Container registry ? Amazon ECR
4. Select your ECR repository
5. Set deployment trigger: Manual
6. Configure service:
   - Service name: `gandarias-api`
   - Virtual CPU: 0.25 vCPU
   - Virtual memory: 0.5 GB
   - Environment variables:
     ```
     DATABASE_URL=Host=your-rds-endpoint;Port=5432;Username=gandariasadmin;Password=YourStrongPassword123!;Database=gandarias;Include Error Detail=true; SSL Mode=Require;Trust Server Certificate=true
     JWT_SECRET_KEY=your-jwt-secret-key-here
     ASPNETCORE_ENVIRONMENT=Production
     SMTP_SERVER=your-smtp-server
     SMTP_PORT=587
     SMTP_USER=your-smtp-user
     SMTP_PASSWORD=your-smtp-password
     FRONTEND_URL=https://your-frontend-domain.com
     PYTHON_API_URL=https://gchlubzoicprv6c7ruoaoe6dmi0jqxpg.lambda-url.us-east-2.on.aws
     ENCRYPTION_KEY=O383TLC7mk3/ow8MWglORMBc8GRWzAfdieLTQDbf7As=
     ENCRYPTION_IV=KOOFbeLOvSo5li1ulRVuxA==
     ```

### Step 5: Configure Auto Scaling (Optional)
- Min instances: 1
- Max instances: 5
- Auto scaling: Enabled

## Option 2: EC2 + RDS (Alternative)

### Step 1: Launch EC2 Instance
```bash
aws ec2 run-instances \
    --image-id ami-0c02fb55956c7d316 \
    --count 1 \
    --instance-type t3.micro \
    --key-name your-key-pair \
    --security-group-ids sg-xxxxxxxx \
    --user-data file://user-data.sh
```

### Step 2: User Data Script (user-data.sh)
```bash
#!/bin/bash
yum update -y
yum install -y docker
systemctl start docker
systemctl enable docker
usermod -a -G docker ec2-user

# Install .NET 8
rpm -Uvh https://packages.microsoft.com/config/centos/7/packages-microsoft-prod.rpm
yum install -y dotnet-runtime-8.0

# Pull and run your container
docker pull YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/gandarias-api:latest
docker run -d -p 80:80 --env-file .env YOUR_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/gandarias-api:latest
```

## Estimated Monthly Costs:

### App Runner Option:
- App Runner (0.25 vCPU, 0.5GB): ~$25-35
- RDS PostgreSQL (t3.micro): ~$15-20
- Data transfer: ~$1-5
- **Total: ~$40-60/month**

### EC2 Option:
- EC2 t3.micro: ~$8-10
- RDS PostgreSQL (t3.micro): ~$15-20
- ELB (optional): ~$16
- **Total: ~$25-45/month**

## Post-Deployment:
1. Update DNS records to point to your App Runner or Load Balancer URL
2. Configure SSL certificates (automatically handled by App Runner)
3. Set up monitoring and logging with CloudWatch
4. Configure backup strategies for RDS

## Monitoring:
- Use CloudWatch for application logs
- Set up RDS monitoring
- Configure alerts for high CPU/memory usage

## Security:
- Use AWS Systems Manager Parameter Store for sensitive configuration
- Enable AWS WAF for additional protection
- Regular security updates for container images