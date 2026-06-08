using UnityEngine;

[CreateAssetMenu(fileName = "Skill_TriadBlitz", menuName = "Titan Core/Skills/Triad Blitz")]
public class Skill_TriadBlitz : SpecialSkill
{
    public override void Activate(RobotController robot)
    {
        robot.ConsumeEnergy(energyCost);

        // Gợi ý: Có thể tạo một State tấn công chuỗi riêng biệt để tối ưu hoạt ảnh
        float damagePerHit = robot.currentAttackDamage * 0.6f;

        Debug.Log("NOISY BOY kích hoạt TRIAD BLITZ: Tung chuỗi 3 đòn liên hoàn, chiếm ưu tiên khung hình!");

        AttackState blitzState = new AttackState(robot, "TriadBlitz", 0.4f, 0f, damagePerHit);
        robot.TransitionToState(blitzState);
    }
}