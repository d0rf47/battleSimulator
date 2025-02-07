
using BattleSimOne.Factory;

namespace BattleSimOne
{
	class Program
	{
		
		static void Main(string[] args)
		{
			BattleSimulator.LoadAttacks();

			BattleSimulator.LoadFighter();
		}
	}
}