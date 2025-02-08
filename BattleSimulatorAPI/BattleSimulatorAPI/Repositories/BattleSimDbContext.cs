using Microsoft.EntityFrameworkCore;

namespace BattleSimulatorAPI.Repositories
{
    public class BattleSimDbContext :DbContext
    {
        public BattleSimDbContext(DbContextOptions<BattleSimDbContext> options) : base(options) { }
    }
}
