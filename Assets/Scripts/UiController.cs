using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Observer: UI ไม่ต้องถูกเรียกจาก Cat/Level โดยตรง แต่สมัครรับ event แล้ววาดเอง
public class UiController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private Level level;

    [Header("Stamina Bar")]
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image staminaFill;          // ไม่ใส่ก็ได้ (ไว้เปลี่ยนสีตอนใกล้หมด)
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowColor = Color.red;
    [SerializeField, Range(0f, 1f)] private float lowThreshold = 0.2f;

    [Header("Food Progress")]
    [SerializeField] private TextMeshProUGUI foodText;

    void Start()
    {
        if (player != null)
        {
            player.Stamina.OnChanged += UpdateStamina;
            // วาดค่าเริ่มต้นทันที เผื่อ event ยิงไปก่อนที่เราจะสมัครรับ
            UpdateStamina(player.Stamina.Current, player.Stamina.Max);
        }
        else
        {
            Debug.LogWarning("UiController: ยังไม่ได้ใส่ Cat");
        }

        if (level != null)
        {
            level.OnProgressChanged += UpdateFood;
            UpdateFood(level.EatenCount, level.TotalCount);
        }
        else
        {
            Debug.LogWarning("UiController: ยังไม่ได้ใส่ Level");
        }
    }

    void OnDestroy()
    {
        if (player != null)
        {
            player.Stamina.OnChanged -= UpdateStamina;
        }
        if (level != null)
        {
            level.OnProgressChanged -= UpdateFood;
        }
    }

    private void UpdateStamina(float current, float max)
    {
        if (staminaSlider == null || max <= 0f) return;

        float ratio = current / max;
        staminaSlider.value = ratio;

        if (staminaFill != null)
        {
            staminaFill.color = ratio <= lowThreshold ? lowColor : normalColor;
        }
    }

    private void UpdateFood(int eaten, int total)
    {
        if (foodText == null) return;

        string text = $"Food: {eaten}/{total}";
        if (total > 0 && eaten >= total)
        {
            text += "\nGo to the checkpoint!";
        }
        foodText.text = text;
    }
}
