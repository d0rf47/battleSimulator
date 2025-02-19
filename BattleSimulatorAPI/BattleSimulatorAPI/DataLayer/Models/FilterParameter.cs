using Microsoft.Data.SqlClient.Server;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;
using BattleSimulatorAPI.DataLayer.Models.Enums;

namespace BattleSimulatorAPI.DataLayer.Models
{
	/// <summary>
	/// Class for Filter Parameter
	/// </summary>
	[Serializable]
	public sealed class FilterParameter
	{
		/// <summary>
		/// Name of Field to Filter by
		/// </summary>
		public string FieldName { get; set; }
		/// <summary>
		/// Data type of Field to Filter by
		/// </summary>
		public string Datatype { get; set; }
		/// <summary>
		/// Primary Value of Field to Filter by
		/// </summary>
		public string PrimaryValue { get; set; }
		/// <summary>
		/// Secondary Value of Field to Filter by
		/// </summary>
		public string SecondaryValue { get; set; }
		/// <summary>
		/// Operation to Filter by
		/// </summary>
		public FilterOperation Operation { get; set; }
		/// <summary>
		/// Group Id to Filter by
		/// </summary>
		public int FilterGroupId { get; set; }

		/// <summary>
		/// Get Value from Database
		/// </summary>
		/// <returns></returns>
		internal SqlDataRecord GetRecord(IReadOnlyDictionary<string, string> overriddenProperties)
		{
			var record = new SqlDataRecord(
				new SqlMetaData("Id", SqlDbType.Int, true, false, Microsoft.Data.SqlClient.SortOrder.Unspecified, -1),
				new SqlMetaData("FieldName", SqlDbType.NVarChar, 128, 0, SqlCompareOptions.IgnoreCase, false, false, Microsoft.Data.SqlClient.SortOrder.Ascending, 1),
				new SqlMetaData("PrimaryValue", SqlDbType.NVarChar, 4000, 0, SqlCompareOptions.IgnoreCase, false, false, Microsoft.Data.SqlClient.SortOrder.Ascending, 3),
				new SqlMetaData("Operation", SqlDbType.Int, false, false, Microsoft.Data.SqlClient.SortOrder.Ascending, 5),
				new SqlMetaData("SecondaryValue", SqlDbType.NVarChar, 4000, 0, SqlCompareOptions.IgnoreCase, false, false, Microsoft.Data.SqlClient.SortOrder.Ascending, 4),
				new SqlMetaData("Datatype", SqlDbType.NVarChar, 20, 0, SqlCompareOptions.IgnoreCase, false, false, Microsoft.Data.SqlClient.SortOrder.Ascending, 2),
				new SqlMetaData("FilterGroupId", SqlDbType.Int, false, false, Microsoft.Data.SqlClient.SortOrder.Ascending, 0));

			var fieldName = overriddenProperties != null && overriddenProperties.ContainsKey(FieldName)
				? overriddenProperties[FieldName]
				: FieldName;

			record.SetString(record.GetOrdinal("FieldName"), fieldName);
			record.SetString(record.GetOrdinal("Datatype"), Datatype);
			record.SetString(record.GetOrdinal("PrimaryValue"), PrimaryValue);
			record.SetInt32(record.GetOrdinal("Operation"), (int)Operation);
			record.SetInt32(record.GetOrdinal("FilterGroupId"), FilterGroupId);

			if (SecondaryValue != null)
				record.SetString(record.GetOrdinal("SecondaryValue"), SecondaryValue);

			return record;
		}
	}

	/// <summary>
	/// List of Parameters to Filter by
	/// </summary>
	[Serializable]
	public class FilterParameterList : List<FilterParameter>, ITableValuedParameter
	{
		/// <summary>
		/// Get the Parameters to Filter By
		/// </summary>
		/// <returns></returns>
		public SqlParameter GetParameter()
		{
			return GetParameter(new Dictionary<string, string>());
		}

		/// <summary>
		/// Get the Parameters to Filter By
		/// </summary>
		/// <returns></returns>
		public SqlParameter GetParameter(IReadOnlyDictionary<string, string> overriddenProperties)
		{
			return new SqlParameter
			{
				ParameterName = "FilterParameter",
				SqlDbType = SqlDbType.Structured,
				TypeName = "Core.FilterParameter",
				Value = GetRecords(overriddenProperties)
			};
		}

		private List<SqlDataRecord> GetRecords(IReadOnlyDictionary<string, string> overriddenProperties)
		{
			return Count == 0 ? null : this.Select(item => item.GetRecord(overriddenProperties)).ToList();
		}
	}
}
