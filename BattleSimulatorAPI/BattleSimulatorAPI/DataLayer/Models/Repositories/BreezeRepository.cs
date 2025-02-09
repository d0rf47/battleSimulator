using Breeze.Persistence;
using Breeze.Persistence.EFCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;

namespace BattleSimulatorAPI.Repositories.Models.Repositories
{
    public interface IBreezeRepository
    {
        string GetMetadata();
        Task<SaveResult> SaveChangesAsync(JObject saveBundle);
        IQueryable EntityQuery(string entity);
        BattleSimDbContext Context { get; } // Expose context
        ICrudRepository<T> GetCrudRepository<T>() where T : class;
        Task<object> GetEntityByIdAsync(string entityName, int id);
        Task<IEnumerable<object>> GetAllEntitiesAsync(string entityName);
    }


    public class BreezeRepository : EFPersistenceManager<BattleSimDbContext>, IBreezeRepository
    {
        private readonly IServiceProvider _serviceProvider;

        public BreezeRepository(BattleSimDbContext context, IServiceProvider serviceProvider)
            : base(context)
        {
            _serviceProvider = serviceProvider;
        }

        public BattleSimDbContext Context => base.Context; // Expose context

        // Get the CrudRepository for a specific entity type
        public ICrudRepository<T> GetCrudRepository<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<ICrudRepository<T>>(); // Resolves CrudRepository<T> using DI
        }

        public string GetMetadata()
        {
            return Metadata();
        }

        /** CRUD METHODS **/

        public async Task<SaveResult> SaveChangesAsync(JObject saveBundle)
        {
            return await base.SaveChangesAsync(saveBundle);
        }

        public IQueryable EntityQuery(string entity)
        {
            return GetType()
                .GetMethod("Queryable")?
                .MakeGenericMethod(Context.Model.FindEntityType(entity)?.ClrType ?? throw new InvalidOperationException("Entity type not found"))
                .Invoke(this, null) as IQueryable ?? throw new InvalidOperationException("Queryable invocation failed");
        }                

        public async Task<IEnumerable<object>> GetAllEntitiesAsync(string entityName)
        {
            var entityType = Context.Model.GetEntityTypes()
                .FirstOrDefault(e => e.ClrType.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase))?.ClrType;

            if (entityType == null) throw new InvalidOperationException("Entity type not found");

            var repoType = typeof(ICrudRepository<>).MakeGenericType(entityType);
            var repo = _serviceProvider.GetRequiredService(repoType);
            var method = repoType.GetMethod("GetAllAsync") ?? throw new InvalidOperationException("GetAllAsync method not found on repository.");

            // Invoke asynchronously and cast properly
            var task = (Task)method.Invoke(repo, null);
            await task.ConfigureAwait(false); // Await the task

            // Use reflection to get the actual result
            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty.GetValue(task);

            // Cast to IEnumerable<object> safely
            return ((IEnumerable<object>)result).ToList();
        }

        public async Task<object> GetEntityByIdAsync(string entityName, int id)
        {
            var entityType = Context.Model.GetEntityTypes()
                .FirstOrDefault(e => e.ClrType.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase))?.ClrType;

            if (entityType == null) throw new InvalidOperationException("Entity type not found");

            object repo;

            // Dynamically find the correct repository interface
            var repositoryInterfaceType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.IsInterface && t.Name.Equals($"I{entityName}Repository", StringComparison.OrdinalIgnoreCase));


            if (repositoryInterfaceType != null)
            {
                // Resolve the specific repository (IFighterRepository, IAttackRepository, etc.)
                repo = _serviceProvider.GetService(repositoryInterfaceType);
            }
            else
            {
                // Fall back to the generic CRUD repository (ICrudRepository<T>)
                var genericRepoType = typeof(ICrudRepository<>).MakeGenericType(entityType);
                repo = _serviceProvider.GetRequiredService(genericRepoType);
            }

            // If no specific repo is found, fall back to the generic CrudRepository<T>
            if (repo == null)
            {
                var repoType = typeof(ICrudRepository<>).MakeGenericType(entityType);
                repo = _serviceProvider.GetRequiredService(repoType);
            }
            var method = repo.GetType().GetMethod("GetByIdAsync") ?? throw new InvalidOperationException("GetByIdAsync method not found on repository.");
            var method2 = entityType.GetMethod("PrintAttributes");

            

            var task = (Task)method.Invoke(repo, new object[] { id });
            await task.ConfigureAwait(false); // Await the task

            // Use reflection to get the actual result
            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty.GetValue(task);

            method2.Invoke(result, null);

            return result;            
        }
    }

}
