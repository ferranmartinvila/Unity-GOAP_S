using GOAP_S.Planning;
using UnityEngine;

public class TestAction_GS : Action_GS
{
    public float time = 1.0f;
    private float timer = 0.0f;

    public override ACTION_RESULT ActionUpdate()
    {
        Debug.Log(agent.name);

        timer += Time.deltaTime;
        if(timer > time)
        {
            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }
}
