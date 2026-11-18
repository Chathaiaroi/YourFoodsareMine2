using UnityEngine;

public class IdleState : HumanState
{
   public void Enter(Human human)
    {
        human.SetColor(Color.green);
    }

    public void Update(Human human)
    {
        if(human.Detect())
        {
            human.ChangeState(new ChaseState());
        }
    }
}
