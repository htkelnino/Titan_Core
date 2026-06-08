using UnityEngine;

public class OverheatState : RobotState
{
    public OverheatState(RobotController robot) : base(robot) { }

    public override void Enter()
    {
        robot.velocity.x = 0f;
        // Kích hoạt VFX nhấp nháy đỏ hoặc Shader Glitch tại đây
    }

    public override void LogicUpdate()
    {
        if (Mathf.Abs(robot.moveInput) < 0.01f && !robot.isCrouching)
        {
            robot.velocity.x = 0f;
        }
    }

    public override void PhysicsUpdate()
    {
        float penalizedSpeed = robot.moveSpeed * 0.2f;
        if (!robot.isCrouching)
        {
            robot.velocity.x = robot.moveInput * penalizedSpeed;
        }
    }

    public override void Exit()
    {
        Debug.Log("ĐÃ KHÔI PHỤC NĂNG LƯỢNG!");
    }
}