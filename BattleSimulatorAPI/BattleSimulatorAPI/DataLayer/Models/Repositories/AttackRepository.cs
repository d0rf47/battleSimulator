using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace BattleSimulatorAPI.Repositories.Models.Repositories
{
    public interface IAttackRepository : ICrudRepository<Attack>
    {
        Task<IEnumerable<Attack>> GetAllAsync();
        Task<Attack> GetByIdAsync(int id);
    }

    public class AttackRepository : CrudRepository<Attack>, IAttackRepository
    {
        public AttackRepository(BattleSimDbContext context) : base(context) { }

        public async Task<IEnumerable<Attack>> GetAllAsync()
        {
            return await _dbSet
                .Include(f => f.ElementType)                
                .ToListAsync();
        }
        public async Task<Attack> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(f => f.ElementType)  // Load ElementType                
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
