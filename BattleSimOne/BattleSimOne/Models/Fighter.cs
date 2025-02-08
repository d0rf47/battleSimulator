using BattleSimOne.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleSimOne.Models
{
    public class Fighter(int id, TypesEnum elementType, string name, int attackPoints, int defensePoints, int healthPoints, int speed, List<int> attackIds, Dictionary<int, Attack> attackLookup) : IEntity
    {
        public int Id { get; } = id;
        public TypesEnum ElementType { get; set; } = elementType;
        public string Name { get; set; } = name;
        public int AttackPoints { get; set; } = attackPoints;
        public int DefensePoints { get; set; } = defensePoints;
        public int HealthPoints { get; set; } = healthPoints;
        public int StartingHealth { get; set; } = healthPoints;
        public int Speed { get; set; } = speed;
        public int Level { get; set; } = 1;
        public List<Attack> Attacks { get; set; } = attackIds.Select(id => attackLookup.ContainsKey(id) ? attackLookup[id] : null)
                                                          .Where(attack => attack != null)
                                                          .ToList();        

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

        public void ResetHealth()
        {
            HealthPoints = StartingHealth;
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
        public int Speed { get; set; }
        public int Level { get; set; } = 1;
        public List<int> AttackIds { get; set; }
    }
}
