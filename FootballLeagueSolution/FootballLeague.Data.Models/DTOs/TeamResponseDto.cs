namespace FootballLeague.Data.Models.DTOs
{
    using FootballLeague.Services.Mapping;

    public class TeamResponseDto : IMapFrom<Team>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
