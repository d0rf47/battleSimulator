using BattleSimulatorAPI.DataLayer.Models.Repositories;
using BattleSimulatorAPI.Repositories.Models.DTO;

namespace BattleSimulatorAPI.Services
{
    public class FighterService
    {
        private readonly IFighterRepository _fighterRepository;

        public FighterService(IFighterRepository fighterRepository)
        {
            _fighterRepository = fighterRepository;
        }

        public async Task<IEnumerable<Fighter>> GetAllFighters()
        {
            return await _fighterRepository.GetAllAsync();
        }

        public async Task<Fighter> GetFighterById(int id)
        {
            return await _fighterRepository.GetByIdAsync(id);
        }
    }

}
