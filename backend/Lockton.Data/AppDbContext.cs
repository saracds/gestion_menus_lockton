using Lockton.Data.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Lockton.Data;

/// <summary>
/// Puente entre C# y SQL Server (Entity Framework Core).
/// Las tablas se crean con database/init.sql.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<UsuarioMenu> UsuarioMenus => Set<UsuarioMenu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tabla dbo.Usuarios
        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.ToTable("Usuarios");
            entidad.HasKey(u => u.Id);
            entidad.Property(u => u.NombreCompleto).HasMaxLength(100).IsRequired();
            entidad.Property(u => u.NombreUsuario).HasMaxLength(50).IsRequired();
            entidad.HasIndex(u => u.NombreUsuario).IsUnique();
            entidad.Property(u => u.ContrasenaHash).HasMaxLength(500).IsRequired();
            entidad.Property(u => u.Rol).HasMaxLength(20).IsRequired();
        });

        // Tabla dbo.Menus
        modelBuilder.Entity<Menu>(entidad =>
        {
            entidad.ToTable("Menus");
            entidad.HasKey(m => m.Id);
            entidad.Property(m => m.Nombre).HasMaxLength(100).IsRequired();
            entidad.Property(m => m.Descripcion).HasMaxLength(250);
            entidad.Property(m => m.Ruta).HasMaxLength(100).IsRequired();
            entidad.HasIndex(m => m.Ruta).IsUnique();
        });

        // Tabla dbo.UsuarioMenus (relación muchos a muchos)
        modelBuilder.Entity<UsuarioMenu>(entidad =>
        {
            entidad.ToTable("UsuarioMenus");
            entidad.HasKey(um => new { um.UsuarioId, um.MenuId }); // PK compuesta

            entidad.HasOne(um => um.Usuario)
                   .WithMany(u => u.UsuarioMenus)
                   .HasForeignKey(um => um.UsuarioId);

            entidad.HasOne(um => um.Menu)
                   .WithMany(m => m.UsuarioMenus)
                   .HasForeignKey(um => um.MenuId);
        });
    }
}
