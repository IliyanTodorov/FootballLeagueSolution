namespace FootballLeague.Data.Models
{
    using System;
    using FootballLeague.Data.Common.Models;

    public class Match : BaseModel<int>
    {
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public DateTime PlayedDate { get; set; }

        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
    }
}
