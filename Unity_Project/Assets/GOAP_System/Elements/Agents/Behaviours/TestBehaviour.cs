using GOAP_S.AI;
using GOAP_S.Tools;

using UnityEngine;

public class TestBehaviour : AgentBehaviour_GS
{
    public override void Start()
    {
        Debug.Log("TestB Start!");
        


    }

    public override void Update()
    {

        SetGoal("test",OperatorType._is_equal, 10);

        
    }
}
