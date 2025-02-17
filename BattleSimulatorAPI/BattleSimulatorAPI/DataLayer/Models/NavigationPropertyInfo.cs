using System;
using System.Reflection;

namespace BattleSimulatorAPI.DataLayer.Models
{
	public class NavigationPropertyInfo
	{
		public PropertyInfo OriginalPropertyInfo { get; set; }
		public Type TargetNavigationType { get; set; }
		public bool IsScalar { get; set; }
		public bool IsDirectChild { get; set; }
	}
}
