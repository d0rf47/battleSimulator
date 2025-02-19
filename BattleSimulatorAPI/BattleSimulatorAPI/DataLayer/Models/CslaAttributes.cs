namespace BattleSimulatorAPI.DataLayer.Models
{
	/// <summary>
	/// This class represents an attribute to define Csla Foreignkey property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CslaForeignKeyAttribute : Attribute
	{
	}

	/// <summary>
	/// This class represents an attribute to define Csla Parent key property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CslaParentKeyAttribute : Attribute
	{
	}

	/// <summary>
	/// This class represents an attribute to define Hidden property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class HiddenAttribute : Attribute
	{
	}

	/// <summary>
	/// This class represents an attribute to define Csla child object property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CslaChildObjectAttribute : Attribute
	{
	}

	/// <summary>
	/// This class represents an attribute to define Csla Footer object property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class CslaFooterObjectAttribute : Attribute
	{
	}

	/// <summary>
	/// This class represents an attribute to define Short Field for a business object
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ShortFieldAttribute : Attribute
	{
	}

	/// <summary>
	/// This class represents an attribute to define Display Field for a business object
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DisplayFieldAttribute : Attribute
	{
	}

	/// <summary>
	/// This class represents an attribute to define lookup property for a Csla Foreignkey field
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ForeignKeyLookupAttribute : Attribute
	{
	}
}
