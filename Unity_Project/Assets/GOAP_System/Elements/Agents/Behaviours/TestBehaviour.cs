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
        SetGoal("damage", OperatorType._equal_equal, 5);
        SetGoal("defence", OperatorType._equal_equal, 8);
    }
}
