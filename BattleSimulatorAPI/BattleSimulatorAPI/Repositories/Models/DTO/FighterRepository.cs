using Microsoft.EntityFrameworkCore;

namespace BattleSimulatorAPI.Repositories.Models.DTO
{
    public interface IFighterRepository : ICrudRepository<Fighter>
    {
        IQueryable<Fighter> GetAllFighters();
        Task<IEnumerable<Fighter>> GetAllWithDetailsAsync();
        Task<Fighter> GetByIdWithDetailsAsync(int id);
        Task<Fighter> GetFighterById(int id);
        Task<object?> GetFightersByElement(int elementType);
        Task<object?> GetFightersByType(int fighterType);
    }

    public class FighterRepository : CrudRepository<Fighter>, IFighterRepository
    {
        public FighterRepository(BattleSimDbContext context) : base(context) { }

        public async Task<IEnumerable<Fighter>> GetAllWithDetailsAsync()
        {
            return await _dbSet
                .Include(f => f.ElementType)
                .Include(f => f.FighterType)
                .ToListAsync();
        }

        public async Task<Fighter> GetByIdWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(f => f.ElementType)
                .Include(f => f.FighterType)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        IQueryable<Fighter> IFighterRepository.GetAllFighters()
        {
            return _dbSet
                .Include(f => f.ElementType)
                .Include(f => f.FighterType);
                
        }

        Task<Fighter> IFighterRepository.GetFighterById(int id)
        {
            throw new NotImplementedException();
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
