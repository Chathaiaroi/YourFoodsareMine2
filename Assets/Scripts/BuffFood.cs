using UnityEngine;
using UnityEngine.UIElements;

public class BuffFood : FoodItem
{
    [SerializeField] private float StaminaRestore = 40f;

    public override void Eat(Player player)
    {
        Debug.Log($"Cat ate buff food: {foodName} (+ {StaminaRestore} stamina)");
        player.Stamina.Restore(StaminaRestore);
        Consume();
    }
}
