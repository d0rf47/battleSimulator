using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BattleSimulatorAPI.Repositories.Models.DTO
{
    [Table("FighterAttack")]
    public class FighterAttack : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int FighterId { get; set; } // Foreign Key
        public Fighter Fighter { get; set; } // Navigation Property

        public int AttackId { get; set; } // Foreign Key
        public Attack Attack { get; set; } // Navigation Property

        public void PrintAttributes()
        {
            Console.WriteLine($"{Attack.Name}:  {Attack.AttackPoints} AP - Cooldown: {Attack.CoolDown} seconds, Type {Attack.ElementType.TypeName}");
        }
    }

}
