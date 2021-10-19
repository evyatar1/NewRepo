using System;
using System.Collections.Generic;

namespace DBMoviesManager
{
    public partial class Movie
    {
        public Movie()
        {
            ActorMovies = new HashSet<ActorMovie>();
        }
        public int MovieSerial { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int? DirectorId { get; set; }
        public string Country { get; set; }
        public decimal ImdbScore { get; set; }

        public virtual Director Director { get; set; }
        public virtual Oscar Oscar { get; set; }
        public virtual ICollection<ActorMovie> ActorMovies { get; set; }
        public override string ToString()
        {
            return $"{Title}, {Year}[Serial:{MovieSerial}]";
        }
    }
}
