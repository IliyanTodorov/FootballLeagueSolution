namespace FootballLeague.Data.Models.DTOs
{
    public class TeamStatsDto
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int Points { get; set; }
    }
}