using UnityEngine;

[CreateAssetMenu(fileName = "NewRobotData", menuName = "Titan Core/Robot Data")]
public class RobotData : ScriptableObject
{
    [Header("General Info")]
    public string robotName;
    [TextArea] public string description;

    [Header("Core Stats")]
    public float maxGlobalHP;
    public float moveSpeed;
    public float baseDamage;

    [Header("Energy Stats")]
    public float maxEnergy = 100f;
    public float energyRegenRate = 15f;

    [Header("Special Skill Setting")]
    public SpecialSkill uniqueSkill;

    public enum PartType { Head, Torso, LeftArm, RightArm, Legs }

    public float GetPartMaxHP(PartType partType)
    {
        switch (partType)
        {
            case PartType.Head: return maxGlobalHP * 0.20f; 
            case PartType.Torso: return maxGlobalHP * 0.35f; 
            case PartType.LeftArm: return maxGlobalHP * 0.15f; 
            case PartType.RightArm: return maxGlobalHP * 0.15f; 
            case PartType.Legs: return maxGlobalHP * 0.15f; 
            default: return 0f;
        }
    }
}