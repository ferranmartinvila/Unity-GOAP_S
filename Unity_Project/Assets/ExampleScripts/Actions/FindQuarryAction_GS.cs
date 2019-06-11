using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP_S.Planning;

public class FindQuarryAction_GS : Action_GS
{
    public bool diamond = false;
    public bool rock = false;

    public override void CopyValues(Action_GS copy)
    {
        diamond = ((FindQuarryAction_GS)copy).diamond;
        rock = ((FindQuarryAction_GS)copy).rock;

        base.CopyValues(copy);
    }

    public override ACTION_RESULT ActionUpdate()
    {
        return ACTION_RESULT.A_CURRENT;
    }
}
