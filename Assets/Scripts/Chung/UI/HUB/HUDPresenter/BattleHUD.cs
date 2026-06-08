using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [Header("Target Robot Link")]
    public RobotController targetRobot;

    [Header("Global HP UI")]
    public Slider globalHPSlider;
    public Image hpBufferImage; 
    public float bufferSpeed = 2f;

    [Header("Diagnostic Silhouette UI (5 Parts)")]
    public Image uiHead;
    public Image uiTorso;
    public Image uiLeftArm;
    public Image uiRightArm;
    public Image uiLegs;

    private Color colorHealthy = new Color32(0, 255, 0, 255);   // Xanh lá (>60%)
    private Color colorDamaged = new Color32(255, 165, 0, 255);  // Vàng cam (30% - 60%)
    private Color colorCritical = new Color32(255, 0, 0, 255);   // Đỏ (<30%)
    private Color colorBroken = new Color32(35, 35, 35, 255);    // Đen (0%)

    void Start()
    {
        if (targetRobot != null && targetRobot.robotDataTemplate != null)
        {
            globalHPSlider.maxValue = targetRobot.robotDataTemplate.maxGlobalHP;
            globalHPSlider.value = targetRobot.robotDataTemplate.maxGlobalHP;

            if (hpBufferImage != null)
                hpBufferImage.fillAmount = 1f;
        }
    }

    void Update()
    {
        if (targetRobot == null) return;

        globalHPSlider.value = targetRobot.currentGlobalHP;

        if (hpBufferImage != null)
        {
            float targetFill = targetRobot.currentGlobalHP / globalHPSlider.maxValue;
            hpBufferImage.fillAmount = Mathf.Lerp(hpBufferImage.fillAmount, targetFill, Time.deltaTime * bufferSpeed);
        }

        UpdateSilhouettePart(uiHead, RobotData.PartType.Head);
        UpdateSilhouettePart(uiTorso, RobotData.PartType.Torso);
        UpdateSilhouettePart(uiLeftArm, RobotData.PartType.LeftArm);
        UpdateSilhouettePart(uiRightArm, RobotData.PartType.RightArm);
        UpdateSilhouettePart(uiLegs, RobotData.PartType.Legs);
    }

    // Hàm phụ trách tính toán phần trăm và nhuộm màu ảnh Image
    private void UpdateSilhouettePart(Image partImage, RobotData.PartType partType)
    {
        if (partImage == null) return;

        float currentPartHP = targetRobot.GetPartCurrentHP(partType);
        float maxPartHP = targetRobot.robotDataTemplate.GetPartMaxHP(partType);
        float ratio = currentPartHP / maxPartHP;

        if (ratio <= 0f)
            partImage.color = colorBroken;     // Máu = 0%: Khối màu đen
        else if (ratio < 0.3f)
            partImage.color = colorCritical;   // Máu < 30%: Khối màu đỏ
        else if (ratio < 0.6f)
            partImage.color = colorDamaged;    // 30% < Máu < 60%: Khối màu vàng cam
        else
            partImage.color = colorHealthy;    // Máu > 60%: Khối sáng đèn xanh lá
    }
}