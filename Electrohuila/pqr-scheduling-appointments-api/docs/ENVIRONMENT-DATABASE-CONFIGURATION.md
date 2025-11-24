# 🌍 Configuración de Entornos y Bases de Datos

## 📋 Resumen

Cada entorno tiene asignada **una base de datos específica y fija**, excepto el entorno **MAIN (Producción)** que tiene acceso a todas las bases de datos y puede cambiar dinámicamente entre ellas.

---

## 🎯 Distribución de Entornos

| Entorno | Puerto | Base de Datos | Puede cambiar BD | Variable de Entorno |
|---------|--------|---------------|------------------|---------------------|
| **DEV** | 5000 | Oracle (fija) | ❌ No | `Development` |
| **QA** | 5002 | SQL Server (fija) | ❌ No | `QA` |
| **STAGING** | 5001 | PostgreSQL (fija) | ❌ No | `Staging` |
| **MAIN** | 8080 | MySQL (por defecto) + Todas | ✅ Sí | `Production` |

---

## 🔧 Entorno DEV (Desarrollo)

### Configuración
- **Puerto**: `5000`
- **Base de Datos**: Oracle (única y fija)
- **Host**: `pqragendamientocitas.cfim4cuiqpgk.us-east-2.rds.amazonaws.com:1521/PQR`
- **Variable de Entorno**: `ASPNETCORE_ENVIRONMENT=Development`

### Características
- ✅ Solo puede usar Oracle
- ❌ No puede cambiar a otras bases de datos
- ⚠️ Cualquier intento de usar `?database=` será ignorado

### Ejemplo de Uso

```bash
# ✅ Funciona - Usa Oracle
GET http://localhost:5000/api/v1/Clients

# ⚠️ Ignora el parámetro - Sigue usando Oracle
GET http://localhost:5000/api/v1/Clients?database=mysql
```

### Archivo `.env` (devops/dev/.env)

```bash
DATABASE_PROVIDER=Oracle
ORACLE_CONNECTION_STRING=User Id=admin;Password=Superman5564;Data Source=pqragendamientocitas.cfim4cuiqpgk.us-east-2.rds.amazonaws.com:1521/PQR;Connection Timeout=60;
```

### Logs Esperados

```
[Middleware] ⚠️ Attempt to change database provider ignored. Environment 'Development' only allows fixed database. Use MAIN environment for dynamic database switching.
[DbContextFactory] Creating DbContext with provider: Oracle
[DbContextFactory] Oracle configured
```

---

## 🧪 Entorno QA (Control de Calidad)

### Configuración
- **Puerto**: `5002`
- **Base de Datos**: SQL Server (única y fija)
- **Host**: `pqr3.cfim4cuiqpgk.us-east-2.rds.amazonaws.com`
- **Database**: `master`
- **Variable de Entorno**: `ASPNETCORE_ENVIRONMENT=QA`

### Características
- ✅ Solo puede usar SQL Server
- ❌ No puede cambiar a otras bases de datos
- ⚠️ Cualquier intento de usar `?database=` será ignorado

### Ejemplo de Uso

```bash
# ✅ Funciona - Usa SQL Server
GET http://localhost:5002/api/v1/Clients

# ⚠️ Ignora el parámetro - Sigue usando SQL Server
GET http://localhost:5002/api/v1/Clients?database=oracle
```

### Archivo `.env` (devops/qa/.env)

```bash
DATABASE_PROVIDER=SqlServer
SQLSERVER_CONNECTION_STRING=Server=pqr3.cfim4cuiqpgk.us-east-2.rds.amazonaws.com;Database=master;User Id=admin;Password=Superman5564;TrustServerCertificate=True;Encrypt=False;
```

### Logs Esperados

```
[Middleware] ⚠️ Attempt to change database provider ignored. Environment 'QA' only allows fixed database. Use MAIN environment for dynamic database switching.
[DbContextFactory] Creating DbContext with provider: SqlServer
[DbContextFactory] SQL Server configured
```

