using UnityEngine;

public abstract class SpecialSkill : ScriptableObject
{
    public string skillName;
    public float energyCost = 50f;
    public float duration = 0.5f;

    public abstract void Activate(RobotController robot);
}