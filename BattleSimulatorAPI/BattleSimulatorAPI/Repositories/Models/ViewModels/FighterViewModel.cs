using BattleSimulatorAPI.Repositories.Models.DTO;

namespace BattleSimulatorAPI.Repositories.Models.ViewModels
{
    public class FighterViewModel : Fighter
    {
        public int StartingHealth { get; set; }
        public void ResetHealth()
        {
            HealthPoints = StartingHealth;
        }
    }
}
