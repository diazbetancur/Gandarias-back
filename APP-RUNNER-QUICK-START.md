# ?? AWS App Runner - Resumen Ejecutivo
## Gandarias API - Despliegue Inmediato

### ?? **¡Tu API está lista para producción en 15 minutos!**

---

## ?? **Costo Total: $40-60/mes**
- ? **App Runner**: $25-35/mes (0.25 vCPU, 0.5 GB)
- ? **RDS PostgreSQL**: $15-20/mes (db.t3.micro)
- ? **SSL/HTTPS**: GRATIS incluido
- ? **Auto-scaling**: GRATIS incluido
- ? **Monitoreo**: GRATIS incluido

---

## ?? **Desplegar AHORA - 3 Pasos**

### **1?? Prerequisites (5 minutos)**
```bash
# Instalar AWS CLI (si no lo tienes)
# Windows: https://awscli.amazonaws.com/AWSCLIV2.msi
# Configurar credenciales
aws configure

# Verificar Docker
docker --version
```

### **2?? Ejecutar Script (10 minutos)**

**En Linux/Mac:**
```bash
chmod +x deploy-app-runner.sh
./deploy-app-runner.sh
```

**En Windows:**
```powershell
Set-ExecutionPolicy RemoteSigned -Scope CurrentUser
.\deploy-app-runner.ps1
```

### **3?? Crear App Runner Service (5 minutos)**
1. **Ir a**: https://console.aws.amazon.com/apprunner/home?region=us-east-1#/services
2. **Click**: "Create service"
3. **Configurar**:
   - Source: Container registry ? Amazon ECR
   - Repository: `TU_ACCOUNT_ID.dkr.ecr.us-east-1.amazonaws.com/gandarias-api:latest`
   - Service name: `gandarias-api`
   - CPU: 0.25 vCPU, Memory: 0.5 GB
4. **Variables de entorno**: Copiar desde `app-runner-env-template.txt`

---

## ?? **Archivos Incluidos**
- ? `deploy-app-runner.sh` - Script Linux/Mac
- ? `deploy-app-runner.ps1` - Script Windows PowerShell
- ? `test-local.sh` - Testing local opcional
- ? `apprunner.yaml` - Configuración App Runner
- ? `APP-RUNNER-GUIDE.md` - Guía completa paso a paso

---

## ?? **Resultado Final**
Después del despliegue tendrás:

?? **API URL**: `https://xxxxxxxx.us-east-1.awsapprunner.com`
?? **Swagger**: `https://xxxxxxxx.us-east-1.awsapprunner.com/swagger`
?? **Health**: `https://xxxxxxxx.us-east-1.awsapprunner.com/health`

**Features automáticos:**
- ? **SSL/HTTPS** - Certificado automático
- ? **Auto-scaling** - De 1 a 25 instancias automáticamente
- ? **Alta disponibilidad** - Multi-AZ automático
- ? **Monitoreo** - CloudWatch logs integrado
- ? **Zero downtime deployments**
- ? **Load balancing** automático

---

## ?? **Actualizar API (futuro)**
```bash
# 1. Hacer cambios en código
# 2. Re-ejecutar script
./deploy-app-runner.sh

# 3. En App Runner console ? Deploy
```

---

## ?? **Soporte Rápido**
Si algo falla:
1. **Ver logs**: CloudWatch ? Log groups ? `/aws/apprunner/gandarias-api`
2. **Health check**: Verificar que `/health` responde
3. **Variables**: Verificar en App Runner console

---

## ?? **¿Por qué App Runner?**
- ?? **Más barato** que ECS Fargate ($40 vs $90+)
- ?? **Más simple** - Zero configuración de infraestructura
- ?? **Más rápido** - Despliegue en minutos
- ?? **Production-ready** desde día 1
- ?? **Escalable** automáticamente

---

## ?? **¡EMPEZAR AHORA!**

```bash
# Test local (opcional)
./test-local.sh

# Deploy a producción
./deploy-app-runner.sh
```

**¡Tu API Gandarias estará en producción en AWS en menos de 20 minutos!** ??

?? **Siguiente paso**: Ejecuta `./deploy-app-runner.sh` o `.\deploy-app-runner.ps1`