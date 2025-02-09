using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BattleSimulatorAPI.Repositories.Models.DTO
{

    [Table("ElementType")]
    public class ElementType
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
