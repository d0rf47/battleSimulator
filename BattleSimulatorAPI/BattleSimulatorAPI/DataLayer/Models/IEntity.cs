namespace BattleSimulatorAPI.Repositories.Models
{
    public interface IEntity
    {
        int Id { get; set; }
        public void PrintAttributes();
    }
}
