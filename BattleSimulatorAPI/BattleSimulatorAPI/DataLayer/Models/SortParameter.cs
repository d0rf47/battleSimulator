using Microsoft.Data.SqlClient.Server;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;

namespace BattleSimulatorAPI.DataLayer.Models
{
	/// <summary>
	/// Interface for Table-valued parameters
	/// </summary>
	public interface ITableValuedParameter
	{
		/// <summary>
		/// Returns parameter
		/// </summary>
		SqlParameter GetParameter();
	}
	/// <summary>
	/// Class to represent the Parameter used in Sorting
	/// </summary>
	[Serializable]
	public sealed class SortParameter
	{
		/// <summary>
		/// Name of Field
		/// </summary>
		public string FieldName { get; set; }
		/// <summary>
		/// The Sorting Order
		/// </summary>
		public SortOrder SortOrder { get; set; }

		/// <summary>
		/// Gets the Record for Field Name and Sort Order
		/// </summary>
		/// <returns>The Data Record</returns>
		internal SqlDataRecord GetRecord(IReadOnlyDictionary<string, string> overriddenProperties)
		{
			var record = new SqlDataRecord(
				new SqlMetaData("Id", SqlDbType.Int, true, false, SortOrder.Unspecified, -1),
				new SqlMetaData("FieldName", SqlDbType.NVarChar, 128, 0, SqlCompareOptions.IgnoreCase, false, false, SortOrder.Ascending, 0),
				new SqlMetaData("SortOrder", SqlDbType.NVarChar, 4, 0, SqlCompareOptions.IgnoreCase, false, false, SortOrder.Ascending, 1)
			);

			var fieldName = overriddenProperties != null && overriddenProperties.ContainsKey(FieldName)
				? overriddenProperties[FieldName]
				: FieldName;

			record.SetString(record.GetOrdinal("FieldName"), fieldName);
			record.SetString(record.GetOrdinal("SortOrder"), SortOrder.ToString());

			return record;
		}
	}

	/// <summary>
	/// The List of Parameters to Sort by
	/// </summary>
	[Serializable]
	public class SortParameterList : List<SortParameter>, ITableValuedParameter
	{
		/// <summary>
		/// Creates a new Sort Parameter with default values
		/// </summary>
		/// <returns></returns>
		public SqlParameter GetParameter()
		{
			return GetParameter(null);
		}

		/// <summary>
		/// Creates a new Sort Parameter with default values
		/// </summary>
		/// <returns></returns>
		public SqlParameter GetParameter(IReadOnlyDictionary<string, string> overriddenProperties)
		{
			return new SqlParameter()
			{
				ParameterName = "SortParameter",
				SqlDbType = SqlDbType.Structured,
				TypeName = "Core.SortParameter",
				Value = GetRecords(overriddenProperties)
			};
		}

		private List<SqlDataRecord> GetRecords(IReadOnlyDictionary<string, string> overriddenProperties)
		{
			return Count.Equals(0) ? null : this.Select(item => item.GetRecord(overriddenProperties)).ToList();
		}
	}
}