---

## 🚀 Entorno STAGING (Pre-Producción)

### Configuración
- **Puerto**: `5001`
- **Base de Datos**: PostgreSQL (única y fija)
- **Host**: `pqr2.cfim4cuiqpgk.us-east-2.rds.amazonaws.com`
- **Database**: `PQR2`
- **Variable de Entorno**: `ASPNETCORE_ENVIRONMENT=Staging`

### Características
- ✅ Solo puede usar PostgreSQL
- ❌ No puede cambiar a otras bases de datos
- ⚠️ Cualquier intento de usar `?database=` será ignorado

### Ejemplo de Uso

```bash
# ✅ Funciona - Usa PostgreSQL
GET http://localhost:5001/api/v1/Clients

# ⚠️ Ignora el parámetro - Sigue usando PostgreSQL
GET http://localhost:5001/api/v1/Clients?database=sqlserver
```

### Archivo `.env` (devops/staging/.env)

```bash
DATABASE_PROVIDER=PostgreSQL
POSTGRESQL_CONNECTION_STRING=Host=pqr2.cfim4cuiqpgk.us-east-2.rds.amazonaws.com;Database=PQR2;Username=postgres;Password=Superman5564;
```

### Logs Esperados

```
[Middleware] ⚠️ Attempt to change database provider ignored. Environment 'Staging' only allows fixed database. Use MAIN environment for dynamic database switching.
[DbContextFactory] Creating DbContext with provider: PostgreSQL
[DbContextFactory] PostgreSQL configured
```

---

## ⚡ Entorno MAIN (Producción)

### Configuración
- **Puerto**: `8080`
- **Base de Datos por Defecto**: MySQL
- **Variable de Entorno**: `ASPNETCORE_ENVIRONMENT=Production`

### Características
- ✅ Base de datos por defecto: MySQL
- ✅ **PUEDE cambiar entre todas las bases de datos**
- ✅ Usa el parámetro `?database=` para cambiar dinámicamente
- ✅ Tiene acceso a: Oracle, SQL Server, PostgreSQL y MySQL

### Bases de Datos Disponibles

| Base de Datos | Host | Database |
|---------------|------|----------|
| **Oracle** | pqragendamientocitas.cfim4cuiqpgk.us-east-2.rds.amazonaws.com:1521 | PQR |
| **SQL Server** | pqr3.cfim4cuiqpgk.us-east-2.rds.amazonaws.com | master |
| **PostgreSQL** | pqr2.cfim4cuiqpgk.us-east-2.rds.amazonaws.com | PQR2 |
| **MySQL** | pqr4.cfim4cuiqpgk.us-east-2.rds.amazonaws.com | PQR4 |

### Ejemplo de Uso

```bash
# ✅ Usa MySQL (por defecto)
GET http://localhost:8080/api/v1/Clients

# ✅ Cambia a Oracle
GET http://localhost:8080/api/v1/Clients?database=oracle

# ✅ Cambia a SQL Server
GET http://localhost:8080/api/v1/Clients?database=sqlserver

# ✅ Cambia a PostgreSQL
GET http://localhost:8080/api/v1/Clients?database=postgresql

# ✅ Usa MySQL explícitamente
GET http://localhost:8080/api/v1/Clients?database=mysql
```

### Archivo `.env` (devops/main/.env)

```bash
DATABASE_PROVIDER=MySQL

# Todas las cadenas de conexión disponibles
ORACLE_CONNECTION_STRING=User Id=admin;Password=Superman5564;Data Source=pqragendamientocitas.cfim4cuiqpgk.us-east-2.rds.amazonaws.com:1521/PQR;Connection Timeout=60;
SQLSERVER_CONNECTION_STRING=Server=pqr3.cfim4cuiqpgk.us-east-2.rds.amazonaws.com;Database=master;User Id=admin;Password=Superman5564;TrustServerCertificate=True;Encrypt=False;
POSTGRESQL_CONNECTION_STRING=Host=pqr2.cfim4cuiqpgk.us-east-2.rds.amazonaws.com;Database=PQR2;Username=postgres;Password=Superman5564;
MYSQL_CONNECTION_STRING=Server=pqr4.cfim4cuiqpgk.us-east-2.rds.amazonaws.com;Database=PQR4;User=admin;Password=Superman5564;SslMode=None;
```

