namespace FootballLeague.API.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    
    using FootballLeague.Data.Models;
    using FootballLeague.Data.Models.DTOs;
    using FootballLeague.Services.Interfaces;

    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return Ok(teams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Team>> GetTeam(int id)
        {
            var team = await _teamService.GetTeamByIdAsync(id);
            if (team == null)
                return NotFound();

            return Ok(team);
        }

        [HttpPost]
        public async Task<ActionResult<Team>> CreateTeam(CreateTeamDto team)
        {
            var createdTeam = await _teamService.CreateTeamAsync(team);
            return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTeam(int id, UpdateTeamDto team)
        {
            var result = await _teamService.UpdateTeamAsync(id, team);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTeam(int id)
        {
            var result = await _teamService.DeleteTeamAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
