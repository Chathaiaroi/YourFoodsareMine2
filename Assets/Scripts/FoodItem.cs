using UnityEngine;

public abstract class FoodItem : MonoBehaviour
{
    [SerializeField] protected string foodName = "Food";

    public string FoodName => FoodName;
    public abstract void Eat(Player player);

    protected void Consume()
    {
        Destroy(gameObject);
    }
}
