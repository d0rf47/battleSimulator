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
        [MaxLength(256)]
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

        /// <summary>
        /// Use for projection onto DTO's 
        /// </summary>
        /// <returns></returns>
        public FighterDto ToDto()
        {
            return new FighterDto
            {
                Id = this.Id,
                Name = this.Name,
                ElementTypeName = this.ElementType?.TypeName,
                FighterTypeName = this.FighterType?.TypeName,
                AttackPoints = this.AttackPoints,
                DefensePoints = this.DefensePoints,
                HealthPoints = this.HealthPoints,
                Speed = this.Speed,
                Level = this.Level,                
                Attacks = this.FighterAttacks?.Select(fa => new AttackDto
                {
                    Id = fa.Attack.Id,
                    Name = fa.Attack.Name,
                    AttackPoints = fa.Attack.AttackPoints,
                    CoolDown = fa.Attack.CoolDown,
                    ElementTypeName = fa.Attack.ElementType?.TypeName
                }).ToList()
            };
        }
    }

    public class FighterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ElementTypeName { get; set; }
        public string FighterTypeName { get; set; }
        public int AttackPoints { get; set; }
        public int DefensePoints { get; set; }
        public int HealthPoints { get; set; }
        public int Speed { get; set; }
        public int Level { get; set; }
        public List<AttackDto> Attacks { get; set; }
    }
}
