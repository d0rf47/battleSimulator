using Csla.Core;
using System.Reflection;
using System.Diagnostics;

namespace BattleSimulatorAPI.DataLayer.Models
{
	/// <summary>
	/// Property to be used in Data Transfer Object (DTO)
	/// </summary>
	[Serializable]
	public sealed class PrefetchProperty
	{
		/// <summary>
		/// Name of Property
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Child Properties
		/// </summary>
		public PrefetchPropertyList ChildProperties { get; set; }

		/// <summary>
		/// Property to Fetch based on specified Name
		/// </summary>
		/// <param name="propertyName"> Name of the property</param>
		public PrefetchProperty(string propertyName)
		{
			PropertyName = propertyName;
			ChildProperties = new PrefetchPropertyList();
		}

		/// <summary>
		/// Property to Fetch based on specified Name
		/// </summary>
		/// <param name="propertyInfo"> Object to load Property from</param>
		public PrefetchProperty(IPropertyInfo propertyInfo) : this(propertyInfo.Name)
		{
		}
	}

	/// <summary>
	/// List of Properties to be used in Data Transfer Object (DTO)
	/// </summary>
	[Serializable]
	[DebuggerDisplay("Count = {_properties.Count}")]
	public sealed class PrefetchPropertyList
	{
		private enum ListType
		{
			Editable,
			None,
			All,
			ChildObjects,
			Hierarchy
		}

		private ListType _type = ListType.Editable;

		private readonly List<PrefetchProperty> _properties = new List<PrefetchProperty>();

		private static readonly PrefetchPropertyList AllProperties = new PrefetchPropertyList() { _type = ListType.All };
		private static readonly PrefetchPropertyList NoProperties = new PrefetchPropertyList() { _type = ListType.None };
		private static readonly PrefetchPropertyList ChildObjectsProperties = new PrefetchPropertyList() { _type = ListType.ChildObjects };
		private static readonly PrefetchPropertyList HierarchyProperties = new PrefetchPropertyList() { _type = ListType.Hierarchy, ResolveForeignKeys = true };

		public static readonly PrefetchPropertyList All = AllProperties;
		public static readonly PrefetchPropertyList None = NoProperties;
		public static readonly PrefetchPropertyList ChildObjects = ChildObjectsProperties;
		public static readonly PrefetchPropertyList Hierarchy = HierarchyProperties;

		/// <summary>
		/// Add Properties to the Existing List
		/// </summary>
		/// <param name="properties">Properties to Add</param>
		public void Add(params PrefetchProperty[] properties)
		{
			if (_type != ListType.Editable)
				throw new Exception("The list is readonly!");

			_properties.AddRange(properties);
		}

		/// <summary>
		/// Add Properties to the Existing List
		/// </summary>
		/// <param name="properties">Readonly Properties to Add</param>
		public void Add(params IPropertyInfo[] properties)
		{
			if (_type != ListType.Editable)
				throw new Exception("The list is readonly!");

			_properties.AddRange(properties.Select(p => new PrefetchProperty(p)));
		}

		/// <summary>
		/// Checks whether the property contains or not
		/// </summary>
		/// <param name="objectType">The Type of Property</param>
		/// <param name="cslaProperty">CSLA Property</param>
		/// <returns>boolean value indicating whether it Conaints or Not</returns>
		public bool Contains(Type objectType, IPropertyInfo cslaProperty)
		{
			var objectProperty = objectType.GetProperty(cslaProperty.Name);

			if (objectProperty == null)
				throw new InvalidOperationException($"{objectType.Name} does not contain a property with name '{cslaProperty.Name}'");

			return Contains(objectProperty, cslaProperty);
		}

		/// <summary>
		/// Checks whether the property contains or not
		/// </summary>
		/// <param name="objectProperty">The Type of Property</param>
		/// <param name="cslaProperty">CSLA Property</param>
		/// <returns>boolean value indicating whether it Conaints or Not</returns>
		public bool Contains(PropertyInfo objectProperty, IPropertyInfo cslaProperty)
		{
			switch (_type)
			{
				case ListType.All:
				case ListType.Hierarchy:
					return true;

				case ListType.None:
					return false;

				case ListType.ChildObjects:
					return objectProperty.GetCustomAttribute<CslaChildObjectAttribute>() != null;

				case ListType.Editable:
					return _properties.Any(p => p.PropertyName == cslaProperty.Name);

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Clears the properties collection
		/// </summary>
		public void Clear() => _properties.Clear();

		/// <summary>
		/// Gets the Specified Property from the Collection based on Name
		/// </summary>
		/// <param name="property">The Property to Find</param>
		/// <returns></returns>
		public PrefetchProperty this[IPropertyInfo property]
		{
			get
			{
				var childProps = _type == ListType.Hierarchy ? Hierarchy : None;

				return
					  _properties.FirstOrDefault(p => p.PropertyName == property.Name)
					  ?? new PrefetchProperty(property) { ChildProperties = childProps };
			}
		}

		/// <summary>
		/// Get/sets the flag indicating if foreign keys of the object should be resolved, to fetch the corresponding Lookup object
		/// </summary>
		public bool ResolveForeignKeys { get; set; }
	}
}
