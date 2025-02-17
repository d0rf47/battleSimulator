using Breeze.ContextProvider;

namespace BattleSimulatorAPI.DataLayer
{
	public class InternalKeyMapping
	{
		public Guid TempKey;
		public string FullTypeName;
		public Guid RealKey;
		public string FullBusinessContractName;

		public InternalKeyMapping(Guid key, string fullTypeName, string fullBusinessContractName)
		{
			TempKey = key;
			FullTypeName = fullTypeName;
			FullBusinessContractName = fullBusinessContractName;
		}
		public InternalKeyMapping(Guid key, string fullTypeName)
		{
			TempKey = key;
			FullTypeName = fullTypeName;
		}

		public InternalKeyMapping(Guid key)
		{
			TempKey = key;
		}

		public KeyMapping GetKeyMapping()
		{
			return new KeyMapping
			{
				EntityTypeName = FullTypeName,
				TempValue = TempKey,
				RealValue = RealKey
			};
		}
	}
}
