using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using EventHub._03.Data.Entities;

namespace EventHub._03.Data
{
    public class EventHubContext : DbContext
    {
        public EventHubContext() : base("name=EventHubConnection")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<TipoEvento> TiposEvento { get; set; }
        public DbSet<Tarea> Tareas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
