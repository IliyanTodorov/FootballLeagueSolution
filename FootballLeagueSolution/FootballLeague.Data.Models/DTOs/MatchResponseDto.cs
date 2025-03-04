namespace FootballLeague.Data.Models.DTOs
{
    using System;
    using AutoMapper;
    using FootballLeague.Services.Mapping;
    
    public class MatchResponseDto : IMapFrom<Match>, IHaveCustomMappings
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public DateTime PlayedDate { get; set; }
        public string HomeTeamName { get; set; }
        public string AwayTeamName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Match, MatchResponseDto>()
                .ForMember(dto => dto.HomeTeamName,
                           opt => opt.MapFrom(src => src.HomeTeam.Name))
                .ForMember(dto => dto.AwayTeamName,
                           opt => opt.MapFrom(src => src.AwayTeam.Name));
        }
    }
}
