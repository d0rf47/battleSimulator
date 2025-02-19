using BattleSimulatorAPI.Repositories.Models.DTO;
using BattleSimulatorAPI.Repositories.Models.Repositories;
using BattleSimulatorAPI.Repositories.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Web.Http.OData.Query;

namespace BattleSimulatorAPI.DataLayer.Models.Repositories
{
    public interface IFighterRepository : ICrudRepository<IFighterModel>
    {
    }

	public class FighterRepository : CrudRepository<IFighter, IFighterInfo, FighterViewModel, IFighterModel>, IFighterRepository
	{
		//public FighterRepository(BattleSimDbContext context) : base(context) { }

		//public async Task<IEnumerable<Fighter>> GetAllAsync()
		//{
		//    return await _dbSet
		//        .Include(f => f.ElementType)  // Load ElementType
		//        .Include(f => f.FighterType)  // Load FighterType
		//        .Include(f => f.FighterAttacks) // Load FighterAttacks
		//            .ThenInclude(fa => fa.Attack) // Load the Attack details inside FighterAttacks
		//                .ThenInclude(a => a.ElementType) // 
		//        .ToListAsync();
		//}

		//public async Task<Fighter> GetByIdAsync(int id)
		//{
		//    return await _dbSet
		//        .Include(f => f.ElementType)  // Load ElementType
		//        .Include(f => f.FighterType)  // Load FighterType
		//        .Include(f => f.FighterAttacks) // Load FighterAttacks
		//            .ThenInclude(fa => fa.Attack) // Load the Attack details inside FighterAttacks
		//                .ThenInclude(a => a.ElementType) // Load ElementType for Attack
		//        .FirstOrDefaultAsync(f => f.Id == id);
		//}
		DbsResult<IEnumerable<IFighterModel>> GetInfoListPaged(ODataQueryOptions<DynamicPoco> queryOptions, bool showAll, int maxRecordsToReturn)
		{
			throw new NotImplementedException();
		}

		PocoSaveResult SaveChanges(JObject model)
		{
			throw new NotImplementedException();
		}
	}
}
