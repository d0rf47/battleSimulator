using BattleSimulatorAPI.DataLayer.Models.ViewModels;
using BattleSimulatorAPI.Repositories.Models.DTO;

namespace BattleSimulatorAPI.Repositories.Models.ViewModels
{

    public interface IFighterModel :IViewModel
    {
        
    }
    public class FighterViewModel : Fighter
    {
        public int StartingHealth { get; set; }
        public void ResetHealth()
        {
            HealthPoints = StartingHealth;
        }
    }
}
