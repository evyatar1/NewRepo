using System;
using System.Collections.Generic;

namespace DBMoviesManager
{
    public partial class Oscar
    {
        public int Year { get; set; }
        public int? BestActorId { get; set; }
        public int? BestActressId { get; set; }
        public int? BestDirectorId { get; set; }
        public int MovieSerial { get; set; }
        public virtual Actor BestActor { get; set; }
        public virtual Actor BestActress { get; set; }
        public virtual Director BestDirector { get; set; }
        public virtual Movie MovieSerialNavigation { get; set; }
        public override string ToString()
        {
            return $"{Year}";
        }
    }
}
