using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.EntityFrameworkCore;
using BattleSimulatorAPI.DataLayer;

namespace BattleSimulatorAPI.Repositories.Models.Repositories
{
    public interface IFighterAttackRepository : ICrudRepository<FighterAttack>
    {
        Task<IEnumerable<FighterAttack>> GetByFighterIdAsync(int fighterId);
        Task<IEnumerable<FighterAttack>> GetByAttackIdAsync(int attackId);
    }

    public class FighterAttackRepository : CrudRepository<FighterAttack>, IFighterAttackRepository
    {
        public FighterAttackRepository(BattleSimDbContext context) : base(context) { }

        public async Task<IEnumerable<FighterAttack>> GetByFighterIdAsync(int fighterId)
        {
            return await _dbSet.Where(fa => fa.FighterId == fighterId).ToListAsync();
        }

        public async Task<IEnumerable<FighterAttack>> GetByAttackIdAsync(int attackId)
        {
            return await _dbSet.Where(fa => fa.AttackId == attackId).ToListAsync();
        }
    }
}
