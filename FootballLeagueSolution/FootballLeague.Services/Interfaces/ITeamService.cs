namespace FootballLeague.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FootballLeague.Data.Models;
    using FootballLeague.Data.Models.DTOs;

    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team> GetTeamByIdAsync(int id);
        Task<Team> CreateTeamAsync(CreateTeamDto team);
        Task<bool> UpdateTeamAsync(int id, UpdateTeamDto team);
        Task<bool> DeleteTeamAsync(int id);
    }
}
