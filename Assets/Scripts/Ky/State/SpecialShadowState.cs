using UnityEngine;

public class SpecialShadowState : RobotState
{
    private float activeTimer;
    private float totalDuration = 0.5f; // Thời gian hiệu lực của thế thủ bắt chước

    public SpecialShadowState(RobotController robot) : base(robot) { }

    public override void Enter()
    {
        activeTimer = totalDuration;
        robot.velocity.x = 0f; // Khóa di chuyển khi gồng chiêu

        // Tiêu hao đúng 50% Pin theo GDD
        robot.ConsumeEnergy(50f);

        // Kích hoạt trạng thái đặc biệt trên Bộ điều khiển
        robot.isParryActive = true;

        Debug.Log("ATOM kích hoạt SHADOW MODE: Sẵn sàng bắt chước và phản đòn!");
        // Tuần sau chèn: robot.animator.Play("ShadowMode_Enter");
    }

    public override void LogicUpdate()
    {
        activeTimer -= Time.deltaTime;

        // Nếu hết thời gian gồng mà đối thủ không đánh trúng -> Thoát trạng thái
        if (activeTimer <= 0f)
        {
            robot.TransitionToState(robot.idleState);
        }
    }

    public override void Exit()
    {
        // Tắt trạng thái Parry khi thoát khỏi State này
        robot.isParryActive = false;
    }
}