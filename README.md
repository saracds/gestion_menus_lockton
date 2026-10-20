# Prueba Técnica Full Stack – Usuarios, Autenticación y Menús Dinámicos

Solución web para registrar usuarios, autenticarlos con JWT y controlar dinámicamente los menús disponibles para cada usuario.

| Capa | Tecnología |
|---|---|
| Base de datos | SQL Server 2022 (script `database/init.sql`) |
| Backend | C# .NET 8, ASP.NET Core Web API, EF Core 8, JWT, Swagger |
| Frontend | React 18 (JavaScript) + Vite, React Router, Tailwind CSS, componentes shadcn/ui |
| Infraestructura | Docker + Docker Compose (4 servicios) |

---

## 1. Ejecución rápida (recomendada)

**Requisito único:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) en ejecución.

```bash
docker compose up --build
```

| Servicio | URL |
|---|---|
| Aplicación web | http://localhost:3000 |
| Swagger (documentación de la API) | http://localhost:5000/swagger |
| SQL Server (SSMS / Azure Data Studio) | `localhost,14333` · usuario `sa` · contraseña `Lockton_Dev_2026!` |

Para detener: `Ctrl + C` y luego `docker compose down`.
Para borrar también la base de datos y empezar de cero: `docker compose down -v`.

> Las contraseñas y la clave JWT por defecto son **solo de desarrollo**. Para cambiarlas, copie `.env.example` como `.env` y edítelo.

```bash
copy .env.example .env
```

### ¿Qué pasa al ejecutar `docker compose up`?

```
db (SQL Server) ──healthy──▶ db-init (ejecuta init.sql y termina) ──ok──▶ api (.NET 8) ◀── web (nginx + React)
```

1. **db**: arranca SQL Server; el *healthcheck* espera a que acepte conexiones.
2. **db-init**: ejecuta `database/init.sql` (crea BD, tablas y datos de prueba).
3. **api**: arranca la API solo cuando el script terminó correctamente.
4. **web**: nginx sirve el React compilado y reenvía `/api/*` al contenedor `api` (mismo origen, sin CORS).

---

## 2. Usuarios de prueba

| Usuario | Contraseña | Rol | Menús asignados |
|---|---|---|---|
| `admin` | `Admin123*` | Administrador | Pólizas, Clientes, Siniestros, Reportes + Administración |
| `ana` | `Ana12345*` | Usuario | Pólizas, Clientes |
| `carlos` | `Carlos123*` | Usuario | Siniestros, Reportes |

Menús creados: **Pólizas** (`polizas`), **Clientes** (`clientes`), **Siniestros** (`siniestros`), **Reportes** (`reportes`).

---

## 3. Arquitectura

```
LOCKTON/
├── database/
│   └── init.sql                 # Creación de BD, tablas, restricciones y datos iniciales
├── backend/                     # Solución .NET 8 en 3 capas
│   ├── Lockton.Api/             # PRESENTACIÓN: controladores, JWT, Swagger, middleware de errores
│   ├── Lockton.Services/        # NEGOCIO: reglas, DTOs, validaciones, hash de contraseñas, tokens
│   ├── Lockton.Data/            # DATOS: entidades, DbContext (EF Core), repositorios
│   └── Dockerfile
├── frontend/                    # React + Vite + Tailwind + shadcn/ui
│   ├── src/services/api.js      # Único punto de comunicación con la API
│   ├── src/context/AuthContext.jsx  # Sesión: usuario, token y menús
│   ├── src/components/          # Layout (menú dinámico), ProtectedRoute, ui/ (shadcn)
│   ├── src/pages/               # Login, Inicio, Usuarios, Asignación, Módulo, 403, 404
│   ├── nginx.conf
│   └── Dockerfile
├── docker-compose.yml
└── .env.example
```


### Modelo de datos

```
Usuarios (Id PK, NombreCompleto, NombreUsuario UNIQUE, ContrasenaHash, Rol CHECK, Activo, FechaCreacion)
   1 ──< UsuarioMenus (UsuarioId FK, MenuId FK, FechaAsignacion)  PK(UsuarioId, MenuId) >── 1
Menus (Id PK, Nombre, Descripcion, Ruta UNIQUE, Activo)
```

- Las entidades de C# se llaman igual que las tablas y columnas (`Usuario.NombreUsuario` ↔ `Usuarios.NombreUsuario`).
- `UsuarioMenus` resuelve la relación **muchos a muchos**; su PK compuesta impide duplicar una asignación.
- `ON DELETE CASCADE` limpia las asignaciones si se elimina un usuario o menú.
- `Rol` (`Administrador`/`Usuario`).
- Restricciones `CHECK` replican en la BD las reglas de negocio: valores de `Rol` y longitud mínima de nombre y usuario.