### Logs Esperados

```
# Usando MySQL por defecto
[DbContextFactory] Creating DbContext with provider: MySQL
[DbContextFactory] MySQL configured

# Cambiando a Oracle
[Middleware] ✅ Database provider from query parameter: oracle (MAIN environment)
[Middleware] 🔄 Database provider set to Oracle for this request
[DbContextFactory] Creating DbContext with provider: Oracle
[DbContextFactory] Oracle configured
```

---

## 🛡️ Implementación de Seguridad

### Middleware de Control

El `DatabaseProviderMiddleware` verifica el entorno actual antes de permitir el cambio de base de datos:

```csharp
public async Task InvokeAsync(HttpContext context)
{
    // Verificar si estamos en el entorno MAIN (Producción)
    var environment = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
    var isMainEnvironment = environment.Equals("Production", StringComparison.OrdinalIgnoreCase) 
                         || environment.Equals("Main", StringComparison.OrdinalIgnoreCase);

    if (context.Request.Query.TryGetValue("database", out var dbFromQuery))
    {
        providerString = dbFromQuery.ToString();
        
        if (!isMainEnvironment)
        {
            _logger.LogWarning(
                "⚠️ Attempt to change database provider ignored. Environment '{Environment}' only allows fixed database.",
                environment);
            providerString = null; // Ignorar el cambio
        }
    }
    
    // ... resto del código
}
```

### ¿Por qué esta configuración?

1. **DEV**: Oracle es la base de datos principal del proyecto
2. **QA**: SQL Server para probar compatibilidad con Microsoft
3. **STAGING**: PostgreSQL para probar código abierto
4. **MAIN**: MySQL por defecto + acceso a todas para máxima flexibilidad

---

## 🚢 Despliegue

### Levantar cada entorno

```bash
# DEV (Oracle)
cd devops/dev
docker-compose up -d --build

# QA (SQL Server)
cd devops/qa
docker-compose up -d --build

# STAGING (PostgreSQL)
cd devops/staging
docker-compose up -d --build

# MAIN (MySQL + Todas)
cd devops/main
docker-compose up -d --build
```

### Verificar conectividad

```bash
# DEV
curl http://localhost:5000/api/v1/DatabaseHealth/check

# QA
curl http://localhost:5002/api/v1/DatabaseHealth/check

# STAGING
curl http://localhost:5001/api/v1/DatabaseHealth/check

# MAIN - Probar todas las BDs
curl http://localhost:8080/api/v1/DatabaseHealth/test-all
```

---

## 📊 Diagrama de Arquitectura

```
┌─────────────────────────────────────────────────────────────┐
│                        ENTORNOS                             │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
        ┌─────────────────────────────────────────┐
        │                                         │
   ┌────▼────┐  ┌────────┐  ┌──────────┐  ┌──────▼──────┐
   │   DEV   │  │   QA   │  │ STAGING  │  │    MAIN     │
   │ :5000   │  │ :5002  │  │  :5001   │  │   :8080     │
   └────┬────┘  └───┬────┘  └────┬─────┘  └──────┬──────┘
        │           │            │                │
        │           │            │                │
   ┌────▼────┐ ┌───▼─────┐ ┌────▼─────┐  ┌──────▼──────┐
   │ Oracle  │ │   SQL   │ │PostgreSQL│  │   MySQL +   │
   │  (fija) │ │  Server │ │  (fija)  │  │  TODAS      │
   └─────────┘ │  (fija) │ └──────────┘  └─────────────┘
               └─────────┘                        │
                                         ┌────────┴────────┐
                                         │                 │
                                    ┌────▼────┐      ┌────▼────┐
                                    │ Oracle  │      │   SQL   │
                                    └─────────┘      │  Server │
                                                     └────┬────┘
                                                          │
                                                   ┌──────▼──────┐
                                                   │ PostgreSQL  │
                                                   └─────────────┘
```

