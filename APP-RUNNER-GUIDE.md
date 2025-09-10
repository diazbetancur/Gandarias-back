# ?? AWS App Runner - Guía de Despliegue Gandarias API

## ?? ¿Por qué App Runner?

**AWS App Runner es la opción PERFECTA para tu API porque:**
- ? **Más económico**: Solo $40-60/mes (vs $90+ en ECS)
- ? **Zero configuración**: No necesitas manejar VPC, Load Balancers, etc.
- ? **SSL automático**: HTTPS incluido sin configuración
- ? **Auto-scaling**: Escala automáticamente según el tráfico
- ? **Despliegue en minutos**: Tu API estará lista en 10-15 minutos

## ?? Costo Mensual Estimado

| Servicio | Costo | Descripción |
|----------|-------|-------------|
| **App Runner** | $25-35 | 0.25 vCPU, 0.5 GB RAM |
| **RDS PostgreSQL** | $15-20 | db.t3.micro con 20GB |
| **Data Transfer** | $1-5 | Transferencia de datos |
| **Total** | **$41-60** | ?? ¡Súper económico! |

## ?? Despliegue en 3 Pasos

### **Paso 1: Preparar Prerequisites**

```bash
# 1. Instalar AWS CLI
# Windows: https://awscli.amazonaws.com/AWSCLIV2.msi
# Mac: brew install awscli
# Linux: sudo apt install awscli

# 2. Configurar AWS credentials
aws configure
# Introduce tu Access Key ID y Secret Access Key

# 3. Verificar Docker está instalado
docker --version
```

### **Paso 2: Ejecutar Script de Despliegue**

```bash
# Hacer ejecutable (Linux/Mac)
chmod +x deploy-app-runner.sh

# En Windows PowerShell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Ejecutar despliegue
./deploy-app-runner.sh
```

**El script automáticamente:**
1. ?? Verifica prerequisites
2. ?? Crea repositorio ECR
3. ?? Construye imagen Docker
4. ?? Sube imagen a ECR
5. ??? Crea base de datos RDS
6. ?? Genera template de variables de entorno

### **Paso 3: Crear Servicio App Runner**

Después de ejecutar el script, sigue estos pasos en la consola de AWS:

#### **3.1 Ir a App Runner Console**
```
https://console.aws.amazon.com/apprunner/home?region=us-east-1#/services
```

#### **3.2 Crear Nuevo Servicio**
1. Click **"Create service"**
2. **Repository type**: Container registry
3. **Provider**: Amazon ECR
4. **Container image URI**: `TU_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/gandarias-api:latest`
5. **Deployment trigger**: Manual (recomendado)

#### **3.3 Configurar Servicio**
```
Service name: gandarias-api
Virtual CPU: 0.25 vCPU
Virtual memory: 0.5 GB
Port: 80
```

#### **3.4 Variables de Entorno (CRÍTICO)**
Copia las variables del archivo `app-runner-env-template.txt` que se generó:

```env
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=Host=TU_RDS_ENDPOINT;Port=5432;Username=gandariasadmin;Password=TU_PASSWORD;Database=gandarias;Include Error Detail=true; SSL Mode=Require;Trust Server Certificate=true
JWT_SECRET_KEY=lk34j5l34jjk34h5kasadsf#$%SfaetfASDfASDFA345345345##$%#FASefaRQ@#$%eFGEAY%$SEVQ345wfw344tw4tqTW#Vw5gw45ytq%T@$%DFASDFasdfasdASDFasdfASDF#$%34534#$SDF
SMTP_SERVER=restaurantegandarias-com.correoseguro.dinaserver.com
SMTP_PORT=587
SMTP_USER=horarios@restaurantegandarias.com
SMTP_PASSWORD=123Gandarias456!
FRONTEND_URL=https://calm-bush-00d363a0f.6.azurestaticapps.net
PYTHON_API_URL=https://gchlubzoicprv6c7ruoaoe6dmi0jqxpg.lambda-url.us-east-2.on.aws
ENCRYPTION_KEY=O383TLC7mk3/ow8MWglORMBc8GRWzAfdieLTQDbf7As=
ENCRYPTION_IV=KOOFbeLOvSo5li1ulRVuxA==
```

#### **3.5 Health Check**
```
Health check path: /health
Health check interval: 20 seconds
Health check timeout: 5 seconds
Healthy threshold: 1
Unhealthy threshold: 5
```

## ?? Después del Despliegue

### **Verificar que Todo Funciona**
```bash
# 1. Health check
curl https://tu-app-runner-url.amazonaws.com/health

# 2. Swagger UI
# Ve a: https://tu-app-runner-url.amazonaws.com/swagger

# 3. Test API endpoint
curl https://tu-app-runner-url.amazonaws.com/api/User
```

### **Configurar Dominio Personalizado (Opcional)**
1. En App Runner console ? Custom domains
2. Agregar tu dominio: `api.tudominio.com`
3. Crear CNAME record en tu DNS:
   ```
   CNAME api TU_APP_RUNNER_URL
   ```

