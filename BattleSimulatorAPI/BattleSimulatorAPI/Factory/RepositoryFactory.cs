using BattleSimulatorAPI.DataLayer.Models.ViewModels;
using BattleSimulatorAPI.Repositories.Models.Repositories;

namespace BattleSimulatorAPI.Factory
{
    public static class RepositoryFactory
    {
        private static readonly Dictionary<string, Type> RepoCache = new();

        static RepositoryFactory()
        {

        }

        #region  Concrete Repo Builders
        
        public static TRepositoryContract GetRepository<TRepositoryContract, TVm>()
            where TRepositoryContract : class, ICrudRepository<TVm>
            where TVm : class, IViewModel
        {
            return FindRepository<TRepositoryContract>();
        }

        private static T FindRepository<T>()
        {
            var type = typeof(T);

            return (T)Activator.CreateInstance(type);
        }

        #endregion
    }
}