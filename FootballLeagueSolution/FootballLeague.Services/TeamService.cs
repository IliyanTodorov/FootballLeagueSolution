namespace FootballLeague.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    using FootballLeague.Services.Interfaces;
    using FootballLeague.Data.Models;
    using FootballLeague.Data.Common.Repositories;
    using FootballLeague.Data.Models.DTOs;
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

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _teamRepository.All().ToListAsync();
        }

        public async Task<Team> GetTeamByIdAsync(int id)
        {
            return await _teamRepository.All()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Team> CreateTeamAsync(CreateTeamDto createTeamDto)
        {
            var team = _mapper.Map<Team>(createTeamDto);

            await _teamRepository.AddAsync(team);
            await _teamRepository.SaveChangesAsync();
            return team;
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