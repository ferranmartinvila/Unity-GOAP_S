using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP_S.AI;
using GOAP_S.Tools;

public class TestBhg : AgentBehaviour_GS
{

    public override void Update()
    {
        SetGoal("damage", OperatorType._equal_equal, 10);
    }
}
