namespace FootballLeague.Data.Models.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FootballLeague.Services.Mapping;
    
    public class UpdateMatchDto : IMapTo<Match>
    {
        [Required]
        public int HomeTeamScore { get; set; }

        [Required]
        public int AwayTeamScore { get; set; }

        [Required]
        public DateTime PlayedDate { get; set; }
    }
}
