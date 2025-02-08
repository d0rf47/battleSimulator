
using BattleSimOne.Factory;
using BattleSimOne.Models;

namespace BattleSimOne
{
	class Program
	{

		static void Main(string[] args)
		{

			List<Fighter> Players = BattleSimulator.LoadFighters();

			Fighter winner =  BattleSimulator.SimulateFight(Players[0], Players[1]);

			for (int i = 2; i < Players.Count - 2; i++)
			{
                winner = BattleSimulator.SimulateFight(winner, Players[i]);
            }

		}
			
    }
}