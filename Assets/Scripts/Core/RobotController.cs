using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class RobotController : MonoBehaviour
{
    [Header("Robot Data")]
    public RobotData robotDataTemplate;

    [HideInInspector] public float currentGlobalHP;
    [HideInInspector] public float currentMoveSpeed;
    [HideInInspector] public float currentAttackDamage;
    [HideInInspector] public bool isParryActive = false;
    [HideInInspector] public bool isNextHitCriticalHeadCrack = false;

    [Header("Physics Settings")]
    public float moveSpeed = 5f;       
    public float customGravity = -20f;

    [Header("Energy System")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyRegenRate = 15f;

    [Header("Overheat State")]
    public bool isOverheated = false;
    public float overheatDuration = 2.5f;
    private float overheatTimer = 0f;

    [Header("State Info")]
    public string currentStateName;

    [Header("Combat References")]
    public Hitbox rightFistHitbox;

    [Header("Combat Status Flags (Generic Buffs/Debuffs)")]
    [HideInInspector] public bool canParryNextHit = false;
    [HideInInspector] public bool forceCriticalPartBreak = false;
    [HideInInspector] public bool isPartImmune = false;
    private float partImmunityTimer = 0f;

    public Rigidbody2D rb { get; private set; }
    public BoxCollider2D groundCollider { get; private set; }
    public Vector2 velocity;

    public float moveInput { get; private set; }
    public bool isCrouching { get; private set; }
    public bool isGrounded { get; private set; }
    public bool isBlocking { get; private set; }

    // State Machine
    private RobotState currentState;
    public IdleState idleState { get; private set; }
    public WalkState walkState { get; private set; }
    public CrouchState crouchState { get; private set; }
    public BlockState blockState { get; private set; }
    public OverheatState overheatState { get; private set; }
    public SpecialShadowState specialShadowState { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; 
        groundCollider = GetComponent<BoxCollider2D>();

        if (robotDataTemplate != null)
        {
            currentGlobalHP = robotDataTemplate.maxGlobalHP;
            currentMoveSpeed = robotDataTemplate.moveSpeed;
            currentAttackDamage = robotDataTemplate.baseDamage;
            currentEnergy = robotDataTemplate.maxEnergy;
            gameObject.name = "Robot_" + robotDataTemplate.robotName;
        }
        else
        {
            Debug.LogError("Chưa gắn Robot Data cho " + gameObject.name);
        }

        currentEnergy = maxEnergy;

        idleState = new IdleState(this);
        walkState = new WalkState(this);
        crouchState = new CrouchState(this);
        blockState = new BlockState(this);
        overheatState = new OverheatState(this);

        TransitionToState(idleState);
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isCrouching = Input.GetKey(KeyCode.S);
        isBlocking = Input.GetKey(KeyCode.L);

        ManageEnergySystem();
        ManageStatusEffects();

        if (currentState != null) currentState.LogicUpdate();

        if (!isOverheated && currentStateName != "AttackState")
        {
            HandleAttacks();
        }

        bool isAttackCancelable = false;
        if (currentState is AttackState currentAttack)
        {
            isAttackCancelable = currentAttack.canCancel;
        }

        if (Input.GetKeyDown(KeyCode.P) && !isOverheated)
        {
            if (currentStateName != "AttackState" || isAttackCancelable)
            {
                if (robotDataTemplate != null && robotDataTemplate.uniqueSkill != null)
                {
                    if (currentEnergy >= robotDataTemplate.uniqueSkill.energyCost)
                    {
                        Debug.Log("<color=cyan>CANCEL COMBO KÍCH HOẠT!</color> Hủy đòn thường nối Tuyệt chiêu!");
                        robotDataTemplate.uniqueSkill.Activate(this);
                    }
                    else
                    {
                        Debug.Log("Không đủ năng lượng PIN để Cancel nối Tuyệt chiêu!");
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        CheckGrounded();
        ApplyCustomGravity();

        if (currentState != null) currentState.PhysicsUpdate();

        rb.linearVelocity = velocity;
    }

    // Hàm chuyển đổi trạng thái
    public void TransitionToState(RobotState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentStateName = currentState.ToString(); 
        currentState.Enter();
    }

    private void ManageEnergySystem()
    {
        if (isOverheated)
        {
            overheatTimer -= Time.deltaTime;
            if (overheatTimer <= 0)
            {
                isOverheated = false;
                currentEnergy = maxEnergy * 0.5f; 
                TransitionToState(idleState);    
            }
            return; 
        }

        // Hồi pin nếu không ở trạng thái tung đòn (sau này) và không Block
        if (currentState != blockState)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }
    }

    // Hàm gọi khi bị trừ pin (Đỡ đòn trúng, dùng chiêu)
    public void ConsumeEnergy(float amount)
    {
        if (isOverheated) return;

        currentEnergy -= amount;
        if (currentEnergy <= 0)
        {
            currentEnergy = 0;
            isOverheated = true;
            overheatTimer = overheatDuration;
            TransitionToState(overheatState); 
            Debug.Log("OVERHEAT! ROBOT CHẬP ĐIỆN!");
        }
    }

    // Bắn tia Raycast để kiểm tra mặt đất và vẽ tia đỏ để Debug
    private void CheckGrounded()
    {
        float rayLength = 0.15f;
        Vector2 origin = new Vector2(groundCollider.bounds.center.x, groundCollider.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, LayerMask.GetMask("Ground"));
        isGrounded = hit.collider != null;
        if (isGrounded && velocity.y < 0f)
        {
            Vector2 snapPosition = rb.position;
            snapPosition.y += (hit.point.y - origin.y);
            rb.position = snapPosition;
        }
    }

    // Hàm mô phỏng trọng lực cho Kinematic
    private void ApplyCustomGravity()
    {
        if (!isGrounded)
        {
            velocity.y += customGravity * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y = 0f;
        }
    }

    // Hàm kiểm tra HP tổng và xử lý KO
    public void TakeGlobalDamage(float amount)
    {
        if (currentGlobalHP <= 0) return;

        currentGlobalHP -= amount;
        if (currentGlobalHP <= 0)
        {
            currentGlobalHP = 0;
            TriggerKO();
        }
    }

    // Hàm kiểm tra tình trạng từng bộ phận và áp dụng hiệu ứng/phạt tương ứng
    public void CheckPartStatus(RobotData.PartType part, float hpRatio)
    {
        switch (part)
        {
            case RobotData.PartType.Head:
                if (hpRatio <= 0f) 
                {
                    Debug.Log("VÙNG ĐẦU BỊ PHÁ HỦY: Áp hiệu ứng Shader Glitch lên màn hình!");
                    // Gợi ý: Kích hoạt UI Shader Glitch tại đây
                }
                break;

            case RobotData.PartType.LeftArm:
                if (hpRatio <= 0f)
                {
                    Debug.Log("TAY TRÁI BỊ PHẾ: Vô hiệu hóa phím đấm tương ứng, giảm 50% DMG!");
                    currentAttackDamage = robotDataTemplate.baseDamage * 0.5f;
                }
                break;

            case RobotData.PartType.RightArm:
                if (hpRatio <= 0f)
                {
                    Debug.Log("TAY PHẢI BỊ PHẾ: Vô hiệu hóa phím đấm tương ứng, giảm 50% DMG!");
                    currentAttackDamage = robotDataTemplate.baseDamage * 0.5f;
                }
                break;

            case RobotData.PartType.Legs:
                if (hpRatio <= 0f)
                {
                    Debug.Log("VÙNG CHÂN BỊ PHÁ HỦY: Giảm 60% tốc chạy, khóa phím Nhảy (W)!");
                    currentMoveSpeed = robotDataTemplate.moveSpeed * 0.4f; 
                }
                break;

            case RobotData.PartType.Torso:
                if (hpRatio <= 0f)
                {
                    TriggerKO(); 
                }
                break;
        }
    }

    private void TriggerKO()
    {
        Debug.Log(gameObject.name + " LÕI NĂNG LƯỢNG BỊ NỔ TUNG! KNOCK-OUT!");
        TransitionToState(overheatState); // Tạm thời đưa về trạng thái vô lực trước khi có Anim chết
    }

    private void HandleAttacks()
    {
        float baseDmg = robotDataTemplate != null ? currentAttackDamage : 35f;

        if (Input.GetKeyDown(KeyCode.H))
        {
            AttackState lightPunch = new AttackState(this, "LightPunch", 0.25f, 8f, baseDmg);
            TransitionToState(lightPunch);
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            AttackState mediumPunch = new AttackState(this, "MediumPunch", 0.3f, 12f, baseDmg);
            TransitionToState(mediumPunch);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            AttackState heavyPunch = new AttackState(this, "HeavyPunch", 0.5f, 16f, baseDmg * 1.5f);
            TransitionToState(heavyPunch);
        }
    }
    // Hai hàm trung gian để State gọi điều khiển thông qua Robot gốc
    public void ActivateWeaponHitbox(float damageValue, bool unblockable = false, string attackName = "")
    {
        if (rightFistHitbox != null)
        {
            rightFistHitbox.ActivateHitbox(damageValue, this.gameObject, unblockable, attackName);
        }
    }

    public void DeactivateWeaponHitbox()
    {
        if (rightFistHitbox != null)
        {
            rightFistHitbox.DeactivateHitbox();
        }
    }

    // Hàm truy xuất máu vùng dành cho UI
    public float GetPartCurrentHP(RobotData.PartType partType)
    {
        RobotBodyPart[] parts = GetComponentsInChildren<RobotBodyPart>();
        foreach (RobotBodyPart part in parts)
        {
            if (part.bodyPartType == partType)
            {
                return part.currentPartHP;
            }
        }
        return 0f;
    }

    public void TriggerParryReward()
    {
        TransitionToState(idleState);

        // Tạo một hiệu ứng phát sáng Neon Xanh Dương rực lên báo hiệu Parry thành công
        Debug.Log("ATOM nhận buff: Tăng tốc độ phản công chớp nhoáng!");
    }

    // Hàm xử lý lực đẩy lùi kinematic
    public void TriggerKnockback(Vector2 knockbackForce)
    {
        velocity.x = knockbackForce.x;

        // Gợi ý: Có thể tạo một HitStunState riêng để khóa điều khiển của nạn nhân khi bị đẩy văng
        Debug.Log(gameObject.name + " BỊ ĐẨY VĂNG RA XA!");
    }

    // HÀM QUẢN LÝ BỘ ĐẾM THỜI GIAN BUFF CHUNG
    private void ManageStatusEffects()
    {
        if (isPartImmune)
        {
            partImmunityTimer -= Time.deltaTime;
            if (partImmunityTimer <= 0f)
            {
                isPartImmune = false;
                Debug.Log(gameObject.name + " đã hết hiệu lực Miễn nhiễm linh kiện!");
            }
        }
        // Sau này có hiệu ứng nào cần đếm thời gian, bạn chỉ cần ném vào đây
    }

    // HÀM CẤP BUFF
    public void GrantPartImmunity(float duration)
    {
        isPartImmune = true;
        partImmunityTimer = duration;
    }
}
