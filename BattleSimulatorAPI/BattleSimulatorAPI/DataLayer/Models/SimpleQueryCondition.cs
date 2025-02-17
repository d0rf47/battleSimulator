using BattleSimulatorAPI.DataLayer.Models.Enums;

namespace BattleSimulatorAPI.DataLayer.Models
{
	/// <summary>
	/// Binary operators
	/// </summary>
	public enum SimpleQueryConditionJoiner
	{
		None = 0,
		And = 1,
		Or = 2
	}

	/// <summary>
	/// A basic container for a query condition
	/// </summary>
	class SimpleQueryCondition
	{
		public string PropertyName { get; set; }
		public SimpleQueryOperatorType OperatorType { get; set; }
		public object Value { get; set; }
		public Type DataType { get; set; }
		public SimpleQueryConditionJoiner Joiner { get; set; }

		/// <summary>
		/// A basic constructor to be used with public setters
		/// </summary>
		public SimpleQueryCondition()
		{
			Joiner = SimpleQueryConditionJoiner.None;
		}
	}
}
