using UnityEngine;

[CreateAssetMenu(fileName = "Skill_ShadowMode", menuName = "Titan Core/Skills/Shadow Mode")]
public class Skill_ShadowMode : SpecialSkill
{
    public override void Activate(RobotController robot)
    {
        SpecialShadowState shadowState = new SpecialShadowState(robot);
        robot.TransitionToState(shadowState);
    }
}