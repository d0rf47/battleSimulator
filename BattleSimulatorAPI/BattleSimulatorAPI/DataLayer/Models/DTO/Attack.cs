using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattleSimulatorAPI.Repositories.Models.DTO
{

    //CoolDown = 5 + (AttackPoints - 30) * (45 / 70)

    [Table("Attack")]
    public class Attack : IEntity
    {
        public int Id { get; set;  }
        public int ElementTypeId { get; set; }  // Foreign Key
        [ForeignKey("ElementTypeId")]
        public ElementType ElementType { get; set; }  // Navigation Property
        [MaxLength(256)]
        public string Name { get; set; }
        public int AttackPoints { get; set; }
        public int CoolDown { get; set; }
        public ICollection<FighterAttack> FighterAttacks { get; set; }

        public void PrintAttributes()
        {
            Console.WriteLine($"{Name} - {AttackPoints}AP -- {ElementType.TypeName} Type, {CoolDown} seconds cooldown");
        }        
    }
    public class AttackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AttackPoints { get; set; }
        public int CoolDown { get; set; }
        public string ElementTypeName { get; set; }
    }
}
