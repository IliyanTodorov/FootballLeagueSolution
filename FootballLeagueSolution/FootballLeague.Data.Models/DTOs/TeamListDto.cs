using AutoMapper;
using FootballLeague.Services.Mapping;
using System.Linq;

namespace FootballLeague.Data.Models.DTOs
{
    public class TeamListDto : IMapTo<Team>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int HomeMatchesCount { get; set; }
        public int AwayMatchesCount { get; set; }
        public int TotalPoints { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Team, TeamListDto>()
                .ForMember(dto => dto.HomeMatchesCount, opt => opt.MapFrom(src => src.HomeMatches.Count))
                .ForMember(dto => dto.AwayMatchesCount, opt => opt.MapFrom(src => src.AwayMatches.Count))
                .ForMember(dto => dto.TotalPoints, opt => opt.MapFrom(src =>
                    src.HomeMatches.Sum(m => m.HomeTeamScore > m.AwayTeamScore ? 3 : m.HomeTeamScore == m.AwayTeamScore ? 1 : 0) +
                    src.AwayMatches.Sum(m => m.AwayTeamScore > m.HomeTeamScore ? 3 : m.AwayTeamScore == m.HomeTeamScore ? 1 : 0)
                ));
        }
    }
}