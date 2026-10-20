/* =====================================================================
   PRUEBA TÉCNICA LOCKTON - Script de creación de base de datos
   Motor: SQL Server 2022
   Es idempotente: se puede ejecutar varias veces sin duplicar datos.
   Convención: nombres de tablas y columnas en español, sin tildes ni ñ
   (ej: ContrasenaHash) para evitar problemas de codificación.
   ===================================================================== */

IF DB_ID('LocktonDb') IS NULL
    CREATE DATABASE LocktonDb;
GO

USE LocktonDb;
GO

/* ---------------------------------------------------------------------
   1. USUARIOS
   - ContrasenaHash: hash + salt generado con PasswordHasher de ASP.NET
     Identity (PBKDF2-HMACSHA512, 100.000 iteraciones). Nunca texto plano.
   - Rol: 'Administrador' gestiona usuarios y asignaciones;
          'Usuario' solo consulta sus propios menús.
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Usuarios') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios (
        Id              INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
        NombreCompleto  NVARCHAR(100)     NOT NULL,
        NombreUsuario   NVARCHAR(50)      NOT NULL CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE,
        ContrasenaHash  NVARCHAR(500)     NOT NULL,
        Rol             NVARCHAR(20)      NOT NULL CONSTRAINT DF_Usuarios_Rol DEFAULT ('Usuario'),
        Activo          BIT               NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT (1),
        FechaCreacion   DATETIME2         NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Administrador', 'Usuario')),
        CONSTRAINT CK_Usuarios_NombreCompleto CHECK (LEN(NombreCompleto) >= 3),
        CONSTRAINT CK_Usuarios_NombreUsuario CHECK (LEN(NombreUsuario) >= 3)
    );
END
GO

/* ---------------------------------------------------------------------
   2. MENÚS
   - Ruta: identificador funcional / ruta del módulo en el frontend.
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.Menus') IS NULL
BEGIN
    CREATE TABLE dbo.Menus (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Menus PRIMARY KEY,
        Nombre       NVARCHAR(100)     NOT NULL,
        Descripcion  NVARCHAR(250)     NULL,
        Ruta         NVARCHAR(100)     NOT NULL CONSTRAINT UQ_Menus_Ruta UNIQUE,
        Activo       BIT               NOT NULL CONSTRAINT DF_Menus_Activo DEFAULT (1)
    );
END
GO

/* ---------------------------------------------------------------------
   3. ASIGNACIONES USUARIO-MENÚ (relación muchos a muchos)
   - La PK compuesta impide asignar el mismo menú dos veces.
   - ON DELETE CASCADE: si se borra un usuario o un menú, se borran
     sus asignaciones.
   --------------------------------------------------------------------- */
IF OBJECT_ID('dbo.UsuarioMenus') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioMenus (
        UsuarioId        INT       NOT NULL,
        MenuId           INT       NOT NULL,
        FechaAsignacion  DATETIME2 NOT NULL CONSTRAINT DF_UsuarioMenus_FechaAsignacion DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT PK_UsuarioMenus PRIMARY KEY (UsuarioId, MenuId),
        CONSTRAINT FK_UsuarioMenus_Usuarios FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE,
        CONSTRAINT FK_UsuarioMenus_Menus    FOREIGN KEY (MenuId)    REFERENCES dbo.Menus(Id)    ON DELETE CASCADE
    );
END
GO

/* =====================================================================
   DATOS INICIALES (solo para pruebas, NO son credenciales reales)
   admin  / Admin123*   -> Administrador, todos los menús
   ana    / Ana12345*   -> Usuario, Pólizas y Clientes
   carlos / Carlos123*  -> Usuario, Siniestros y Reportes
   ===================================================================== */

IF NOT EXISTS (SELECT 1 FROM dbo.Menus)
BEGIN
    INSERT INTO dbo.Menus (Nombre, Descripcion, Ruta) VALUES
        (N'Pólizas',    N'Consulta y gestión de pólizas de seguros', 'polizas'),
        (N'Clientes',   N'Información de clientes asegurados',       'clientes'),
        (N'Siniestros', N'Registro y seguimiento de siniestros',     'siniestros'),
        (N'Reportes',   N'Reportes e indicadores de gestión',        'reportes');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios)
BEGIN
    INSERT INTO dbo.Usuarios (NombreCompleto, NombreUsuario, ContrasenaHash, Rol) VALUES
        (N'Administrador del sistema', 'admin',
         'AQAAAAIAAYagAAAAEJNRyT/zc9Rn5TxSvtsWYsTXSwUb7MSVeMk7Gp3QNRErJUvUNuz50mTOrcT/eHer4A==', 'Administrador'),
        (N'Ana Gómez', 'ana',
         'AQAAAAIAAYagAAAAELV8vo4ZD+kDcAI9Yh7wxLxoQvx84hSFp+LHxa86MAxcGgR9G3wIkzILhHkOTvIw0Q==', 'Usuario'),
        (N'Carlos Pérez', 'carlos',
         'AQAAAAIAAYagAAAAEGjs3hxkQMrNIMm2dH7/Lc3Q0sdjJzd//8JLYAYfoDOrIozioJaROgLPT/l/IHkwaw==', 'Usuario');

    -- admin: todos los menús
    INSERT INTO dbo.UsuarioMenus (UsuarioId, MenuId)
    SELECT u.Id, m.Id FROM dbo.Usuarios u CROSS JOIN dbo.Menus m
    WHERE u.NombreUsuario = 'admin';

    -- ana: Pólizas y Clientes
    INSERT INTO dbo.UsuarioMenus (UsuarioId, MenuId)
    SELECT u.Id, m.Id FROM dbo.Usuarios u JOIN dbo.Menus m ON m.Ruta IN ('polizas', 'clientes')
    WHERE u.NombreUsuario = 'ana';

    -- carlos: Siniestros y Reportes
    INSERT INTO dbo.UsuarioMenus (UsuarioId, MenuId)
    SELECT u.Id, m.Id FROM dbo.Usuarios u JOIN dbo.Menus m ON m.Ruta IN ('siniestros', 'reportes')
    WHERE u.NombreUsuario = 'carlos';
END
GO

PRINT 'Base de datos LocktonDb lista.';
GO
