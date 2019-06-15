using GOAP_S.AI;
using GOAP_S.Planning;
using GOAP_S.Tools;
using UnityEngine;


public class RecollectorBehaviour : AgentBehaviour_GS
{
    public bool going_for_rock = false;
    public bool going_for_diamond = false;
    public SlotManager targeted_slot = null;

    public override void Update()
    {
        if (global_blackboard.GetValue<int>("current_rock") < global_blackboard.GetValue<int>("rock_goal"))
        {
            SetGoal("current_rock", OperatorType._bigger_or_equal, global_blackboard.GetValue<int>("rock_goal"), VariableLocation._global);
        }

        if (global_blackboard.GetValue<int>("current_diamond") < global_blackboard.GetValue<int>("diamond_goal"))
        {
            SetGoal("current_diamond", OperatorType._bigger_or_equal, global_blackboard.GetValue<int>("diamond_goal"), VariableLocation._global);
        }
    }

    public override bool InActionUpdate(Action_GS.ACTION_RESULT current_action_result, Action_GS.ACTION_STATE current_action_state)
    {
        if (global_blackboard.GetValue<int>("current_rock") >= global_blackboard.GetValue<int>("rock_goal"))
        {
            if (blackboard.GetValue<int>("rock") > 0 || going_for_rock)
            {
                blackboard.SetVariable<int>("rock", 0);
                blackboard.SetVariable<Vector3>("target_position", new Vector3(0.0f, 0.0f, 0.0f));
                blackboard.SetVariable<bool>("in_pos", false);
                going_for_rock = false;
                targeted_slot.slot_available = true;
                targeted_slot = null;

                return false;
            }
        }

        if (global_blackboard.GetValue<int>("current_diamond") >= global_blackboard.GetValue<int>("diamond_goal"))
        {
            if (blackboard.GetValue<int>("diamond") > 0 || going_for_diamond)
            {
                blackboard.SetVariable<int>("diamond", 0);
                blackboard.SetVariable<Vector3>("target_position", new Vector3(0.0f, 0.0f, 0.0f));
                blackboard.SetVariable<bool>("in_pos", false);
                going_for_diamond = false;

                if (targeted_slot != null)
                {
                    targeted_slot.slot_available = true;
                    targeted_slot = null;
                }

                return false;
            }
        }
        return true;
    }

    public int ResourcesNum()
    {
        return blackboard.GetValue<int>("rock") + blackboard.GetValue<int>("diamond");
    }
}
