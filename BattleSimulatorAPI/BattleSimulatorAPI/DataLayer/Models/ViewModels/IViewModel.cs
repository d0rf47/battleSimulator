using System.ComponentModel.DataAnnotations;

namespace BattleSimulatorAPI.DataLayer.Models.ViewModels
{
    public interface IViewModel
    {
        bool Disabled { get; set; }
        IEnumerable<ValidationResult> ModelIsValid();
        EntityModel GetEntityModel();
        List<IViewModel> AssociatedEntites();
    }
}
