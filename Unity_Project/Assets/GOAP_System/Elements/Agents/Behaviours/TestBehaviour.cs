using GOAP_S.AI;
using GOAP_S.Tools;

using UnityEngine;

public class TestBehaviour : AgentBehaviour_GS
{
    int k = 9;
    public override void Start()
    {
        Debug.Log("TestB Start!");
        


    }

    public override void Update()
    {

        k += 1;

        SetGoal("test", OperatorType._equal, 0);

        
    }
}
