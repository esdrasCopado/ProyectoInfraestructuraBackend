# SolicitudServidores

Sistema para gestionar **solicitudes, usuarios, servidores, VPNs, subdominios, evidencias y reportes** de infraestructura.

---

## ✅ Lo que ya puedes probar

- **Login JWT** con usuario demo
- **CRUD de usuarios**
- **CRUD de solicitudes**
- **CRUD de servidores**
- Solicitudes con **múltiples servidores**
- Registro de:
  - tipo de uso (`Interno` / `Publicado`)
  - IP opcional
  - llave de licencia
  - VPN y expiración
  - subdominios
  - evidencias de pruebas
  - tareas pendientes
- **Reportes** de:
  - vulnerabilidades pendientes
  - revisión anual pendiente
  - VPNs por expirar
- **Frontend estático** en `Back/wwwroot`
- **Swagger** para probar la API

---

## 🚀 Opción 1: Probar **sin Docker** (recomendada)

Esta opción ya está configurada para usar **SQLite local**.

### Requisitos
- Tener instalado **.NET 8 SDK**

### Pasos
```powershell
cd "c:\Users\Louie\Downloads\resguardo-servidores-back-esdras\Back"
dotnet restore
dotnet build
dotnet run --urls "http://localhost:5055"
```

### Abrir en el navegador
- API/UI estática: `http://localhost:5055/`
- Swagger: `http://localhost:5055/swagger`
- Health: `http://localhost:5055/health`

### Front Angular
En otra terminal:
```powershell
cd "c:\Users\Louie\Downloads\resguardo-servidores-back-esdras\Front"
npm install
npm start
```

Abre:
- Angular: `http://localhost:4200/`

El frontend Angular usa `proxy.conf.json` para redirigir `/api` y `/health` al backend local en `http://localhost:5055`.

### Script rápido
También puedes levantar ambos con:
```powershell
cd "c:\Users\Louie\Downloads\resguardo-servidores-back-esdras"
.\run-local.ps1
```

### Credenciales demo
- **Correo:** `admin@local`
- **Contraseña:** `admin`

### Base de datos local
Se crea automáticamente en:
- `Back/solicitud_servidores_dev.db`

Si quieres reiniciar la base local desde cero, puedes borrar ese archivo o cambiar en `Back/variables.env`:
```env
RESET_SQLITE_ON_START=true
```

---

## 🐳 Opción 2: Probar **con Docker**

Si tienes Docker Desktop instalado y ejecutándose:

```powershell
cd "c:\Users\Louie\Downloads\resguardo-servidores-back-esdras"
docker compose up -d --build
```

Luego abre:
- API/UI: `http://localhost:8080/`
- Swagger: `http://localhost:8080/swagger`

En Docker, la API usa **PostgreSQL** automáticamente.

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

### Servidores y reportes
- `GET /api/servidor`
- `GET /api/servidor/{id}`
- `GET /api/servidor/solicitud/{idSolicitud}`
- `POST /api/servidor`
- `PUT /api/servidor/{id}`
- `DELETE /api/servidor/{id}`
- `GET /api/servidor/recursos-predeterminados`
- `GET /api/servidor/reportes/vulnerabilidades-pendientes`
- `GET /api/servidor/reportes/revision-anual`
- `GET /api/servidor/reportes/vpns-por-expirar?dias=30`

---

## 🧪 Verificación real hecha

Se validó localmente en este entorno:

- `dotnet build` ✅
- `GET /health` ✅ devuelve `provider = sqlite`
- `POST /api/auth/login` ✅
- `POST /api/usuario` ✅
- `POST /api/solicitud` ✅
- `POST /api/servidor` ✅
- reportes de revisión anual / VPNs / vulnerabilidades ✅
- `GET /` ✅ sirve la interfaz web

---

## 📁 Estructura principal

- `Back/` → API .NET 8 + frontend estático
- `Back/wwwroot/` → interfaz para probar el sistema
- `Back/Controller/` → controladores CRUD
- `Back/Models/` → entidades del dominio
- `Back/Repositories/` → acceso a datos
- `docs/` → plantillas y documentos
- `Front/` → base Angular opcional

---

## 💡 Nota

Si usas el modo sin Docker, **no necesitas PostgreSQL**. El proyecto arranca con SQLite local y ya te deja probar todo lo implementado.
# ProyectoInfraestructuraBackend
