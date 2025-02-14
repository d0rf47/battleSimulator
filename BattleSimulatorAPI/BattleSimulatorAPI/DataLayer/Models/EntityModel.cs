using BattleSimulatorAPI.Repositories.Models;

namespace BattleSimulatorAPI.DataLayer.Models
{
    public class EntityModel
    {
        public List<Entity> Entities { get; }

        public EntityModel() 
        { 
            Entities = new List<Entity>();
        }

        public Entity GetEntity(string typeName)
        {
            return Entities.FirstOrDefault(e => e.EntityType.Name == typeName);
        }
    }
}
