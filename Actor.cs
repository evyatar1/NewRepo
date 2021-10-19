using System;
using System.Collections.Generic;

namespace DBMoviesManager
{
    public enum Gender { Male = 0, Female = 1 };
    public partial class Actor
    {
        public Actor()
        {
            ActorMovies = new HashSet<ActorMovie>();
            OscarsBestActor = new HashSet<Oscar>();
            OscarsBestActress = new HashSet<Oscar>();
        }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int YearBorn { get; set; }
        public int Gender { get; set; }

        public virtual ICollection<ActorMovie> ActorMovies { get; set; }
        public virtual ICollection<Oscar> OscarsBestActor { get; set; }
        public virtual ICollection<Oscar> OscarsBestActress { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}, Id:{Id}";
        }
    }
}
