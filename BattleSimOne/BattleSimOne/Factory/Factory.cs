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

	public class Fighter  (int id, TypesEnum elementType, string name, int attackPoints, int defensePoints, int healthPoints, List<int> attackIds, Dictionary<int, Attack> attackLookup) : IEntity
	{
		public int Id { get; } = id;
		public TypesEnum ElementType { get; set; } = elementType;
		public string Name { get; set; } = name;
		public int AttackPoints { get; set; } = attackPoints;
		public int DefensePoints { get; set; } = defensePoints;
		public int HealthPoints { get; set; } = healthPoints;
		public List<Attack> Attacks { get; set; } = attackIds.Select(id => attackLookup.ContainsKey(id) ? attackLookup[id] : null)
														  .Where(attack => attack != null)
														  .ToList();

		public void AttackOpponent(Fighter opponent)
		{
			
		}

		public void PrintAttributes()
		{
			Console.WriteLine($"Name: {Name}");
			Console.WriteLine($"Type: {ElementType}");
			Console.WriteLine($"Attack: {AttackPoints}");
			Console.WriteLine($"Defense: {DefensePoints}");
			Console.WriteLine($"Health: {HealthPoints}");
			Console.WriteLine("Attack List");
			foreach (var attack in Attacks)
			{

				attack.PrintAttributes();
			}
		}
	}

	public class FighterJson
	{
		public int Id { get; set; }
		public TypesEnum ElementType { get; set; }
		public string Name { get; set; }
		public int AttackPoints { get; set; }
		public int DefensePoints { get; set; }
		public int HealthPoints { get; set; }
		public List<int> AttackIds { get; set; }
	}

	public class Attack(int id, TypesEnum elementType, string name, int attackPoints) : IEntity 
	{
		public int Id { get; } = id;
		public TypesEnum ElementType { get; set; } = elementType;
		public string Name { get; set; } = name;
		public int AttackPoints { get; set; } = attackPoints;

		public void PrintAttributes()
		{
			Console.WriteLine($"{Name} - {AttackPoints}AP -- {ElementType} Type");
		}
	}
	public abstract class Factory
	{
	}

	public static class BattleSimulator
	{
		
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

		public static List<Fighter> LoadFighter()
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
	}
	
}
