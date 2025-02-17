using Breeze.WebApi2;

namespace BattleSimulatorAPI.DataLayer.Models
{
	public class DbsResult<T>
	{
		public T Result { get; set; }
		public long? InlineCount { get; set; }
		public bool HasMoreRows { get; set; }

		public static implicit operator QueryResult(DbsResult<T> resultModel)
		{
			return new QueryResult { InlineCount = resultModel.InlineCount, Results = resultModel.Result };
		}

		//public static implicit operator CustomQueryResults(DbsResult<T> resultModel)
		//{
		//	return new CustomQueryResults { InlineCount = resultModel.InlineCount, Results = resultModel.Result, HasMoreRows = resultModel.HasMoreRows };
		//}
	}
}
