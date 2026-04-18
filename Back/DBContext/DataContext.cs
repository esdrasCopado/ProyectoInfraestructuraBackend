using Microsoft.EntityFrameworkCore;
using SolicitudServidores.Back.Models;
using SolicitudServidores.Models;

namespace SolicitudServidores.DBContext
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Solicitud> Solicitudes => Set<Solicitud>();
        public DbSet<Servidor> Servidores => Set<Servidor>();
        public DbSet<VPN> VPNs => Set<VPN>();
        public DbSet<WAF> WAFs => Set<WAF>();
        public DbSet<EvidenciasPruebas> EvidenciasPruebas => Set<EvidenciasPruebas>();
        public DbSet<Subdominio> Subdominios => Set<Subdominio>();
        public DbSet<ServerRequest> SolicitudServidores => Set<ServerRequest>();
        public DbSet<ServerEntry> ServidoresEntrada => Set<ServerEntry>();
        public DbSet<PermisoCategoria> PermisoCategorias => Set<PermisoCategoria>();
        public DbSet<Carta> Cartas => Set<Carta>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.PermisoCategorias)
                .WithOne(pc => pc.Usuario)
                .HasForeignKey(pc => pc.IdUsuario);

            modelBuilder.Entity<Solicitud>()
                .HasMany(s => s.Servidores)
                .WithOne(s => s.Solicitud)
                .HasForeignKey(s => s.Id_Solicitud)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Servidor>()
                .HasMany(s => s.VPNs)
                .WithOne(v => v.Servidor)
                .HasForeignKey(v => v.Id_servidor)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<VPN>()
                .HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.Id_usuario_Responsable)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<VPN>()
                .Property(v => v.Id_usuario_Responsable)
                .HasColumnName("id_usuario");

            modelBuilder.Entity<Servidor>()
                .HasMany(s => s.Subdominios)
                .WithOne(sub => sub.Servidor)
                .HasForeignKey(sub => sub.Id_Servidor)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Servidor>()
                .HasMany(s => s.WAFs)
                .WithOne(w => w.Servidor)
                .HasForeignKey(w => w.Id_Servidor)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Servidor>()
                .HasMany(s => s.EvidenciasPruebas)
                .WithOne(e => e.Servidor)
                .HasForeignKey(e => e.Id_Servidor)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Rol)
                .HasDefaultValue("Dependencia / Cliente");

            modelBuilder.Entity<Solicitud>()
                .Property(s => s.Estado)
                .HasDefaultValue("Pendiente");

            modelBuilder.Entity<Servidor>()
                .Property(s => s.TipoUso)
                .HasDefaultValue("Interno");

            modelBuilder.Entity<Servidor>()
                .Property(s => s.PlantillaRecursos)
                .HasDefaultValue("General");

            var unaccentLowerMethod = typeof(DataContext).GetMethod(nameof(UnaccentLower), new[] { typeof(string) });
            if (unaccentLowerMethod == null)
                throw new InvalidOperationException("No se pudo encontrar el método UnaccentLower.");

            modelBuilder.HasDbFunction(unaccentLowerMethod)
                .HasName("unaccent_lower")
                .HasSchema("public");
        }

        public static string UnaccentLower(string input) => throw new NotImplementedException();
    }
}
