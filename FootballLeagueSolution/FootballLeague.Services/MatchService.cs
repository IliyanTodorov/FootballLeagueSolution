namespace FootballLeague.Services
{
    using System;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper;
    
    using FootballLeague.Data.Common.Repositories;
    using FootballLeague.Data.Models;
    using FootballLeague.Data.Models.DTOs;
    using FootballLeague.Services.Interfaces;

    public class MatchService : IMatchService
    {
        private readonly IRepository<Match> _matchRepository;
        private readonly IRepository<Team> _teamRepository;
        private readonly IMapper _mapper;

        public MatchService(IRepository<Match> matchRepository, IRepository<Team> teamRepository, IMapper mapper)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MatchResponseDto>> GetAllMatchesAsync()
        {
            var matches = await _matchRepository.All()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MatchResponseDto>>(matches);
        }

        public async Task<MatchResponseDto> GetMatchByIdAsync(int id)
        {
            var match = await _matchRepository.All()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == id);

            return _mapper.Map<MatchResponseDto>(match);
        }

        public async Task<MatchResponseDto> CreateMatchAsync(CreateMatchDto createMatchDto)
        {
            if (createMatchDto.HomeTeamId == createMatchDto.AwayTeamId)
            {
                throw new ArgumentException("Home team and away team cannot be the same.");
            }

            if (createMatchDto.HomeTeamScore < 0 || createMatchDto.AwayTeamScore < 0)
            {
                throw new ArgumentException("Scores must be non-negative.");
            }

            if (createMatchDto.PlayedDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Match date cannot be in the future.");
            }

            var homeTeamExists = await _teamRepository.All()
                                   .AnyAsync(t => t.Id == createMatchDto.HomeTeamId);
            var awayTeamExists = await _teamRepository.All()
                                   .AnyAsync(t => t.Id == createMatchDto.AwayTeamId);

            if (!homeTeamExists || !awayTeamExists)
            {
                throw new ArgumentException("One or both of the provided team IDs do not exist or have been deleted.");
            }

            var match = _mapper.Map<Match>(createMatchDto);

            await _matchRepository.AddAsync(match);
            await _matchRepository.SaveChangesAsync();

            var createdMatch = await _matchRepository.All()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == match.Id);

            return _mapper.Map<MatchResponseDto>(createdMatch);
        }

        public async Task<bool> UpdateMatchAsync(int id, UpdateMatchDto updateMatchDto)
        {
            if (updateMatchDto.HomeTeamScore < 0 || updateMatchDto.AwayTeamScore < 0)
            {
                throw new ArgumentException("Scores must be non-negative.");
            }

            if (updateMatchDto.PlayedDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Match date cannot be in the future.");
            }

            var match = await _matchRepository.All()
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
            {
                return false;
            }

            _mapper.Map(updateMatchDto, match);
            _matchRepository.Update(match);

            return await _matchRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteMatchAsync(int id)
        {
            var match = await _matchRepository.All().FirstOrDefaultAsync(m => m.Id == id);
            if (match == null)
            {
                return false;
            }

            _matchRepository.Delete(match);
            return await _matchRepository.SaveChangesAsync() > 0;
        }
    }
}
