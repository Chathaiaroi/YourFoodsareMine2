using UnityEngine;

public class ChaseState : HumanState
{
    private float lostTimer;

    public void Enter(Human human)
    {
        lostTimer = 0f;
        human.SetColor(Color.red);
    }

    public void Update(Human human)
    {
        if(human.Detect())
        {
            lostTimer = 0f;
        }
        else
        {
            lostTimer += Time.deltaTime;
            if(lostTimer >= human.LoseSightTime)
            {
                human.ChangeState(new ReturnState());
                return;
            }
        }

        human.Chase();
        if(human.IsCloseToCat)
        {
            human.CatchCat();
        }
    }
}

