namespace BattleSimulatorAPI.DataLayer.Models
{
	public static class DateTimeExtender
	{
		public static object GetDbValue(this DateTime? input)
		{
			return input.HasValue ? (object)input.Value : DBNull.Value;
		}
	}

	public static class ListExtender
	{
		public static bool IsNullOrEmpty<TSource>(this IEnumerable<TSource> input)
		{
			return input == null || !input.Any();
		}
	}

	public static class ObjectExtender
	{
		public static string AsString(this object input, string defaultValue)
		{
			return input != null ? input.ToString() : defaultValue;
		}
	}
}
