namespace FootballLeague.Services.Helpers
{
    using System.Linq;
    using System.Collections.Generic;
    using FootballLeague.Data.Models;

    public static class TeamStatsCalculator
    {
        public static (int wins, int draws, int losses, int points) Calculate(IEnumerable<Match> homeMatches, IEnumerable<Match> awayMatches)
        {
            int homeWins = homeMatches.Count(m => m.HomeTeamScore > m.AwayTeamScore);
            int homeDraws = homeMatches.Count(m => m.HomeTeamScore == m.AwayTeamScore);
            int homeLosses = homeMatches.Count(m => m.HomeTeamScore < m.AwayTeamScore);

            int awayWins = awayMatches.Count(m => m.AwayTeamScore > m.HomeTeamScore);
            int awayDraws = awayMatches.Count(m => m.AwayTeamScore == m.HomeTeamScore);
            int awayLosses = awayMatches.Count(m => m.AwayTeamScore < m.HomeTeamScore);

            int wins = homeWins + awayWins;
            int draws = homeDraws + awayDraws;
            int losses = homeLosses + awayLosses;
            int points = wins * 3 + draws * 1;

            return (wins, draws, losses, points);
        }
    }
}