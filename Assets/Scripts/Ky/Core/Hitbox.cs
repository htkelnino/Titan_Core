using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private Collider2D hitboxCollider;
    private float currentDamage;
    private GameObject owner;
    private bool isUnblockable;
    private string currentAttackName;

    void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
        hitboxCollider.enabled = false;
    }

    // Hàm thiết lập thông số đòn đánh từ AttackState gọi sang
    public void ActivateHitbox(float damage, GameObject robotOwner, bool unblockable = false, string attackName = "")
    {
        currentDamage = damage;
        owner = robotOwner;
        isUnblockable = unblockable;
        currentAttackName = attackName;
        hitboxCollider.enabled = true;
    }

    // Tắt hitbox khi đòn đánh kết thúc
    public void DeactivateHitbox()
    {
        hitboxCollider.enabled = false;
    }

    // Bắt va chạm với các bộ phận trên cơ thể đối phương
    void OnTriggerEnter2D(Collider2D other)
    {
        RobotBodyPart victimPart = other.GetComponent<RobotBodyPart>();

        if (victimPart != null)
        {
            if (other.transform.root.gameObject == owner) return;

            RobotController victimController = other.transform.root.GetComponent<RobotController>();

            if (victimController != null)
            {
                if (victimController.currentStateName == "BlockState" && isUnblockable)
                {
                    Debug.Log("<color=red>ĐÒN XUYÊN GIÁP!</color> Zeus bẻ gãy hoàn toàn phòng thủ của đối phương!");
                    victimPart.TakePartDamage(currentDamage);
                    victimController.TriggerKnockback(Vector2.right * 15f); // Đẩy lùi mạnh (Sẽ code ở Bước 3)
                    DeactivateHitbox();
                    return;
                }
            }

            if (victimController.currentStateName == "BlockState" && !victimController.isCrouching)
            {
                // Nếu đòn tấn công đến từ Skill_LowBlowCombo của Midas
                if (currentAttackName == "LowBlow")
                {
                    Debug.Log("<color=yellow>MIDAS CHƠI BẨN:</color> Đòn tầm thấp xuyên qua thủ đứng!");
                    victimPart.TakePartDamage(currentDamage); // Vẫn dính sát thương chân dù đang đỡ
                    DeactivateHitbox();
                    return;
                }
            }

            RobotController attackerController = owner.GetComponent<RobotController>();
            if (attackerController != null && attackerController.isNextHitCriticalHeadCrack)
            {
                // Nếu trúng vào bộ phận Đầu của đối thủ
                if (victimPart.bodyPartType == RobotData.PartType.Head)
                {
                    victimPart.currentPartHP = 0; 
                    Debug.Log("<color=red>BÚA TẠ PHÁ HỦY HOÀN TOÀN VÙNG ĐẦU ĐỐI THỦ!</color>");
                }
                attackerController.isNextHitCriticalHeadCrack = false; 
            }

            victimPart.TakePartDamage(currentDamage);
            DeactivateHitbox();
        }
    }
}