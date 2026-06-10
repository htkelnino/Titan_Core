using UnityEngine;

[CreateAssetMenu(fileName = "Skill_ScrapIronHammer", menuName = "Titan Core/Skills/Scrap Iron Hammer")]
public class Skill_ScrapIronHammer : SpecialSkill
{
    public override void Activate(RobotController robot)
    {
        robot.ConsumeEnergy(energyCost);

        float finalDamage = robot.currentAttackDamage * 2.0f;

        AttackState hammerState = new AttackState(robot, "ScrapHammer", 0.6f, 0f, finalDamage);
        robot.TransitionToState(hammerState);

        if (Random.value <= 0.30f)
        {
            robot.forceCriticalPartBreak = true;
            Debug.Log("METRO: Cú bổ búa đã kích hoạt thuộc tính HỦY DIỆT VÙNG ĐẦU!");
        }
    }
}