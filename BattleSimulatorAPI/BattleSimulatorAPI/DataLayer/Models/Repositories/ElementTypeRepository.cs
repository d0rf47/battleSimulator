﻿using BattleSimulatorAPI.Repositories.Models.DTO;
using Microsoft.EntityFrameworkCore;
using BattleSimulatorAPI.DataLayer;

namespace BattleSimulatorAPI.Repositories.Models.Repositories
{
    public interface IElementTypeRepository : ICrudRepository<ElementType>
    {
        Task<ElementType?> GetByNameAsync(string typeName);
    }
    public class ElementTypeRepository : CrudRepository<ElementType>, IElementTypeRepository
    {
        public ElementTypeRepository(BattleSimDbContext context) : base(context) { }

        public async Task<ElementType?> GetByNameAsync(string typeName)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.TypeName == typeName);
        }
    }
}
