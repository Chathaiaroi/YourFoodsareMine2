using UnityEngine;

public class ReturnState : HumanState
{
    public void Enter(Human human)
    {
        human.SetColor(Color.yellow);
    }

    public void Update(Human human)
    {
        if(human.Detect())
        {
            human.ChangeState(new ChaseState());
            return;
        }

        if(human.MoveToHome())
        {
            human.ChangeState(new IdleState());
        }
    }
}
