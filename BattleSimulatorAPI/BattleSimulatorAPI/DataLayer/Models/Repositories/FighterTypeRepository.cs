using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace BattleSimulatorAPI.Repositories.Models.Repositories
{
    public interface IFighterTypeRepository : ICrudRepository<FighterType>
    {
        Task<FighterType?> GetByNameAsync(string typeName);
    }

    public class FighterTypeRepository : CrudRepository<FighterType>, IFighterTypeRepository
    {
        public FighterTypeRepository(BattleSimDbContext context) : base(context) { }

        public async Task<FighterType?> GetByNameAsync(string typeName)
        {
            return await _dbSet.FirstOrDefaultAsync(ft => ft.TypeName == typeName);
        }
    }
}
