namespace FootballLeague.Domain
{
    using System.Collections.Generic;

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Match> Matches { get; set; }
    }
}
