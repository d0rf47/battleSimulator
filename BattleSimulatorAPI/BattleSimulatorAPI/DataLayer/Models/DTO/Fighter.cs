using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattleSimulatorAPI.Repositories.Models.DTO
{
    [Table("Fighter")]
    public class Fighter : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int ElementTypeId { get; set; }
        [ForeignKey("ElementTypeId")]
        public ElementType ElementType { get; set; }  // Navigation Property
        public int FighterTypeId { get; set; }
        [ForeignKey("FighterTypeId")]
        public FighterType FighterType { get; set; }  // Navigation Property
        public string Name { get; set; }
        public int AttackPoints { get; set; }
        public int DefensePoints { get; set; }
        public int HealthPoints { get; set; }
        public int Speed { get; set; }
        public int Level { get; set; }        
        public ICollection<FighterAttack> FighterAttacks { get; set; }

        public void PrintAttributes()
        {
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Type: {ElementType.TypeName}");
            Console.WriteLine($"Attack: {AttackPoints}");
            Console.WriteLine($"Defense: {DefensePoints}");
            Console.WriteLine($"Health: {HealthPoints}");
            Console.WriteLine("Attack List");
            foreach (var attack in FighterAttacks)
            {

                attack.PrintAttributes();
            }
        }
        
    }
}
