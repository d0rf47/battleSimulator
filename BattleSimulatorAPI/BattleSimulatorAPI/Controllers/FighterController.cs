using BattleSimulatorAPI.Repositories.Models.DTO;
using BattleSimulatorAPI.Repositories.Models.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace BattleSimulatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FighterController : ControllerBase
    {
        private readonly ICrudRepository<Fighter> _fighterRepository;

        public FighterController(ICrudRepository<Fighter> fighterRepository)
        {
            _fighterRepository = fighterRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _fighterRepository.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var fighter = await _fighterRepository.GetByIdAsync(id);
            if (fighter == null) return NotFound();
            return Ok(fighter);
        }

        [HttpPost]
        public async Task<IActionResult> AddFighter([FromBody] Fighter fighter)
        {
            await _fighterRepository.AddAsync(fighter);
            return CreatedAtAction(nameof(GetById), new { id = fighter.Id }, fighter);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _fighterRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}

