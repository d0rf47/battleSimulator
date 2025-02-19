namespace BattleSimulatorAPI.DataLayer.Models.Enums
{
	/// <summary>
	/// Sort Order Enumerator
	/// </summary>
	public enum SortOrder
	{
		Asc,
		Desc
	}

	/// <summary>
	/// Filter Operation Enumerator
	/// </summary>
	public enum FilterOperation
	{
		All,                  //0
		Exact,                //1
		Like,                 //2
		Between,              //3
		In,                   //4
		StartsWith,           //5
		EndsWith,             //6
		Contains,             //7
		LessThanOrEqual,      //8
		GreaterThanOrEqual,   //9
		RegEx,               //10
		NotEqual,            //11
		NotUsed,             //12
		EffectiveDateStatus, //13
	}

	public enum PagedResultType
	{
		Sequential = 0,
		PagedResult = 1,
		EntityResult = 2,
		Parallel = 3
	}
}
