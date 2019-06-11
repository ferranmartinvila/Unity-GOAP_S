using GOAP_S.AI;
using GOAP_S.Tools;
using UnityEngine;


public class RecollectorBehaviour : AgentBehaviour_GS
{
    public override void Update()
    {
        if (global_blackboard.GetValue<int>("current_rock") < global_blackboard.GetValue<int>("rock_goal"))
        {
            SetGoal("current_rock", OperatorType._bigger_or_equal, global_blackboard.GetValue<int>("rock_goal"), VariableLocation._global);
        }
        /*if(global_blackboard.GetValue<int>("current_diamond") < global_blackboard.GetValue<int>("diamond_goal"))
        {
            SetGoal("current_diamond", OperatorType._bigger_or_equal, global_blackboard.GetValue<int>("diamond_goal"), VariableLocation._global);
        }*/
    }

    public int ResourcesNum()
    {
        return blackboard.GetValue<int>("rock");// + blackboard.GetValue<int>("diamond");
    }
}
