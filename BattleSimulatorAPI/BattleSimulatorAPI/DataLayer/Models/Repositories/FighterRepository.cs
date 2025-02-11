using BattleSimulatorAPI.Repositories.Models.DTO;
using BattleSimulatorAPI.Repositories.Models.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BattleSimulatorAPI.DataLayer.Models.Repositories
{
    public interface IFighterRepository : ICrudRepository<Fighter>
    {        
        Task<IEnumerable<Fighter>> GetAllAsync();
        Task<Fighter> GetByIdAsync(int id);        
        Task<object?> GetFightersByElement(int elementType);
        Task<object?> GetFightersByType(int fighterType);
    }

    public class FighterRepository : CrudRepository<Fighter>, IFighterRepository
    {
        public FighterRepository(BattleSimDbContext context) : base(context) { }

        public async Task<IEnumerable<Fighter>> GetAllAsync()
        {
            return await _dbSet
                .Include(f => f.ElementType)  // Load ElementType
                .Include(f => f.FighterType)  // Load FighterType
                .Include(f => f.FighterAttacks) // Load FighterAttacks
                    .ThenInclude(fa => fa.Attack) // Load the Attack details inside FighterAttacks
                        .ThenInclude(a => a.ElementType) // 
                .ToListAsync();
        }

        public async Task<Fighter> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(f => f.ElementType)  // Load ElementType
                .Include(f => f.FighterType)  // Load FighterType
                .Include(f => f.FighterAttacks) // Load FighterAttacks
                    .ThenInclude(fa => fa.Attack) // Load the Attack details inside FighterAttacks
                        .ThenInclude(a => a.ElementType) // Load ElementType for Attack
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        Task<object?> IFighterRepository.GetFightersByElement(int elementType)
        {
            throw new NotImplementedException();
        }

        Task<object?> IFighterRepository.GetFightersByType(int fighterType)
        {
            throw new NotImplementedException();
        }
    }
}
