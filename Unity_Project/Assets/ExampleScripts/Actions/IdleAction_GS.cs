using GOAP_S.Planning;
using UnityEngine;

public class IdleAction_GS : Action_GS
{
    public float time = 1.0f;
    private float timer = 0.0f;

    public override ACTION_RESULT ActionUpdate()
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            timer = 0.0f;
            return ACTION_RESULT.A_NEXT;
        }
        return ACTION_RESULT.A_CURRENT;
    }
}