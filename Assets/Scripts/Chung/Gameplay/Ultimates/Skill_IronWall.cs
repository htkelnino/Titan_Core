using UnityEngine;

[CreateAssetMenu(fileName = "Skill_IronWall", menuName = "Titan Core/Skills/Iron Wall")]
public class Skill_IronWall : SpecialSkill
{
    public override void Activate(RobotController robot)
    {
        robot.ConsumeEnergy(energyCost);

        robot.TriggerIronWallShield(3.0f);

        robot.TransitionToState(robot.idleState);
    }
}
