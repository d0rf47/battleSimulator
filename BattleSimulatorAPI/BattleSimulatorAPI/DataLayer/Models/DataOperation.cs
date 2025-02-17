using BattleSimulatorAPI.Repositories.Models;

namespace BattleSimulatorAPI.DataLayer.Models
{
	public enum DataOperationType : int
	{
		Added = 1,
		Modified = 2,
		Deleted = 3,
		Unchanged = 4,
		Detached = 9999
	}

	public class DataOperation
	{
		public DataOperationType Type { get; set; }
		public Dictionary<string, object> ModifiedProperties { get; set; }

		public Entity Entity { get; set; }

		public DataOperation()
		{
			this.Type = DataOperationType.Unchanged;
			this.ModifiedProperties = new Dictionary<string, object>();
		}
	}
}
