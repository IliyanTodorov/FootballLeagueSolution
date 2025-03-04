namespace FootballLeague.API.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;

    using FootballLeague.Data.Models.DTOs;
    using FootballLeague.Services.Interfaces;
    
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MatchResponseDto>>> GetMatches()
        {
            var matches = await _matchService.GetAllMatchesAsync();
            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchResponseDto>> GetMatch(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null)
            {
                return NotFound();
            }
            return Ok(match);
        }

        [HttpPost]
        public async Task<ActionResult<MatchResponseDto>> CreateMatch(CreateMatchDto createMatchDto)
        {
            var match = await _matchService.CreateMatchAsync(createMatchDto);
            return CreatedAtAction(nameof(GetMatch), new { id = match.Id }, match);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMatch(int id, UpdateMatchDto updateMatchDto)
        {
            var result = await _matchService.UpdateMatchAsync(id, updateMatchDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(int id)
        {
            var result = await _matchService.DeleteMatchAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
