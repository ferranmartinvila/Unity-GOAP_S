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
        SetGoal("attack", OperatorType._equal_equal, global_blackboard.GetValue<int>("max_attack"));
        SetGoal("energy", OperatorType._equal_equal, global_blackboard.GetValue<int>("max_energy"));
    }
}
