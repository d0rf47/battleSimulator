namespace BattleSimulatorAPI.DataLayer.Models
{
	/// <summary>
	/// Query Parameter for Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class QueryParameterAttribute : Attribute
	{
		/// <summary>
		/// Name of Parameter
		/// </summary>
		public string Name { get; set; }
	}
}
