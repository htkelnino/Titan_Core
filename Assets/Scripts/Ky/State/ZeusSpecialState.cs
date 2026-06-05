using UnityEngine;

public class ZeusSpecialState : RobotState
{
    private float timer;
    private float totalDuration = 1.0f; 
    private float startupTime = 0.8f; 
    private float damage;

    private bool hitboxSpawned = false;

    public ZeusSpecialState(RobotController robot, float damage) : base(robot)
    {
        this.damage = damage;
    }

    public override void Enter()
    {
        timer = totalDuration;
        robot.velocity.x = 0f;
        hitboxSpawned = false;

        Debug.Log("ZEUS ĐANG SẠC PISTON... Ống thủy lực rít lên áp suất cao!");
        // Liên kết Art: robot.animator.Play("Zeus_Piston_Charge");
    }

    public override void LogicUpdate()
    {
        timer -= Time.deltaTime;
        float elapsedTime = totalDuration - timer;

        if (elapsedTime >= startupTime && !hitboxSpawned)
        {
            hitboxSpawned = true;
            Debug.Log("ZEUS XẢ ĐÒN PISTON OVERDRIVE!");

            if (robot.rightFistHitbox != null)
            {
                robot.rightFistHitbox.ActivateHitbox(damage, robot.gameObject, true);
            }
        }

        if (timer <= 0f)
        {
            robot.DeactivateWeaponHitbox();
            robot.TransitionToState(robot.idleState);
        }
    }

    public override void Exit()
    {
        robot.DeactivateWeaponHitbox();
    }
}