using Csla;

namespace BattleSimulatorAPI.Repositories.Models
{

	public interface IEntity<out TIdDataType> : IBusinessBase
		where TIdDataType : IComparable<TIdDataType>, IEquatable<TIdDataType>
	{
		TIdDataType Id { get; }
		public void PrintAttributes();
    }

    public interface IReadOnlyBase<out TIdDataType> : IReadOnlyBase
		where TIdDataType : IComparable<TIdDataType>, IEquatable<TIdDataType>
    {
		TIdDataType Id { get; }
	}


	public class Entity
    {
        public Type EntityType { get; set; }
        public List<EntityProperty> DataProperties { get; set; } = new();
		public List<EntityNavigationProperty> NavigationProperties { get; private set; }

		public Entity()
        {
			this.DataProperties = new List<EntityProperty>();
			this.NavigationProperties = new List<EntityNavigationProperty>();
		}
    }

    public class EntityProperty
    {
        public string Name { get; set; }
        public Type PropertyType { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public EntityProperty() { }
    }

	public class EntityNavigationProperty
	{
		public string Name { get; set; }
		public Entity TargetEntity { get; set; }
		public string ForeignKeyName { get; set; }
		public string AssociationName { get; set; }
		public bool IsScalar { get; set; }
		public bool IsDirectChild { get; set; }
	}
}
