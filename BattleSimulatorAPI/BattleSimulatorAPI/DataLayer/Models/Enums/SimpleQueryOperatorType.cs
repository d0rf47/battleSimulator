namespace BattleSimulatorAPI.DataLayer.Models.Enums
{
	/// <summary>
	/// Comparisons
	/// </summary>
	public enum SimpleQueryOperatorType
	{
		Equals = 0,
		Contains = 1,
		GreatThan = 2,
		LessThan = 3,
		GreatThanEqual = 4,
		LessThanEqual = 5,
		StartsWith = 6,
		EndsWith = 7,
		NotEqual = 8,
		In = 12,
	}
}
