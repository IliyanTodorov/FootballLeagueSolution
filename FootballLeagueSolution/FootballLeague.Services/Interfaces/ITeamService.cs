namespace FootballLeague.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FootballLeague.Data.Models;
    using FootballLeague.Data.Models.DTOs;

    public interface ITeamService
    {
        Task<IEnumerable<TeamListDto>> GetAllTeamsAsync();
        Task<TeamResponseDto> GetTeamByIdAsync(int id);
        Task<TeamStatsDto> GetTeamStatsAsync(int id);
        Task<IEnumerable<TeamStatsDto>> GetTeamRankingsAsync();
        Task<TeamResponseDto> CreateTeamAsync(CreateTeamDto team);
        Task<bool> UpdateTeamAsync(int id, UpdateTeamDto team);
        Task<bool> DeleteTeamAsync(int id);
    }
}
