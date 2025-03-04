namespace FootballLeague.Data.Models.DTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using FootballLeague.Services.Mapping;
    
    public class CreateMatchDto : IMapTo<Match>
    {
        [Required]
        public int HomeTeamId { get; set; }

        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        public int HomeTeamScore { get; set; }

        [Required]
        public int AwayTeamScore { get; set; }

        [Required]
        public DateTime PlayedDate { get; set; }
    }
}
