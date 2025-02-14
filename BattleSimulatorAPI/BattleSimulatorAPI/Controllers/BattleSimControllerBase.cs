using BattleSimulatorAPI.DataLayer.Models.ViewModels;
using BattleSimulatorAPI.Factory;
using BattleSimulatorAPI.Repositories.Models.Repositories;

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


    }
}
