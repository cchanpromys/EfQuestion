using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace EfCreateObj
{
    public class Context : DbContext
    {
        public DbSet<House> Houses { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Desk> Desks { get; set; }
        public DbSet<Chair> Chairs { get; set; }

        public Context()
            : base("MyConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder m)
        {
            m.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }
    }

    public class EntityFrameworkConfiguration : DbConfiguration
    {
        public EntityFrameworkConfiguration()
        {
            AddInterceptor(new SoftDeleteInterceptor());
        }
    }

    internal sealed class Configuration : DbMigrationsConfiguration<Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations";
        }

        protected override void Seed(Context context)
        {
        }
    }
}