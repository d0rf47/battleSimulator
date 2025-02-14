namespace BattleSimulatorAPI.Repositories.Models
{
    public interface IEntity
    {
        int Id { get; set; }
        public void PrintAttributes();
    }

    public class Entity
    {
        public Type EntityType { get; set; }
        public List<EntityProperty> EntityProperties { get; set; } = new();

        public Entity()
        {

        }
    }

    public class EntityProperty
    {
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public EntityProperty() { }
    }
}
