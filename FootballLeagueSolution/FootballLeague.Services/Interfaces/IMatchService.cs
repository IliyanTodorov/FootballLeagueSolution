namespace FootballLeague.Services.Interfaces
{
    using FootballLeague.Data.Models.DTOs;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    
    public interface IMatchService
    {
        Task<IEnumerable<MatchResponseDto>> GetAllMatchesAsync();
        Task<MatchResponseDto> GetMatchByIdAsync(int id);
        Task<MatchResponseDto> CreateMatchAsync(CreateMatchDto createMatchDto);
        Task<bool> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto);
        Task<bool> DeleteMatchAsync(int id);
    }
}
