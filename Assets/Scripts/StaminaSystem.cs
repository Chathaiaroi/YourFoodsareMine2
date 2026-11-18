using System;
using UnityEngine;

[Serializable] public class StaminaSystem
{
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float drainPerSecond = 25f;
    [SerializeField] private float recoverPerSecond = 15f;
    [SerializeField, Range(0f, 1f)] private float exhaustRecoverRatio = 0.2f;

    private float current;
    private bool exhausted;

    public float Current => current;
    public float Max => maxStamina;
    public bool CanRun => !exhausted;

    // Observer: UI สมัครรับ event เพื่อวาดหลอด stamina
    public event Action<float, float> OnChanged;
    public event Action OnExhausted;

    public void Init()
    {
        current = maxStamina;
        exhausted = false;
        OnChanged?.Invoke(current, maxStamina);
    }

    public void Tick(bool running, float deltaTime)
    {
        float before = current;

        if (running)
        {
            current = Mathf.Max(0f, current - drainPerSecond * deltaTime);
            if (current <= 0f && !exhausted)
            {
                exhausted = true;
                OnExhausted?.Invoke();
            }
        }
        else
        {
            current = Mathf.Min(maxStamina, current + recoverPerSecond * deltaTime);
            CheckRecovered();
        }

        if (!Mathf.Approximately(before, current))
        {
            OnChanged?.Invoke(current, maxStamina);
        }
    }
    public void Restore(float amount)
    {
        current = Mathf.Min(maxStamina, current + amount);
        CheckRecovered();
        OnChanged?.Invoke(current, maxStamina);
    }

    private void CheckRecovered()
    {
        if (exhausted && current >= maxStamina * exhaustRecoverRatio)
        {
            exhausted = false;
        }
    }
}

