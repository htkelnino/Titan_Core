using UnityEngine;

[CreateAssetMenu(fileName = "Skill_LowBlowCombo", menuName = "Titan Core/Skills/Low Blow Combo")]
public class Skill_LowBlowCombo : SpecialSkill
{
    public override void Activate(RobotController robot)
    {
        robot.ConsumeEnergy(energyCost);

        Debug.Log("MIDAS kích hoạt LOW BLOW COMBO: Đấm móc sườn và đá quét trụ chơi bẩn!");

        // Bạn cần cập nhật script Hitbox.cs ở các tuần sau để check: 
        // Nếu đòn đánh là "Low" và đối thủ đang ở BlockState thông thường (không phải Duck Block) -> Vẫn dính sát thương chân!
        AttackState lowAttackState = new AttackState(robot, "LowBlow", 0.45f, 0f, robot.currentAttackDamage * 1.3f);
        robot.TransitionToState(lowAttackState);
    }
}