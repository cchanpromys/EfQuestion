using System.Collections.ObjectModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using NUnit.Framework;

namespace EfCreateObj
{
    [TestFixture]
    public class Test
    {
        public void CanAdd()
        {
            int houseId;

            using (var db = new Context())
            {
                //Create a parent first
                var house = new House();
                var chair = new Chair {Width = 100};
                db.Houses.Add(house);
                db.Chairs.Add(chair);

                db.SaveChanges();

                houseId = house.Id;
            }

            using (var db = new Context())
            {
                var objCx = ((IObjectContextAdapter) db).ObjectContext;

                var room = objCx.CreateObject<Room>();
                db.Rooms.Add(room);

                room.HouseId = houseId;

                Assert.That(room.House, Is.Null);

                objCx.DetectChanges();

                //Should auto wire the parent
                Assert.That(room.House, Is.Not.Null);

                room.Desks = new Collection<Desk>
                    {
                        new Desk
                            {
                                Chair = db.Chairs.First()
                            }
                    };

                db.SaveChanges();
            }
        }
    }
}