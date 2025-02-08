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
        Task<SaveResult> SaveChangesAsync(JObject saveBundle);
        IQueryable EntityQuery(string entity);
    }

    public class BreezeRepository : EFPersistenceManager<BattleSimDbContext>, IBreezeRepository
    {
        public BreezeRepository(BattleSimDbContext context) : base(context) { }

        public IQueryable EntityQuery(string entity)
        {
            return GetType()
                .GetMethod("Queryable")?
                .MakeGenericMethod(Context.Model.FindEntityType(entity)?.ClrType ?? throw new InvalidOperationException("Entity type not found"))
                .Invoke(this, null) as IQueryable ?? throw new InvalidOperationException("Queryable invocation failed");
        }

        public async Task<SaveResult> SaveChangesAsync(JObject saveBundle)
        {
            return await base.SaveChangesAsync(saveBundle);
        }
    }
}
