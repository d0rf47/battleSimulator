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
   }

}
