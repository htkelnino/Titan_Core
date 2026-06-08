using UnityEngine;

[CreateAssetMenu(fileName = "Skill_SynchronizedAssault", menuName = "Titan Core/Skills/Synchronized Assault")]
public class Skill_SynchronizedAssault : SpecialSkill
{
    public override void Activate(RobotController robot)
    {
        robot.ConsumeEnergy(energyCost);

        Debug.Log("TWIN CITIES kích hoạt SYNCHRONIZED ASSAULT: Đòn đánh kép từ 2 CPU hoạt động độc lập!");

        AttackState doublePunchState = new AttackState(robot, "DoublePunch", 0.5f, 0f, robot.currentAttackDamage * 1.8f);
        robot.TransitionToState(doublePunchState);
    }
}