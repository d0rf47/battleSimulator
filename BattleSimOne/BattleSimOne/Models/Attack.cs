using BattleSimOne.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleSimOne.Models
{
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
}
