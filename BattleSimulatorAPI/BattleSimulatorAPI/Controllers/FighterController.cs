using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace BattleSimulatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FighterController : ControllerBase
    {
        private readonly IFighterRepository _fighterRepository;

        public FighterController(IFighterRepository fighterRepository)
        {
            _fighterRepository = fighterRepository;
        }

        // Breeze: Get all Fighters
        [HttpGet]
        public IQueryable<Fighter> Get()
        {
            return _fighterRepository.GetAllFighters();
        }

        // Get single Fighter by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Fighter>> GetById(int id)
        {
            var fighter = await _fighterRepository.GetFighterById(id);
            if (fighter == null)
                return NotFound();

            return Ok(fighter);
        }

        // Get Fighters by Element Type
        [HttpGet("element/{elementType}")]
        public async Task<ActionResult<List<Fighter>>> GetByElement(int elementType)
        {
            return Ok(await _fighterRepository.GetFightersByElement(elementType));
        }

        // Get Fighters by Fighter Type
        [HttpGet("type/{fighterType}")]
        public async Task<ActionResult<List<Fighter>>> GetByType(int fighterType)
        {
            return Ok(await _fighterRepository.GetFightersByType(fighterType));
        }
    }
}

