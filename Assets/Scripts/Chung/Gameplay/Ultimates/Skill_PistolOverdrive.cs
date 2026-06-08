using UnityEngine;

[CreateAssetMenu(fileName = "Skill_PistolOverdrive", menuName = "Titan Core/Skills/Pistol Overdrive")]
public class Skill_PistolOverdrive : SpecialSkill
{
    public override void Activate(RobotController robot)
    {
        robot.ConsumeEnergy(energyCost);

        float specialDamage = robot.currentAttackDamage * 2.5f;

        ZeusSpecialState zeusState = new ZeusSpecialState(robot, specialDamage);
        robot.TransitionToState(zeusState);
    }
}