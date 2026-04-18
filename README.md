# SolicitudServidores

Sistema para gestionar **solicitudes, usuarios, servidores, VPNs, subdominios, evidencias, cartas responsivas y reportes** de infraestructura.

---

## ✅ Funcionalidades disponibles

- **Login JWT** con usuario demo
- **CRUD de usuarios** con roles
- **CRUD de solicitudes** con notificaciones
- **CRUD de servidores** con múltiples recursos por solicitud
- Registro de:
  - tipo de uso (`Interno` / `Publicado`)
  - IP opcional, llave de licencia
  - VPN y fecha de expiración
  - subdominios y evidencias de pruebas
  - tareas pendientes
- **Gestión de VPNs**
- **Cartas responsivas** (generación desde plantillas `.docx`)
- **Reportes** de:
  - vulnerabilidades pendientes
  - revisión anual pendiente
  - VPNs por expirar
- **Frontend estático** en `Back/wwwroot`
- **Swagger** para explorar y probar la API

---

## 🚀 Opción 1: Ejecutar sin Docker (recomendada)

Usa **SQLite local** — no requiere PostgreSQL ni ninguna otra base de datos.

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js + npm](https://nodejs.org/) (para el frontend Angular)

### Backend

```powershell
cd Back
dotnet restore
dotnet run --urls "http://localhost:5055"
```

### Frontend Angular

En otra terminal:

```powershell
cd Front
npm install
npm start
```

### Script todo-en-uno

```powershell
.\run-local.ps1
```

Levanta backend y frontend en ventanas separadas automáticamente.

### URLs

| Servicio | URL |
|---|---|
| Frontend Angular | http://localhost:4200 |
| API / UI estática | http://localhost:5055 |
| Swagger | http://localhost:5055/swagger |
| Health check | http://localhost:5055/health |

### Credenciales demo

- **Correo:** `admin@local`
- **Contraseña:** `admin`

### Base de datos local

Se crea automáticamente en `Back/solicitud_servidores_dev.db`.

Para reiniciarla desde cero, edita `Back/variables.env`:

```env
RESET_SQLITE_ON_START=true
```

---

## 🐳 Opción 2: Ejecutar con Docker

Requiere Docker Desktop instalado y en ejecución.

```powershell
docker compose up -d --build
```

| Servicio | URL |
|---|---|
| API / UI estática | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |

En modo Docker la API usa **PostgreSQL** automáticamente.

Para correr los tests en contenedor:

```powershell
docker compose --profile test run --rm tests
```

---

## 🔎 Endpoints principales

### Autenticación
- `POST /api/auth/login`

### Usuarios
- `GET /api/usuario`
- `GET /api/usuario/todos`
- `GET /api/usuario/{id}`
- `POST /api/usuario`
- `PUT /api/usuario/{id}`
- `DELETE /api/usuario/{id}`
- `GET /api/usuario/roles`

### Solicitudes
- `GET /api/solicitud`
- `GET /api/solicitud/todas`
- `GET /api/solicitud/{id}`
- `GET /api/solicitud/usuario/{idUsuario}`
- `POST /api/solicitud`
- `PUT /api/solicitud/{id}`
- `DELETE /api/solicitud/{id}`
- `GET /api/solicitud/notificaciones/nuevas`
- `PUT /api/solicitud/{id}/notificacion-leida`

### Servidores
- `GET /api/servidor`
- `GET /api/servidor/{id}`
- `GET /api/servidor/solicitud/{idSolicitud}`
- `POST /api/servidor`
- `PUT /api/servidor/{id}`
- `DELETE /api/servidor/{id}`
- `GET /api/servidor/recursos-predeterminados`

### Reportes
- `GET /api/servidor/reportes/vulnerabilidades-pendientes`
- `GET /api/servidor/reportes/revision-anual`
- `GET /api/servidor/reportes/vpns-por-expirar?dias=30`

### VPNs
- `GET /api/vpn`
- `GET /api/vpn/{id}`
- `POST /api/vpn`
- `PUT /api/vpn/{id}`
- `DELETE /api/vpn/{id}`

### Cartas responsivas
- `GET /api/carta`
- `POST /api/carta`
- `GET /api/carta/{id}`

---

## 📁 Estructura del proyecto

```
├── Back/                         # API .NET 8
│   ├── Controller/               # Controladores (Auth, Usuario, Solicitud, Servidor, VPN, Carta)
│   ├── Models/                   # Entidades del dominio
│   ├── DTOs/                     # Objetos de transferencia de datos
│   ├── Repositories/             # Acceso a datos
│   ├── Services/                 # Lógica de negocio
│   ├── Helpers/                  # Utilidades internas
│   ├── Utilities/                # Herramientas compartidas
│   ├── DBContext/                # Contexto de Entity Framework
│   ├── Migrations/               # Migraciones de base de datos
│   ├── wwwroot/                  # Frontend estático para pruebas rápidas
│   ├── variables.env             # Variables de entorno locales
│   └── Dockerfile
├── Front/                        # Frontend Angular
├── docs/                         # Plantillas de documentos (.docx)
├── tools/                        # Scripts auxiliares (extract_docx.py)
├── docker-compose.yml
└── run-local.ps1                 # Script para levantar backend + frontend
```

---

## 💡 Notas

- Sin Docker, **no necesitas PostgreSQL**. El proyecto arranca con SQLite local.
- Las variables de API externas (`APID_USER`, `APID_URL`, `APIC_KEY`, `APIC_URL`) en `docker-compose.yml` deben reemplazarse con los valores reales para producción.
- La clave JWT en producción debe cambiarse por un valor seguro.
