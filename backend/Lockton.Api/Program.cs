using System.Text;
using Lockton.Api.Extensiones;
using Lockton.Api.Middleware;
using Lockton.Data;
using Lockton.Data.Entidades;
using Lockton.Data.Repositorios;
using Lockton.Services.Configuracion;
using Lockton.Services.Interfaces;
using Lockton.Services.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------
// 1. Configuración (appsettings.json + variables de entorno)
// ---------------------------------------------------------------
var cadenaConexion = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Falta ConnectionStrings__Default.");

var configuracionJwt = builder.Configuration.GetSection("Jwt").Get<ConfiguracionJwt>() ?? new ConfiguracionJwt();
if (string.IsNullOrWhiteSpace(configuracionJwt.Clave) || configuracionJwt.Clave.Length < 32)
    throw new InvalidOperationException("Jwt__Clave debe tener al menos 32 caracteres.");

builder.Services.Configure<ConfiguracionJwt>(builder.Configuration.GetSection("Jwt"));

// ---------------------------------------------------------------
// 2. Inyección de dependencias: Api → Services → Data
// ---------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlServer(cadenaConexion, sql => sql.EnableRetryOnFailure()));

// Capa de datos
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IMenuRepositorio, MenuRepositorio>();

// Capa de servicios
builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddScoped<ITokenServicio, TokenServicio>();
builder.Services.AddScoped<IAutenticacionServicio, AutenticacionServicio>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicio>();
builder.Services.AddScoped<IMenuServicio, MenuServicio>();

// Controladores + validaciones y mensajes de error en español
builder.Services.AgregarControladoresConValidacionEnEspanol();

// ---------------------------------------------------------------
// 3. Autenticación JWT
// ---------------------------------------------------------------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.MapInboundClaims = false; 
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = configuracionJwt.Emisor,
            ValidateAudience = true,
            ValidAudience = configuracionJwt.Audiencia,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuracionJwt.Clave)),
            NameClaimType = TokenServicio.ClaimNombreUsuario,
            RoleClaimType = TokenServicio.ClaimRol
        };

        opciones.Events = new JwtBearerEvents
        {
            // Sin token, o token inválido/expirado
            OnChallenge = async contexto =>
            {
                contexto.HandleResponse();
                contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await contexto.Response.WriteAsJsonAsync(new
                {
                    status = 401,
                    title = "No autenticado",
                    detail = "Debe iniciar sesión con un token válido."
                });
            },
            // Token válido pero sin el rol requerido
            OnForbidden = async contexto =>
            {
                contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
                await contexto.Response.WriteAsJsonAsync(new
                {
                    status = 403,
                    title = "Acceso denegado",
                    detail = "No tiene permisos para realizar esta operación."
                });
            }
        };
    });

builder.Services.AddAuthorization();

// ---------------------------------------------------------------
// 4. Swagger
// ---------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Lockton API", Version = "v1" });

    var esquemaJwt = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegue el token obtenido en POST /api/auth/login",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };
    c.AddSecurityDefinition("Bearer", esquemaJwt);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { esquemaJwt, Array.Empty<string>() } });
});

// ---------------------------------------------------------------
// 5. Pipeline HTTP
// ---------------------------------------------------------------
var app = builder.Build();

app.UseMiddleware<ManejoErroresMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { estado = "ok" })).AllowAnonymous();

app.Run();
