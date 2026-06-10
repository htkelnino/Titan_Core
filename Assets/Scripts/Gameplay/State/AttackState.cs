using UnityEngine;

public class AttackState : RobotState
{
    private string attackAnimName;
    private float attackDuration;
    private float timer;
    private float energyCost;
    private float damage;

    private float startupTime;
    private float activeTime;
    private bool hitboxActivated = false;
    private bool hitboxDeactivated = false;
    public bool canCancel { get; private set; }
    public AttackState(RobotController robot, string animName, float duration, float energyCost, float damage) : base(robot)
    {
        this.attackAnimName = animName;
        this.attackDuration = duration;
        this.energyCost = energyCost;
        this.damage = damage;

        this.startupTime = duration * 0.3f;
        this.activeTime = duration * 0.7f;
    }

    public override void Enter()
    {
        timer = attackDuration;
        robot.velocity.x = 0f;

        robot.ConsumeEnergy(energyCost);

        hitboxActivated = false;
        hitboxDeactivated = false;
        canCancel = false;

        Debug.Log($"Tung đòn: {attackAnimName} | Tốn {energyCost}% PIN");
        // Chèn sau: robot.animator.Play(attackAnimName);
    }

    public override void LogicUpdate()
    {
        timer -= Time.deltaTime;
        float elapsedTime = attackDuration - timer;

        // Giai đoạn Active: Bật Hitbox
        if (elapsedTime >= startupTime && !hitboxActivated)
        {
            hitboxActivated = true;
            robot.ActivateWeaponHitbox(damage, false, attackAnimName);
        }

        // --- CƠ CHẾ CANCEL: Bật cờ cho phép Hủy đòn ngay khi vừa chạm giai đoạn Active ---
        if (elapsedTime >= startupTime)
        {
            canCancel = true;
        }

        // Giai đoạn Recovery: Tắt Hitbox
        if (elapsedTime >= activeTime && !hitboxDeactivated)
        {
            hitboxDeactivated = true;
            robot.DeactivateWeaponHitbox();
        }

        // Kết thúc đòn đánh
        if (timer <= 0f)
        {
            if (robot.isCrouching) robot.TransitionToState(robot.crouchState);
            else robot.TransitionToState(robot.idleState);
        }
    }

    public override void Exit()
    {
        robot.DeactivateWeaponHitbox();
    }
}