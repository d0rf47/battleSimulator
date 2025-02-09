using BattleSimulatorAPI.Repositories.Models.DTO;

namespace BattleSimulatorAPI.Repositories.Models.ViewModels
{
    public class AttackViewModel : Attack
    {
        Dictionary<int, int> AttackCoolDowns = new Dictionary<int, int>();
    }
}
