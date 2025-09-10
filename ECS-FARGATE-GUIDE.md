# ?? ECS Fargate Deployment Guide - Gandarias API

## ?? Overview

Este gu�a te ayudar� a desplegar tu API de Gandarias en AWS ECS Fargate, una opci�n serverless altamente escalable y confiable.

## ?? Costo Estimado Mensual

| Componente | Costo Mensual | Descripci�n |
|------------|---------------|-------------|
| ECS Fargate (2 tareas, 0.25 vCPU, 0.5GB) | $15-25 | Containers de la API |
| Application Load Balancer | $16 | Balanceador de carga |
| RDS PostgreSQL (db.t3.micro) | $15-20 | Base de datos |
| NAT Gateways (2) | $45 | Conectividad de red |
| CloudWatch Logs | $2-5 | Monitoreo y logs |
| **Total** | **$93-111** | |

> **Nota:** Para reducir costos, puedes usar una sola zona de disponibilidad y eliminar un NAT Gateway (~$22 menos)

## ??? Arquitectura

```
Internet
    ?
Application Load Balancer (Public Subnets)
    ?
ECS Fargate Tasks (Private Subnets)
    ?
RDS PostgreSQL (Private Subnets)
```

## ?? Componentes Incluidos

### 1. **ecs-infrastructure.yaml** - CloudFormation Template
- VPC completa con subnets p�blicas y privadas
- Application Load Balancer con SSL/TLS
- ECS Cluster con Fargate
- RDS PostgreSQL
- Auto Scaling autom�tico
- Security Groups configurados
- CloudWatch Logs

### 2. **ecs-task-definition.json** - Definici�n de Task
- Configuraci�n de contenedor optimizada
- Health checks configurados
- Manejo seguro de secretos con SSM
- Resource limits apropiados

### 3. **deploy-ecs.sh** - Script de Despliegue
- Automatizaci�n completa del despliegue
- Construcci�n y push de imagen Docker
- Actualizaci�n de servicios ECS
- Validaci�n de estado del despliegue

## ?? Despliegue Paso a Paso

### Prerequisitos

1. **AWS CLI configurado**
   ```bash
   aws configure
   ```

2. **Docker instalado**
   ```bash
   docker --version
   ```

3. **Permisos AWS necesarios**
   - ECS Full Access
   - ECR Full Access
   - CloudFormation Full Access
   - RDS Full Access
   - EC2 Full Access
   - IAM Role Creation

### Paso 1: Preparar el Entorno

```bash
# Clonar o navegar a tu directorio del proyecto
cd /path/to/gandarias-back

# Hacer el script ejecutable (Linux/Mac)
chmod +x deploy-ecs.sh

# En Windows PowerShell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Paso 2: Desplegar la Infraestructura

```bash
# Ejecutar el script de despliegue
./deploy-ecs.sh
```

El script te pedir�:
- **Database Password:** Contrase�a para PostgreSQL (m�n. 8 caracteres)
- **JWT Secret Key:** Clave secreta para JWT

### Paso 3: Configurar Par�metros Adicionales (Opcional)

```bash
# Agregar par�metros adicionales en SSM Parameter Store
aws ssm put-parameter --name "/gandarias/smtp-server" --value "your-smtp-server" --type "String"
aws ssm put-parameter --name "/gandarias/smtp-user" --value "your-smtp-user" --type "String"
aws ssm put-parameter --name "/gandarias/smtp-password" --value "your-smtp-password" --type "SecureString"
aws ssm put-parameter --name "/gandarias/frontend-url" --value "https://your-frontend-domain.com" --type "String"
```

## ?? Configuraci�n Avanzada

### Auto Scaling

El despliegue incluye auto scaling autom�tico:
- **M�nimo:** 2 tareas
- **M�ximo:** 10 tareas
- **Trigger:** 70% CPU utilization
- **Scale Out:** 5 minutos cooldown
- **Scale In:** 5 minutos cooldown

### Monitoreo

```bash
# Ver logs en tiempo real
aws logs tail /ecs/gandarias-api --follow

# Ver m�tricas del servicio
aws ecs describe-services --cluster gandarias-api-cluster --services gandarias-api-service

