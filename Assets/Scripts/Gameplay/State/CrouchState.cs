using UnityEngine;

public class CrouchState : RobotState
{
    public CrouchState(RobotController robot) : base(robot) { }

    public override void Enter()
    {
        robot.velocity.x = 0f; 
        // Chèn lệnh gọi Animation Crouch tại đây sau này
    }

    public override void LogicUpdate()
    {
        if (robot.isBlocking && !robot.isOverheated)
        {
            robot.TransitionToState(robot.blockState);
            return;
        }
        if (!robot.isCrouching)
        {
            robot.TransitionToState(robot.idleState);
        }
    }
}