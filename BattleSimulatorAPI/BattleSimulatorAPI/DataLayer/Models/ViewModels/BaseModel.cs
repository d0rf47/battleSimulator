using System.ComponentModel.DataAnnotations;

namespace BattleSimulatorAPI.DataLayer.Models.ViewModels
{
	public abstract class BaseModel<TVm> : PocoBase where TVm : IViewModel
	{
		public bool Disabled { get; set; }

		public bool SystemRecord { get; set; }

		public Guid RowId { get; set; } = Guid.NewGuid();

		public IEnumerable<ValidationResult> ModelIsValid()
		{
			var validationResults = new List<ValidationResult>();
			var vc = new ValidationContext(this, null, null);

			Validator.TryValidateObject(this, vc, validationResults, true);

			return validationResults;
		}

		public static EntityModel EntityModel()
		{
			return ViewModelHelpers.GetEntityModel<TVm>();
		}

		public virtual EntityModel GetEntityModel()
		{
			return ViewModelHelpers.GetEntityModel<TVm>();
		}

		public virtual List<IViewModel> AssociatedEntities()
		{
			return new List<IViewModel>();
		}
	}
}
