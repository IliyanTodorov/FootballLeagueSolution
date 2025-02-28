namespace FootballLeague.Domain
{
    public class Match
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }

        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }

        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
    }
}
