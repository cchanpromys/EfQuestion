using System.Data.Entity;

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
    }
}
