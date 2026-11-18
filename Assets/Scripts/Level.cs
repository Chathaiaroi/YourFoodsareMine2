using System;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private List<QuestFood> foods = new List<QuestFood>();   // อาหารที่ต้องทานในด่านนี้
    [SerializeField] private Checkpoint checkpoint;

    private readonly HashSet<QuestFood> eaten = new HashSet<QuestFood>();

    public int EatenCount => eaten.Count;
    public int TotalCount => foods.Count;

    // Observer: UI / GameManager สมัครรับ event
    public event Action<int, int> OnProgressChanged;   // (กินแล้ว, ทั้งหมด)
    public event Action OnLevelCompleted;

    void OnEnable()
    {
        QuestFood.OnQuestFoodEaten += HandleFoodEaten;
    }

    void OnDisable()
    {
        QuestFood.OnQuestFoodEaten -= HandleFoodEaten;
        if (checkpoint != null)
        {
            checkpoint.OnCatReached -= CompleteLevel;
        }
    }

    void Start()
    {
        if (foods.Count == 0)
        {
            Debug.LogWarning("Level: ยังไม่ได้ใส่ QuestFood ในลิสต์ Foods");
        }
        if (checkpoint != null)
        {
            checkpoint.OnCatReached += CompleteLevel;
        }
        else
        {
            Debug.LogWarning("Level: ยังไม่ได้ใส่ Checkpoint");
        }
        OnProgressChanged?.Invoke(EatenCount, TotalCount);
    }

    public bool IsCompleted()
    {
        return foods.Count > 0 && eaten.Count >= foods.Count;
    }

    private void HandleFoodEaten(QuestFood food)
    {
        // นับเฉพาะอาหารที่อยู่ในลิสต์ของด่านนี้ และนับซ้ำไม่ได้
        if (!foods.Contains(food) || !eaten.Add(food)) return;

        Debug.Log($"Food eaten: {EatenCount}/{TotalCount}");
        OnProgressChanged?.Invoke(EatenCount, TotalCount);

        if (IsCompleted() && checkpoint != null)
        {
            checkpoint.Unlock();
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("Im Full!!");
        OnLevelCompleted?.Invoke();
    }
}
