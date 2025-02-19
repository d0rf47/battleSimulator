using BattleSimulatorAPI.DataLayer.Models.ViewModels;
using BattleSimulatorAPI.Repositories.Models.DTO;

namespace BattleSimulatorAPI.Repositories.Models.ViewModels
{

    public interface IFighterModel : IViewModel
    {
        int HealthPoints { get; set; }
        int StartingHealthPoints { get; set; }
    }
    public class FighterViewModel : BaseModel<FighterViewModel>, IFighterModel
	{
		public int StartingHealthPoints { get; set; }
		public int HealthPoints { get; set; }
		public void ResetHealth()
        {
            HealthPoints = StartingHealthPoints;
        }
    }
}
