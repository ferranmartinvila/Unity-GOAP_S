using GOAP_S.Planning;
using UnityEngine;

public class IdleAction_GS : Action_GS
{
    public override ACTION_RESULT ActionEnd()
    {
        Debug.Log("Idle Action Complete!");
        return base.ActionEnd();
    }
}