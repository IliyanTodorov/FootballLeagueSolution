namespace FootballLeague.Data.Models
{
    using System.Collections.Generic;
    using FootballLeague.Data.Common.Models;

    public class Team : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}