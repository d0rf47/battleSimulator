using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace BattleSimulatorAPI.Repositories
{
    public interface IAttackRepository : ICrudRepository<Attack>
    {
        Task<IEnumerable<Attack>> GetAllWithDetailsAsync();
        Task<Attack> GetByIdWithDetailsAsync(int id);
    }

    public class AttackRepository : CrudRepository<Attack>, IAttackRepository
    {
        public AttackRepository(BattleSimDbContext context) : base(context) { }

        public async Task<IEnumerable<Attack>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(f => f.ElementType)
                .ToListAsync();
        }

        public async Task<Attack> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(f => f.ElementType)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}