## ?? Gestión y Monitoreo

### **Ver Logs**
```bash
# App Runner logs en CloudWatch
aws logs filter-log-events \
  --log-group-name "/aws/apprunner/gandarias-api/service" \
  --region us-east-1
```

### **Escalar Servicio**
En App Runner console:
- **Auto scaling**: Min 1, Max 25 instances
- **Concurrency**: 100 concurrent requests per instance

### **Actualizar Aplicación**
```bash
# 1. Hacer cambios en código
# 2. Re-ejecutar script
./deploy-app-runner.sh

# 3. En App Runner console: Deploy ? Manual deployment
```

## ?? Seguridad

### **Variables de Entorno Sensibles**
Para mayor seguridad, usa AWS Systems Manager Parameter Store:

```bash
# Crear parámetros seguros
aws ssm put-parameter \
  --name "/gandarias/db-password" \
  --value "tu_password_seguro" \
  --type "SecureString"

# En App Runner, usa:
DATABASE_URL=Host=endpoint;Port=5432;Username=admin;Password={{resolve:ssm-secure:/gandarias/db-password}};Database=gandarias
```

### **CORS y Headers**
Ya configurado en tu `Program.cs` para producción con headers de seguridad.

## ?? Optimización de Costos

### **Development Environment**
Para development, puedes pausar/eliminar recursos:
```bash
# Parar RDS (no disponible en t3.micro, pero puedes eliminar y recrear)
aws rds delete-db-instance --db-instance-identifier gandarias-api-db --skip-final-snapshot

# Eliminar App Runner service cuando no uses
aws apprunner delete-service --service-arn tu-service-arn
```

### **Production Optimizations**
1. **Reserved Instances** para RDS (hasta 60% descuento)
2. **App Runner Spot** cuando esté disponible
3. **CloudFront** para cache global (opcional)

## ?? Troubleshooting

### **Problemas Comunes**

#### **1. App Runner no inicia**
```bash
# Ver logs
aws logs tail /aws/apprunner/gandarias-api/service --follow

# Verificar variables de entorno
# Verificar que el puerto 80 esté correcto
```

#### **2. Base de datos no conecta**
```bash
# Verificar RDS está corriendo
aws rds describe-db-instances --db-instance-identifier gandarias-api-db

# Test conexión desde local
psql -h TU_RDS_ENDPOINT -U gandariasadmin -d gandarias
```

#### **3. Health check falla**
```bash
# Verificar endpoint /health funciona
curl -v https://tu-url/health

# Ver logs de aplicación
# Verificar Program.cs tiene app.MapHealthChecks("/health")
```

## ?? CI/CD (Opcional)

### **GitHub Actions**
Crear `.github/workflows/deploy.yml`:

```yaml
name: Deploy to App Runner
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Configure AWS credentials
        uses: aws-actions/configure-aws-credentials@v2
        with:
          aws-access-key-id: ${{ secrets.AWS_ACCESS_KEY_ID }}
          aws-secret-access-key: ${{ secrets.AWS_SECRET_ACCESS_KEY }}
          aws-region: us-east-1
      
      - name: Deploy to App Runner
        run: |
          chmod +x deploy-app-runner.sh
          ./deploy-app-runner.sh
```

## ?? Comandos Útiles

```bash
# Ver servicios App Runner
aws apprunner list-services --region us-east-1

# Describir servicio específico
aws apprunner describe-service --service-arn tu-service-arn

# Ver deployments
aws apprunner list-operations --service-arn tu-service-arn

# Forzar nuevo deployment
aws apprunner start-deployment --service-arn tu-service-arn

# Ver métricas en CloudWatch
aws cloudwatch get-metric-statistics \
  --namespace AWS/AppRunner \
  --metric-name RequestCount \
  --dimensions Name=ServiceName,Value=gandarias-api \
  --start-time 2024-01-01T00:00:00Z \
  --end-time 2024-01-02T00:00:00Z \
  --period 3600 \
  --statistics Sum
```

## ?? ¡Éxito!

**Tu API Gandarias estará corriendo en:**
- ?? **URL**: `https://tu-app-runner-id.us-east-1.awsapprunner.com`
- ?? **Swagger**: `https://tu-app-runner-id.us-east-1.awsapprunner.com/swagger`
- ?? **Health**: `https://tu-app-runner-id.us-east-1.awsapprunner.com/health`

**Beneficios que obtienes:**
- ? **SSL/HTTPS automático**
- ? **Auto-scaling** basado en tráfico
- ? **Alta disponibilidad** en múltiples AZ
- ? **Monitoreo** con CloudWatch
- ? **Despliegues** con zero downtime
- ? **Logs centralizados**

**¡Tu API está lista para recibir tráfico de producción!** ??

## ?? Soporte

Si tienes problemas:
1. Revisa los logs de CloudWatch
2. Verifica las variables de entorno
3. Confirma que la base de datos RDS está disponible
4. Verifica que la imagen Docker se construyó correctamente

¡Disfruta de tu API en la nube! ???