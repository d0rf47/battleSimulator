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

		public Dictionary<Entity, EntityNavigationProperty> GetEntitiesForNavigationTarget(Entity checkEntity)
		{
			var navigationPropertiesDict = new Dictionary<Entity, EntityNavigationProperty>();

			foreach (var entity in Entities)
			{
				var navigationProperty = entity.NavigationProperties.FirstOrDefault(np =>
					(checkEntity.EntityType.BaseType != null || typeof(IPoco).IsAssignableFrom(checkEntity.EntityType))
					&&
						(
							np.TargetEntity == checkEntity ||
							(!np.TargetEntity.EntityType.IsInterface && np.TargetEntity.EntityType.Name == checkEntity.EntityType.BaseType.Name) ||
							(np.TargetEntity.EntityType.IsInterface && np.TargetEntity.EntityType.IsAssignableFrom(checkEntity.EntityType))
						)
					);

				if (navigationProperty == null) continue;

				if (!navigationPropertiesDict.ContainsKey(entity))
				{
					navigationPropertiesDict.Add(entity, navigationProperty);
				}
			}

			return navigationPropertiesDict;
		}
	}
}
