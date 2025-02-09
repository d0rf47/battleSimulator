using BattleSimulatorAPI.Repositories.Models.DTO;
using BattleSimulatorAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BattleSimulatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BreezeController : ControllerBase
    {
        private readonly IBreezeRepository _breezeRepository;

        public BreezeController(IBreezeRepository breezeRepository)
        {
            _breezeRepository = breezeRepository;
        }

        [HttpGet("metadata")]
        public IActionResult GetMetadata()
        {
            return Ok(_breezeRepository.GetMetadata());
        }

        [HttpPost("savechanges")]
        public async Task<IActionResult> SaveChanges([FromBody] JObject saveBundle)
        {
            var saveResult = await _breezeRepository.SaveChangesAsync(saveBundle);
            return Ok(saveResult);
        }

        // Generic GET for all entities (e.g., /api/breeze/entity/Fighter)
        [HttpGet("entity/{entityName}")]
        public async Task<IActionResult> GetEntities(string entityName)
        {
            var entities = await _breezeRepository.GetAllEntitiesAsync(entityName);
            return Ok(entities);
        }

        // Generic GET by ID (e.g., /api/breeze/entity/Fighter/1)
        [HttpGet("entity/{entityName}/{id}")]
        public async Task<IActionResult> GetEntityById(string entityName, int id)
        {
            var entity = await _breezeRepository.GetEntityByIdAsync(entityName, id);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        //[HttpGet("fighters")]
        //public async Task<IActionResult> GetFighters()
        //{
        //    var fighters = await _breezeRepository.GetCrudRepository<Fighter>().GetAllAsync();
        //    return Ok(fighters);
        //}

        //[HttpGet("fighters/{id}")]
        //public async Task<IActionResult> GetFighterById(int id)
        //{
        //    var fighter = await _breezeRepository.GetCrudRepository<Fighter>().GetByIdAsync(id);
        //    if (fighter == null) return NotFound();
        //    return Ok(fighter);
        //}

        //[HttpPost("fighters")]
        //public async Task<IActionResult> AddFighter([FromBody] Fighter fighter)
        //{
        //    await _breezeRepository.GetCrudRepository<Fighter>().AddAsync(fighter);
        //    return CreatedAtAction(nameof(GetFighterById), new { id = fighter.Id }, fighter);
        //}

        //[HttpPut("fighters/{id}")]
        //public async Task<IActionResult> UpdateFighter(int id, [FromBody] Fighter fighter)
        //{
        //    if (id != fighter.Id) return BadRequest();
        //    await _breezeRepository.GetCrudRepository<Fighter>().UpdateAsync(fighter);
        //    return NoContent();
        //}

        //[HttpDelete("fighters/{id}")]
        //public async Task<IActionResult> DeleteFighter(int id)
        //{
        //    await _breezeRepository.GetCrudRepository<Fighter>().DeleteAsync(id);
        //    return NoContent();
        //}
    }

}
