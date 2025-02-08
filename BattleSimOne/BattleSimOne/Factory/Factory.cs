using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BattleSimOne.Models;

namespace BattleSimOne.Factory
{
	public interface IEntity
	{		
		int Id { get; }
		public void PrintAttributes();
	}

	public abstract class Factory
	{
	}

	public static class BattleSimulator
	{
        private static Random rnd = new Random();

		

        public static Dictionary<int,Attack> LoadAttacks()
		{
			const string FilePath = @"Resources\Attacks.json";
			string json = File.ReadAllText(FilePath);
			List<Attack> attacks = JsonSerializer.Deserialize<List<Attack>>(json);
			Dictionary<int, Attack> attackDictionary = attacks.ToDictionary(a => a.Id);
			Console.WriteLine("********Attacks********");
			foreach (var att in attacks)
			{
				Console.WriteLine($"{att.Name}, {att.ElementType}, {att.AttackPoints}");
			}
			return attackDictionary;
		}

		public static List<Fighter> LoadFighters()
		{
			const string FilePath = @"Resources\Fighters.json";
			string json = File.ReadAllText(FilePath);
			List<FighterJson> fighterJsonList = JsonSerializer.Deserialize<List<FighterJson>>(json);


			// Create the attack lookup dictionary
			Dictionary<int, Attack> attackLookup = LoadAttacks();

			// Now create the Fighter objects by passing both the `attackIds` and the `attackLookup` dictionary
			List<Fighter> fighters = fighterJsonList.Select(fighterJson =>
				new Fighter(
					fighterJson.Id,
					fighterJson.ElementType,
					fighterJson.Name,
					fighterJson.AttackPoints,
					fighterJson.DefensePoints,
					fighterJson.HealthPoints,
					fighterJson.Speed,
					fighterJson.AttackIds,
					attackLookup
				)
			).ToList();

			Console.WriteLine("********Fighters********");
			foreach (var fighter in fighters)
			{
				fighter.PrintAttributes();
				Console.WriteLine();
			}
			return fighters;
		}

		public static void AttackOpponent(Fighter Attacker, Fighter Reciever, Attack attack)
		{
			double damageMultiplier = 1.0;

			damageMultiplier = attack.ElementType.Effectiveness(Reciever.ElementType);

			double damageDealt = damageMultiplier * (((1.5 * Attacker.Level) + 20) * attack.AttackPoints * Attacker.AttackPoints / Reciever.DefensePoints) / 15;
			Console.WriteLine($"{Attacker.Name} used {attack.Name} - {damageDealt}");
			Reciever.HealthPoints -= (int)Double.Round(damageDealt);

		}

		public static Fighter SimulateFight(Fighter player1, Fighter player2)
		{
            Console.WriteLine();
            Console.WriteLine($"Battle Begin");
			Console.WriteLine($"{player1.Name} VS. {player2.Name}");
			bool attacked = false;
            int attackToUse = rnd.Next(0, 4);
            if (player1.Speed <= player2.Speed)
            {
                AttackOpponent(player1, player2, player1.Attacks[attackToUse]);
				attacked = true;
            }
            else
            {
                AttackOpponent(player2, player1, player2.Attacks[attackToUse]);
            }

            while (player1.HealthPoints > 0 && player2.HealthPoints > 0)
			{
                attackToUse = rnd.Next(0, 4);
				if(attacked)
				{
                    AttackOpponent(player2, player1, player2.Attacks[attackToUse]);
					attacked =false;
                } else
				{
                    AttackOpponent(player1, player2, player1.Attacks[attackToUse]);
                    attacked = true;
                }

            }

			Fighter winner = player1.HealthPoints > player2.HealthPoints ? player1 : player2;

			Console.WriteLine("***AND THE WINNER IS***");
			Console.WriteLine($"{winner.Name}");
			Console.WriteLine();

			winner.ResetHealth();


            return winner;
		}
	}
	
}
