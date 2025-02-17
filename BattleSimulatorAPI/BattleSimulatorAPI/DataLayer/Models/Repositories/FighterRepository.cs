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

		Fighter ICrudRepository<Fighter>.CreateEntity(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			throw new NotImplementedException();
		}

		bool ICrudRepository<Fighter>.DeleteEntity(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			throw new NotImplementedException();
		}

		DbsResult<Fighter> ICrudRepository<Fighter>.GetEntity(ODataQueryOptions<DynamicPoco> queryOptions)
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

		DbsResult<IEnumerable<Fighter>> ICrudRepository<Fighter>.GetInfoListPaged(ODataQueryOptions<DynamicPoco> queryOptions, bool showAll, int maxRecordsToReturn)
		{
			throw new NotImplementedException();
		}

		DbsResult<IEnumerable<IFighterModel>> ICrudRepository<IFighterModel>.GetInfoListPaged(ODataQueryOptions<DynamicPoco> queryOptions, bool showAll, int maxRecordsToReturn)
		{
			throw new NotImplementedException();
		}

		PocoSaveResult ICrudRepository<Fighter>.SaveChanges(JObject model)
		{
			throw new NotImplementedException();
		}

		PocoSaveResult ICrudRepository<IFighterModel>.SaveChanges(JObject model)
		{
			throw new NotImplementedException();
		}
	}
}
