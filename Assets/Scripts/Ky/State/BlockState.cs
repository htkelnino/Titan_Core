using UnityEngine;

public class BlockState : RobotState
{
    public BlockState(RobotController robot) : base(robot) { }

    public override void Enter()
    {
        robot.velocity.x = 0f; 

        // Chuẩn bị cho Animation
        if (robot.isCrouching)
        {
            Debug.Log("Trạng thái: Đỡ đòn thấp (Duck Block)");
            // Kích hoạt Anim Duck Block
        }
        else
        {
            Debug.Log("Trạng thái: Đỡ đòn cao (High Block)");
            // Kích hoạt Anim High Block
        }
    }

    public override void LogicUpdate()
    {
        if (!robot.isBlocking)
        {
            if (robot.isCrouching)
                robot.TransitionToState(robot.crouchState);
            else
                robot.TransitionToState(robot.idleState);
        }
    }
}