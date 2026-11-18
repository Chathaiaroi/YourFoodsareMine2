using UnityEngine;
using System;

public class QuestFood : FoodItem
{
    public static event Action<QuestFood> OnQuestFoodEaten;

    public override void Eat(Player player)
    {
        Debug.Log($"Cat ate quest food : {foodName}");
        OnQuestFoodEaten?.Invoke(this);
        Consume();
    }

}
