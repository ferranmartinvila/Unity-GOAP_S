using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP_S.Planning;

public class FindBarrelAction_GS : Action_GS
{
    public float time = 0.2f;
    private float timer = 0.0f;

    public override ACTION_RESULT ActionUpdate()
    {
        Debug.Log(agent.name);

        timer += Time.deltaTime;
        if (timer > time)
        {
            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }
}
