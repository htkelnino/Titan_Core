using UnityEngine;

public class WalkState : RobotState
{
    public WalkState(RobotController robot) : base(robot) { }

    public override void LogicUpdate()
    {
        if (robot.isBlocking && !robot.isOverheated)
        {
            robot.TransitionToState(robot.blockState);
            return;
        }
        if (robot.isCrouching)
        {
            robot.TransitionToState(robot.crouchState);
        }
        else if (Mathf.Abs(robot.moveInput) < 0.01f)
        {
            robot.TransitionToState(robot.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        robot.velocity.x = robot.moveInput * robot.moveSpeed;
    }
}