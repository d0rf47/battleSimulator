namespace BattleSimulatorAPI.DataLayer.Models
{
	public class AbstractQuery
	{
		public string TargetTypeName { get; set; }

		public string WhereConditionsXml { get; set; }

		public string OrderByExpressionsXml { get; set; }

		public int PageSize { get; set; }

		public int PageNumber { get; set; }

		public bool HasMoreRows { get; set; }
	}
}
