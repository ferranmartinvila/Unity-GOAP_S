using GOAP_S.AI;
using GOAP_S.Tools;

using UnityEngine;

public class TestBehaviour : AgentBehaviour_GS
{
    public override void Start()
    {
        Debug.Log("Test Start!");
    }

    public override void Update()
    {
        SetGoal("in_place", OperatorType._equal_equal,true);
        SetGoal("melon", OperatorType._equal_equal, 1);
        SetGoal("sandia", OperatorType._equal_equal, 1);

    }

    public override void InActionUpdate()
    {
        //In idle
        //Recalculate when needed?
    }
}
