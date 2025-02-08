namespace BattleSimulatorAPI.Repositories.Models.DTO
{
    public class Fighter : IEntity
    {
        public int Id { get; }
        public ElementTypeEnum ElementType { get; set; }
        public FighterTypeEnum FighterType { get; set; }
        public string Name { get; set; }
        public int AttackPoints { get; set; }
        public int DefensePoints { get; set; }
        public int HealthPoints { get; set; }
        public int Speed { get; set; }
        public int Level { get; set; }
        public List<Attack> Attacks { get; set; } = new List<Attack>();

        public void PrintAttributes()
        {
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Type: {ElementType}");
            Console.WriteLine($"Attack: {AttackPoints}");
            Console.WriteLine($"Defense: {DefensePoints}");
            Console.WriteLine($"Health: {HealthPoints}");
            Console.WriteLine("Attack List");
            foreach (var attack in Attacks)
            {

                attack.PrintAttributes();
            }
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