# Ver estado de las tareas
aws ecs list-tasks --cluster gandarias-api-cluster --service-name gandarias-api-service
```

### Actualizaciones

```bash
# Para actualizar la aplicaci�n, simplemente ejecuta de nuevo:
./deploy-ecs.sh
```

## ?? Seguridad

### Manejo de Secretos
- Todas las credenciales se almacenan en AWS Systems Manager Parameter Store
- Encriptaci�n en tr�nsito y en reposo
- Roles IAM con permisos m�nimos necesarios

### Red
- API ejecuta en subnets privadas
- Solo el Load Balancer es p�blico
- Security Groups restrictivos
- NAT Gateways para acceso saliente

## ?? SSL/TLS (Opcional)

Para habilitar HTTPS:

1. **Crear certificado SSL en ACM:**
   ```bash
   aws acm request-certificate --domain-name api.tudominio.com --validation-method DNS
   ```

2. **Actualizar el Listener del ALB:**
   ```bash
   # Agregar listener HTTPS en el puerto 443
   # Redirigir HTTP a HTTPS
   ```

## ?? Monitoreo y Alertas

### CloudWatch Dashboards
```bash
# Crear dashboard personalizado
aws cloudwatch put-dashboard --dashboard-name "Gandarias-API" --dashboard-body file://dashboard.json
```

### Alertas
```bash
# Alerta por alta utilizaci�n de CPU
aws cloudwatch put-metric-alarm \
  --alarm-name "Gandarias-High-CPU" \
  --alarm-description "Alert when CPU exceeds 80%" \
  --metric-name CPUUtilization \
  --namespace AWS/ECS \
  --statistic Average \
  --period 300 \
  --threshold 80 \
  --comparison-operator GreaterThanThreshold
```

## ?? Troubleshooting

### Problemas Comunes

1. **Service no se despliega:**
   ```bash
   # Ver eventos del servicio
   aws ecs describe-services --cluster gandarias-api-cluster --services gandarias-api-service
   ```

2. **Health check falla:**
   ```bash
   # Verificar logs de la aplicaci�n
   aws logs filter-log-events --log-group-name "/ecs/gandarias-api" --filter-pattern "ERROR"
   ```

3. **Base de datos no conecta:**
   ```bash
   # Verificar security groups y par�metros de conexi�n
   aws ssm get-parameter --name "/gandarias/database-url" --with-decryption
   ```

## ?? CI/CD (Opcional)

Para automatizar despliegues con GitHub Actions:

```yaml
# .github/workflows/deploy-ecs.yml
name: Deploy to ECS
on:
  push:
    branches: [main]
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - name: Deploy to ECS
        run: |
          chmod +x deploy-ecs.sh
          ./deploy-ecs.sh
```

## ?? Comandos �tiles

```bash
# Escalar servicio manualmente
aws ecs update-service --cluster gandarias-api-cluster --service gandarias-api-service --desired-count 5

# Forzar nuevo despliegue
aws ecs update-service --cluster gandarias-api-cluster --service gandarias-api-service --force-new-deployment

# Ver logs en tiempo real
aws logs tail /ecs/gandarias-api --follow --region us-east-1

# Conectar a base de datos (desde EC2 bastion)
psql -h <rds-endpoint> -U gandariasadmin -d gandarias

# Eliminar todo el stack
aws cloudformation delete-stack --stack-name gandarias-api-infrastructure
```

## ?? Siguientes Pasos

1. **Configurar dominio personalizado** con Route 53
2. **Implementar CI/CD** con GitHub Actions o CodePipeline
3. **Configurar backups** autom�ticos de RDS
4. **Implementar WAF** para protecci�n adicional
5. **Configurar CloudFront** para mejor rendimiento global

## ?? Optimizaciones de Costos

1. **Usar Fargate Spot** para cargas no cr�ticas (hasta 70% descuento)
2. **Reserved Instances** para RDS (hasta 60% descuento)
3. **Single AZ** para desarrollo/testing
4. **Programar parada** de recursos no productivos

---

## ?? Soporte

Si encuentras problemas:
1. Revisa los logs de CloudWatch
2. Verifica la configuraci�n de Security Groups
3. Consulta la documentaci�n de AWS ECS
4. Contacta al equipo de DevOps

�Tu API estar� corriendo en producci�n en menos de 20 minutos! ??