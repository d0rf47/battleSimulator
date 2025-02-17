using BattleSimulatorAPI.DataLayer.Models.ViewModels;
using BattleSimulatorAPI.Factory;
using BattleSimulatorAPI.Repositories.Models.Repositories;
using System.Web.Http;
using System.Web.Http.OData.Query;
using Breeze.WebApi2;
using BattleSimulatorAPI.DataLayer.Models;

namespace BattleSimulatorAPI.Controllers
{

    public abstract class BattleSimControllerBase<TRepository, TRepositoryModel> : BreezeControllerBase
        where TRepository : class, ICrudRepository<TRepositoryModel>
        where TRepositoryModel : class, IViewModel
    {
        protected BattleSimControllerBase(string entityName) : base(entityName) { }

        #region Repo

        private TRepository _entityCrudRepo;

        public TRepository EntityCrudRepo
        {
            get =>
                _entityCrudRepo ?? (_entityCrudRepo =
                RepositoryFactory.GetRepository<TRepository, TRepositoryModel>());

            set => _entityCrudRepo = value;
        }
		#endregion

		#region Crud Actions

		[HttpGet]
		public virtual QueryResult GetAllEntities(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			return EntityCrudRepo.GetInfoListPaged(queryOptions, false);
		}

		[HttpGet]
		public virtual QueryResult GetAllEntitisCount(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			return EntityCrudRepo.GetInfoListPaged(queryOptions, false);
		}

		[HttpGet]
		public virtual QueryResult GetAllEntitiesEntityResult(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			return EntityCrudRepo.GetInfoListPaged(queryOptions, false);
		}

		[HttpGet]
		public virtual QueryResult GetAllEntitiesPagedResult(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			return EntityCrudRepo.GetInfoListPaged(queryOptions, false);
		}

		[HttpGet]
		public virtual QueryResult CreateEntity(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			var data = EntityCrudRepo.CreateEntity(queryOptions);

			return new QueryResult
			{
				InlineCount = 1,
				Results = data
			};
		}		

		[HttpGet]
		public virtual QueryResult DeleteEntity(ODataQueryOptions<DynamicPoco> queryOptions)
		{
			var data = EntityCrudRepo.DeleteEntity(queryOptions);

			return new QueryResult
			{
				InlineCount = 1,
				Results = data
			};
		}
		#endregion

	}
}
