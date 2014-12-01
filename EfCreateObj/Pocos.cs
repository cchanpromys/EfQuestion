using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfCreateObj
{
    public class House
    {
        public int Id { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Desk> Desks { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }
        public int HouseId { get; set; }

        [ForeignKey("HouseId")]
        public virtual House House { get; set; }
        public virtual ICollection<Desk> Desks { get; set; }
    }

    public class Desk
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public int? HouseId { get; set; }

        public int ChairId { get; set; }

        [ForeignKey("ChairId")]
        public virtual Chair Chair { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

        [ForeignKey("HouseId")]
        public virtual House House { get; set; }
    }

    public class Chair
    {
        public int Id { get; set; }
        public decimal Width { get; set; }
        public virtual ICollection<Desk> Desks { get; set; }
    }
}
