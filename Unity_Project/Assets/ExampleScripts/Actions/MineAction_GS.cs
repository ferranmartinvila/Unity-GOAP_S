using GOAP_S.Planning;
using UnityEngine;

public class MineAction_GS : Action_GS
{
    public float time = 1.0f;
    private float timer = 0.0f;
    public bool diamond = false;
    public bool rock = false;

    public override ACTION_RESULT ActionUpdate()
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            RecollectorBehaviour agent_behaviour = ((RecollectorBehaviour)agent.behaviour);

            if (agent_behaviour.targeted_slot != null)
            {
                agent_behaviour.targeted_slot.slot_available = true;
                agent_behaviour.targeted_slot = null;
            }

            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }

    public override void BlitDebugUI()
    {
        base.BlitDebugUI();

        if (state == ACTION_STATE.A_UPDATE)
        {
            if (diamond)
            {
                GUILayout.Label("Diamond");
            }
            if (rock)
            {
                GUILayout.Label("Rock");
            }
        }
    }
}
