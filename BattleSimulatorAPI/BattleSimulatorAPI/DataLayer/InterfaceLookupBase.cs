namespace BattleSimulatorAPI.DataLayer
{
	public interface InterfaceLookupBase
	{
		/// <summary>
		/// Id Property
		/// </summary>
		int Id { get; }

		/// <summary>
		/// EntityId Property
		/// </summary>
		int EntityId { get; }

		/// <summary>
		/// EntityDisplayShort Property
		/// </summary>
		string EntityDisplayShort { get; }

		/// <summary>
		/// EntityDisplayLong Property
		/// </summary>
		string EntityDisplayLong { get; }

		/// <summary>
		/// ImplementationId Property
		/// </summary>
		int ImplementationId { get; }

		/// <summary>
		/// Disabled Property
		/// </summary>
		bool Disabled { get; }
	}
}
