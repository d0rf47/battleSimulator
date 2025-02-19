using System.ComponentModel.DataAnnotations;

namespace BattleSimulatorAPI.DataLayer.Models.ViewModels
{
    public interface IViewModel : IPoco
    {
		Guid RowId { get; set; }
		bool Disabled { get; set; }
        IEnumerable<ValidationResult> ModelIsValid();
        EntityModel GetEntityModel();
        List<IViewModel> AssociatedEntities();

    }

	public interface IInfoViewModel : IViewModel
	{
	}

	public interface ILookup : IViewModel
	{
		string Code { get; set; }
		string Name { get; set; }
	}
}