---

## 🔍 Verificación de Configuración

### Script de Prueba

Crea un archivo `test-environments.sh`:

```bash
#!/bin/bash

echo "🧪 Probando configuración de entornos..."
echo ""

echo "📌 DEV (Oracle - Fija):"
curl -s http://localhost:5000/api/v1/DatabaseHealth/info | jq '.currentProvider'
curl -s http://localhost:5000/api/v1/DatabaseHealth/info?database=mysql | jq '.currentProvider'
echo ""

echo "📌 QA (SQL Server - Fija):"
curl -s http://localhost:5002/api/v1/DatabaseHealth/info | jq '.currentProvider'
curl -s http://localhost:5002/api/v1/DatabaseHealth/info?database=oracle | jq '.currentProvider'
echo ""

echo "📌 STAGING (PostgreSQL - Fija):"
curl -s http://localhost:5001/api/v1/DatabaseHealth/info | jq '.currentProvider'
curl -s http://localhost:5001/api/v1/DatabaseHealth/info?database=sqlserver | jq '.currentProvider'
echo ""

echo "📌 MAIN (MySQL + Todas - Dinámica):"
curl -s http://localhost:8080/api/v1/DatabaseHealth/info | jq '.currentProvider'
curl -s http://localhost:8080/api/v1/DatabaseHealth/info?database=oracle | jq '.currentProvider'
curl -s http://localhost:8080/api/v1/DatabaseHealth/info?database=sqlserver | jq '.currentProvider'
curl -s http://localhost:8080/api/v1/DatabaseHealth/info?database=postgresql | jq '.currentProvider'
curl -s http://localhost:8080/api/v1/DatabaseHealth/info?database=mysql | jq '.currentProvider'
echo ""

echo "✅ Pruebas completadas"
```

### Resultados Esperados

```
🧪 Probando configuración de entornos...

📌 DEV (Oracle - Fija):
"Oracle"
"Oracle"  ← Ignora el parámetro ?database=mysql

📌 QA (SQL Server - Fija):
"SqlServer"
"SqlServer"  ← Ignora el parámetro ?database=oracle

📌 STAGING (PostgreSQL - Fija):
"PostgreSQL"
"PostgreSQL"  ← Ignora el parámetro ?database=sqlserver

📌 MAIN (MySQL + Todas - Dinámica):
"MySQL"  ← Por defecto
"Oracle"  ← ✅ Cambia correctamente
"SqlServer"  ← ✅ Cambia correctamente
"PostgreSQL"  ← ✅ Cambia correctamente
"MySQL"  ← ✅ Cambia correctamente

✅ Pruebas completadas
```

---

## 🎓 Resumen

### Ventajas de esta Configuración

✅ **Aislamiento por Entorno**: Cada entorno tiene su BD específica
✅ **Seguridad**: Solo MAIN puede cambiar de BD dinámicamente
✅ **Consistencia**: DEV, QA y STAGING siempre usan la misma BD
✅ **Flexibilidad en Producción**: MAIN puede acceder a todas las BDs
✅ **Fácil Debugging**: Sabes exactamente qué BD usa cada entorno

### Casos de Uso

- **Desarrollo**: Usa Oracle para desarrollo diario
- **QA**: Prueba con SQL Server para validar compatibilidad
- **Staging**: Verifica con PostgreSQL antes de producción
- **Producción**: MySQL por defecto, pero puede consultar otras BDs si es necesario

---

## 📝 Autor

**Diego Arias**  
ElectroHuila - Sistema de Agendamiento de Citas PQR  
Fecha: Octubre 2025

