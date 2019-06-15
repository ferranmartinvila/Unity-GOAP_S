using UnityEngine;
using GOAP_S.Planning;

public class StoreResourcesAction_GS : Action_GS
{
    public float time = 0.2f;
    private float timer = 0.0f;

    public override ACTION_RESULT ActionUpdate()
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            RecollectorBehaviour agent_behaviour = ((RecollectorBehaviour)agent.behaviour);

            agent_behaviour.going_for_diamond = false;
            agent_behaviour.going_for_rock = false;

            if(agent_behaviour.targeted_slot != null)
            {
                agent_behaviour.targeted_slot.slot_available = true;
                agent_behaviour.targeted_slot = null;
            }

            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }
}