---

## 4. API REST

Swagger básico para probar la API: **http://localhost:5000/swagger**
1. Ejecutar `POST /api/auth/login` con `{ "nombreUsuario": "admin", "contrasena": "Admin123*" }` y copiar el `token`.
2. Botón **Authorize** → pegar el token.
3. Probar el resto de endpoints.


| Método | Endpoint | Acceso | Propósito | Respuestas |
|---|---|---|---|---|
| POST | `/api/auth/login` | Público | Autenticar y obtener JWT | 200, 400, 401 |
| POST | `/api/users` | Administrador | Crear usuario | 201, 400, 401, 403, 409 |
| GET | `/api/users` | Administrador | Listar usuarios | 200, 401, 403 |
| GET | `/api/menus` | Autenticado | Catálogo de menús activos | 200, 401 |
| PUT | `/api/users/{id}/menus` | Administrador | Reemplazar los menús de un usuario (`{ "idsMenus": [1,2] }`) | 200, 400, 401, 403, 404 |
| GET | `/api/users/{id}/menus` | Administrador o el propio usuario | Menús asignados a un usuario | 200, 401, 403, 404 |
| GET | `/api/modules/{route}` | Usuario con ese menú asignado | Contenido (simulado) del módulo | 200, 401, 403, 404 |
| GET | `/health` | Público | Estado de la API | 200 |

**Ejemplos de JSON :**

```json
// POST /api/auth/login  (entrada)
{ "nombreUsuario": "ana", "contrasena": "Ana12345*" }

// respuesta 200
{
  "token": "eyJhbGciOi...",
  "expiraEn": "2026-10-20T00:30:00Z",
  "usuario": { "id": 2, "nombreCompleto": "Ana Gómez", "nombreUsuario": "ana", "rol": "Usuario", "activo": true, "fechaCreacion": "..." }
}

// POST /api/users  (entrada)
{ "nombreCompleto": "Laura Ruiz", "nombreUsuario": "laura", "contrasena": "Laura1234", "rol": "Usuario" }

// GET /api/menus  (respuesta)
[ { "id": 1, "nombre": "Pólizas", "descripcion": "Consulta y gestión de pólizas de seguros", "ruta": "polizas", "activo": true } ]
```

**Formato de error** :

```json
{ "status": 403, "title": "Acceso denegado", "detail": "No tiene permiso para acceder al módulo 'Siniestros'." }
```

Errores de validación (400):

```json
{
  "status": 400,
  "title": "Datos inválidos",
  "detail": "Uno o más campos no cumplen las validaciones.",
  "errors": {
    "Contrasena": ["La contraseña debe contener al menos una letra y un número."],
    "Rol": ["El rol debe ser 'Administrador' o 'Usuario'."]
  }
}
```


---

## 5. Escenario de validación (paso a paso)

1. Abrir http://localhost:3000 e ingresar como **ana** → solo ve *Pólizas* y *Clientes*.
2. Escribir en la barra la URL http://localhost:3000/modulo/siniestros → pantalla **403**. El botón *"Intentar acceso directo a la API"* muestra la respuesta real: `HTTP 403`.
3. Intentar http://localhost:3000/usuarios → 403 (solo administradores).
4. Cerrar sesión e ingresar como **carlos** → solo ve *Siniestros* y *Reportes*.
5. Ingresar como **admin** → *Usuarios*: crear un usuario nuevo; *Asignación de menús*: asignarle menús.
6. Iniciar sesión con el usuario nuevo y comprobar que ve únicamente lo asignado.
7. **Desde la API (Swagger)**: login como `ana`, *Authorize* con su token y ejecutar `GET /api/users` → **403**; `GET /api/modules/siniestros` → **403**; `GET /api/users/3/menus` → **403**. Sin token → **401**.

---


## 7. Mejoras posibles (fuera del alcance de 3 horas)

- Usuario SQL de mínimo privilegio para la API en lugar de `sa`.
- Refresh tokens y almacenamiento del token en cookie `HttpOnly`.
- Bloqueo de cuenta tras intentos fallidos y *rate limiting* en el login.
- Editar/desactivar usuarios y CRUD de menús.
- Pruebas unitarias (xUnit) de los servicios y pruebas de integración de la API.
- HTTPS con certificado en el proxy.

---

## 9. Uso de herramientas de IA

Ver [HERRAMIENTAS_IA.md](HERRAMIENTAS_IA.md).
