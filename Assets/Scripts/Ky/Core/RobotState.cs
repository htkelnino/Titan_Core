using UnityEngine;

public abstract class RobotState 
{
    protected RobotController robot;

    protected RobotState(RobotController robot)
    {
        this.robot = robot;
    }

    public virtual void Enter() { }

    public virtual void LogicUpdate() { }

    public virtual void PhysicsUpdate() { }

    public virtual void Exit() { }
}
