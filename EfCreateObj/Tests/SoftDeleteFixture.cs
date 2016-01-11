using System.Data.Entity;
using System.Linq;
using NUnit.Framework;

namespace EfCreateObj.Tests
{
    [TestFixture]
    public class SoftDeleteFixture
    {
        /// <summary>
        /// Because we don't have "OneToManyCascadeDeleteConvention"
        /// </summary>
        [Test]
        public void SoftDeleteShouldWorkWithoutConceptualNullProblem()
        {
            //Create 2 houses that contains a room in it
            int id1, id2;
            using (var db = new Context())
            {
                var h1 = new House
                    {
                        Rooms = new[] {new Room()}
                    };

                var h2 = new House
                    {
                        Rooms = new[] {new Room()}
                    };

                db.Houses.Add(h1);
                db.Houses.Add(h2);
                db.SaveChanges();

                id1 = h1.Id;
                id2 = h2.Id;
            }

            //Load house1 and SoftDelete it. This should work
            using (var db = new Context())
            {
                var h1 = db
                    .Houses
                    .Single(x => x.Id == id1);

                db.Houses.Remove(h1);
                db.SaveChanges();
            }

            using (var db = new Context())
            {
                //Any calls that loads the child collection to the identity table will throw a
                //"ObjectContext_CommitWithConceptualNull" exception saying:
                //  The operation failed: The relationship could not be changed because one or more of the foreign-key properties is non-nullable. When a change is made to a relationship, the related foreign-key property is set to a null value. If the foreign-key does not support null values, a new relationship must be defined, the foreign-key property must be assigned another non-null value, or the unrelated object must be deleted.

                var h2 = db
                    .Houses
                    .Include(x => x.Rooms) //**This line is the problem**
                    .Single(x => x.Id == id2);

                //Doing something like this will fail too...
                //h2.Rooms.ToList();
                
                db.Houses.Remove(h2);
                db.SaveChanges(); //This line will fail
            }
        }
    }
}