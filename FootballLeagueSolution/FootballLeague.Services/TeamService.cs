namespace FootballLeague.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using FootballLeague.Services.Interfaces;
    using FootballLeague.Data.Models;
    using FootballLeague.Data.Common.Repositories;
    using FootballLeague.Data.Models.DTOs;
    using FootballLeague.Services.Helpers;
    using AutoMapper;

    public class TeamService : ITeamService
    {
        private readonly IDeletableEntityRepository<Team> _teamRepository;
        private readonly IMapper _mapper;

        public TeamService(IDeletableEntityRepository<Team> teamRepository, IMapper mapper)
        {
            _teamRepository = teamRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeamListDto>> GetAllTeamsAsync()
        {
            var teams = await _teamRepository.All()
                .Include(t => t.HomeMatches)
                .Include(t => t.AwayMatches)
                .ToListAsync();

            var teamListDtos = _mapper.Map<IEnumerable<TeamListDto>>(teams);
            return teamListDtos;
        }

        public async Task<TeamResponseDto> GetTeamByIdAsync(int id)
        {
            var team = await _teamRepository.All()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
            {
                return null;
            }

            var teamDto = _mapper.Map<TeamResponseDto>(team);
            return teamDto;
        }

        public async Task<TeamStatsDto> GetTeamStatsAsync(int id)
        {
            var team = await _teamRepository.All()
                                .Include(t => t.HomeMatches)
                                .Include(t => t.AwayMatches)
                                .FirstOrDefaultAsync(t => t.Id == id);

            if (team == null)
            {
                throw new ArgumentException("Team with this id doesn't exists.");
            }

            var (wins, draws, losses, points) = TeamStatsCalculator.Calculate(team.HomeMatches, team.AwayMatches);

            return new TeamStatsDto
            {
                TeamId = team.Id,
                TeamName = team.Name,
                Wins = wins,
                Draws = draws,
                Losses = losses,
                Points = points
            };
        }

        public async Task<IEnumerable<TeamStatsDto>> GetTeamRankingsAsync()
        {
            var teams = await _teamRepository.All()
                              .Include(t => t.HomeMatches)
                              .Include(t => t.AwayMatches)
                              .ToListAsync();

            var rankings = teams.Select(team =>
            {
                var (wins, draws, losses, points) = TeamStatsCalculator.Calculate(team.HomeMatches, team.AwayMatches);

                return new TeamStatsDto
                {
                    TeamId = team.Id,
                    TeamName = team.Name,
                    Wins = wins,
                    Draws = draws,
                    Losses = losses,
                    Points = points
                };
            })
            .OrderByDescending(r => r.Points)
            .ThenBy(r => r.TeamName)
            .ToList();

            return rankings;
        }

        public async Task<TeamResponseDto> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            var team = _mapper.Map<Team>(createTeamDto);

            await _teamRepository.AddAsync(team);
            await _teamRepository.SaveChangesAsync();

            var teamResponse = _mapper.Map<TeamResponseDto>(team);

            return teamResponse;
        }

        public async Task<bool> UpdateTeamAsync(int id, UpdateTeamDto updateTeamDto)
        {
            var team = await _teamRepository.All().FirstOrDefaultAsync(t => t.Id == id);
            if (team == null)
            {
                return false;
            }

            _mapper.Map(updateTeamDto, team);

            _teamRepository.Update(team);
            return await _teamRepository.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteTeamAsync(int id)
        {
            var team = await _teamRepository.All().FirstOrDefaultAsync(t => t.Id == id);
            if (team == null)
            {
                return false;
            }
            
            _teamRepository.Delete(team);
            return await _teamRepository.SaveChangesAsync() > 0;
        }
    }
}