namespace FootballLeague.Data.Models.DTOs
{
    using System.ComponentModel.DataAnnotations;
    using FootballLeague.Services.Mapping;

    public class UpdateTeamDto : IMapTo<Team>
    {
        [Required(ErrorMessage = "Team name is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Team name must be between 5 and 100 characters.")]
        public string Name { get; set; }
    }
}
