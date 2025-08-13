using Microsoft.EntityFrameworkCore;
using EcoSenseAPI.Models;

namespace EcoSenseAPI.Data
{
    public class EcoSenseContext : DbContext
    {
        public EcoSenseContext(DbContextOptions<EcoSenseContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<DatoSalud> DatosSalud { get; set; }
        public DbSet<Dispositivo> Dispositivos { get; set; }
        public DbSet<ConfiguracionUmbral> ConfiguracionUmbrales { get; set; }
        public DbSet<Lectura> Lecturas { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<ReporteExportado> ReportesExportados { get; set; }
        public DbSet<MensajeContacto> MensajesContacto { get; set; }
        public DbSet<FaqComentario> FaqComentarios { get; set; }
        public DbSet<CotizacionSolicitud> Cotizaciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones uno a uno
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.DatoSalud)
                .WithOne(ds => ds.Usuario)
                .HasForeignKey<DatoSalud>(ds => ds.IdUsuario);

            base.OnModelCreating(modelBuilder);
        }
    }
}
