namespace BattleSimulatorAPI.Repositories.Models.DTO
{

    //CoolDown = 5 + (AttackPoints - 30) * (45 / 70)

    public class Attack : IEntity
    {
        public int Id { get; }
        public ElementTypeEnum ElementType { get; set; }
        public string Name { get; set; }
        public int AttackPoints { get; set; }
        public int CoolDown { get; set; }

        public void PrintAttributes()
        {
            Console.WriteLine($"{Name} - {AttackPoints}AP -- {ElementType} Type");
        }

        Task ICrudRepository<IEntity>.AddAsync(IEntity entity)
        {
            throw new NotImplementedException();
        }

        Task ICrudRepository<IEntity>.DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<IEntity>> ICrudRepository<IEntity>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<IEntity> ICrudRepository<IEntity>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task ICrudRepository<IEntity>.UpdateAsync(IEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
