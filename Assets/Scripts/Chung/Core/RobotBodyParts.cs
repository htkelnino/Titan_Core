using UnityEngine;

public class RobotBodyPart : MonoBehaviour
{
    public RobotData.PartType bodyPartType;

    [Header("Health Settings")]
    public float maxPartHP = 100f;
    public float currentPartHP;

    [Header("Visual States (Sprites)")]
    private SpriteRenderer spriteRenderer;
    public Sprite healthySprite;  
    public Sprite damagedSprite;  
    public Sprite criticalSprite;  

    [Header("VFX Blueprint")]
    public ParticleSystem sparkVFX; // Gắn Particle tia lửa điện màu cam/vàng vào đây

    private RobotController rootController; 

    void Start()
    {
        currentPartHP = maxPartHP;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rootController = GetComponentInParent<RobotController>();

        if (rootController != null && rootController.robotDataTemplate != null)
        {
            maxPartHP = rootController.robotDataTemplate.GetPartMaxHP(bodyPartType);
            currentPartHP = maxPartHP;
        }

        if (healthySprite != null) spriteRenderer.sprite = healthySprite;
    }

    public void TakePartDamage(float damageAmount)
    {
        if (rootController != null && rootController.isIronWallInvulnerable)
        {
            currentPartHP -= damageAmount;
            currentPartHP = Mathf.Clamp(currentPartHP, 0, maxPartHP);
            if (rootController != null) rootController.TakeGlobalDamage(damageAmount);
            return;
        }

        if (currentPartHP <= 0) return;

        // --- CƠ CHẾ ĐẶC THÙ: PARRY / PHẢN ĐÒN SHADOW MODE ---
        if (rootController != null && rootController.isParryActive)
        {
            Debug.Log($"<color=cyan>PARRY THÀNH CÔNG!</color> ATOM hóa giải hoàn toàn {damageAmount} DMG!");

            // Phần thưởng 1: Hồi lại 100% thanh năng lượng (Pin)
            rootController.currentEnergy = rootController.maxEnergy;

            // Phần thưởng 2: Tăng 20% tốc độ ra đòn tiếp theo (bằng cách giảm thời gian cooldown đòn đánh)
            rootController.TriggerParryReward();
            return;
        }

        float finalDamage = damageAmount;
        if (bodyPartType == RobotData.PartType.Head) finalDamage *= 1.5f; 

        currentPartHP -= finalDamage;
        currentPartHP = Mathf.Clamp(currentPartHP, 0, maxPartHP);

        // Kích hoạt hiệu ứng xẹt tia lửa điện khi trúng đòn
        if (sparkVFX != null) sparkVFX.Play();

        if (rootController != null)
        {
            rootController.TakeGlobalDamage(finalDamage);
            rootController.CheckPartStatus(bodyPartType, currentPartHP / maxPartHP);
        }

        // Cập nhật hình ảnh Sprite dựa trên lượng máu còn lại của vùng
        UpdatePartVisual();
    }

    private void UpdatePartVisual()
    {
        float hpPercentage = currentPartHP / maxPartHP;

        if (hpPercentage <= 0.15f && criticalSprite != null)
        {
            spriteRenderer.sprite = criticalSprite;
            // Bật hiệu ứng xẹt lửa liên tục nếu bị phế
            var emission = sparkVFX.emission;
            emission.rateOverTime = 20f;
        }
        else if (hpPercentage <= 0.50f && damagedSprite != null)
        {
            spriteRenderer.sprite = damagedSprite;
        }
    }
}