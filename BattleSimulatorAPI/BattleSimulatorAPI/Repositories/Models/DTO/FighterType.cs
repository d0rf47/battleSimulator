using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BattleSimulatorAPI.Repositories.Models.DTO
{
    [Table("FighterType")]
    public class FighterType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TypeName { get; set; }

        [MaxLength(256)]
        public string? TypeDesc { get; set; }

        // Navigation Property (if needed)
        //public ICollection<Fighter>? Fighters { get; set; }
    }
}
