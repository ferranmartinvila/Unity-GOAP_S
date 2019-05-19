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
        Debug.Log(blackboard.GetVariable<float>("rere").value);

        blackboard.SetVariable<int>("yte", blackboard.GetVariable<int>("yte").value + 1);

        SetGoal("in_place", OperatorType._equal_equal,true);
        SetGoal("damage", OperatorType._equal_equal, 1);
        SetGoal("speed", OperatorType._equal_equal, 1);

    }

    public override void InActionUpdate()
    {
        //In idle
        //Recalculate when needed?
    }
}
