namespace BattleSimulatorAPI.Repositories.Models
{
    public interface IEntity : ICrudRepository<IEntity>
    {
        int Id { get; }
        public void PrintAttributes();
    }
}
