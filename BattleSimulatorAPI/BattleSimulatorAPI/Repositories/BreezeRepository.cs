using Breeze.Persistence;
using Breeze.Persistence.EFCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;

namespace BattleSimulatorAPI.Repositories
{
    public interface IBreezeRepository
    {
        string GetMetadata();
        Task<SaveResult> SaveChangesAsync(JObject saveBundle);
        IQueryable EntityQuery(string entity);

        BattleSimDbContext Context { get; } // Expose context

        ICrudRepository<T> GetCrudRepository<T>() where T : class; // New method to get a CrudRepository

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

        public string GetMetadata()
        {
            return base.Metadata();
        }

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

        public BattleSimDbContext Context => base.Context; // Expose context

        // Get the CrudRepository for a specific entity type
        public ICrudRepository<T> GetCrudRepository<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<ICrudRepository<T>>(); // Resolves CrudRepository<T> using DI
        }

        public async Task<IEnumerable<object>> GetAllEntitiesAsync(string entityName)
        {
            var entityType = Context.Model.GetEntityTypes()
            .FirstOrDefault(e => e.ClrType.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase))?.ClrType;

            if (entityType == null) throw new InvalidOperationException("Entity type not found");

            var repoType = typeof(ICrudRepository<>).MakeGenericType(entityType);
            var repo = _serviceProvider.GetRequiredService(repoType);
            var method = repoType.GetMethod("GetAllAsync");
            if (method == null)
                throw new InvalidOperationException("GetAllAsync method not found on repository.");

            // Invoke asynchronously and cast properly
            var task = (Task)method.Invoke(repo, null);
            await task.ConfigureAwait(false); // Await the task

            // Use reflection to get the actual result
            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty.GetValue(task);

            // Cast to IEnumerable<object> safely
            return ((IEnumerable<object>)result).ToList();
        }

        // Fetch entity by ID dynamically
        public async Task<object> GetEntityByIdAsync(string entityName, int id)
        {
            var entityType = Context.Model.FindEntityType(entityName)?.ClrType;
            if (entityType == null) throw new InvalidOperationException("Entity type not found");

            var repoType = typeof(ICrudRepository<>).MakeGenericType(entityType);
            var repo = _serviceProvider.GetRequiredService(repoType);
            var method = repoType.GetMethod("GetByIdAsync");

            return await (Task<object>)method.Invoke(repo, new object[] { id });
        }
    }

}
