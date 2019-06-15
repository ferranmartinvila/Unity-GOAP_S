using UnityEngine;
using GOAP_S.Planning;

public class FindBarrelAction_GS : Action_GS
{
    public float time = 1.0f;
    private float timer = 0.0f;

    public override ACTION_RESULT ActionUpdate()
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            RecollectorBehaviour agent_behaviour = ((RecollectorBehaviour)agent.behaviour);

            GameObject[] stores = GameObject.FindGameObjectsWithTag("Store");
            foreach (GameObject store in stores)
            {
                if (store.GetComponentInChildren<SlotManager>().slot_available == true)
                {
                    agent_behaviour.targeted_slot = store.GetComponentInChildren<SlotManager>();
                    agent_behaviour.targeted_slot.slot_available = false;

                    blackboard.SetVariable<Vector3>("store_pos", agent_behaviour.targeted_slot.gameObject.transform.position);

                    break;
                }
            }

            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }
}
