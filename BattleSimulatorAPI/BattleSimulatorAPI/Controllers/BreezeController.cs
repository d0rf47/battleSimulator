using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using BattleSimulatorAPI.Repositories.Models.Repositories;

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
    }

}
