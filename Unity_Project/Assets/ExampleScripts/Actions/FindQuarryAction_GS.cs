using UnityEngine;
using GOAP_S.Planning;

public class FindQuarryAction_GS : Action_GS
{
    public float time = 1.0f;
    private float timer = 0.0f;
    public bool diamond = false;
    public bool rock = false;

    public override void CopyValues(Action_GS copy)
    {
        diamond = ((FindQuarryAction_GS)copy).diamond;
        rock = ((FindQuarryAction_GS)copy).rock;

        base.CopyValues(copy);
    }

    public override ACTION_RESULT ActionUpdate()
    {
        timer += Time.deltaTime;
        if (timer > time)
        {
            RecollectorBehaviour agent_behaviour = ((RecollectorBehaviour)agent.behaviour);
            agent_behaviour.going_for_diamond = diamond;
            agent_behaviour.going_for_rock = rock;

            GameObject[] mines = null;
            if (diamond)
            {
                mines = GameObject.FindGameObjectsWithTag("DiamondMine");
            }
            else if (rock)
            {
                mines = GameObject.FindGameObjectsWithTag("RockMine");
            }

            foreach (GameObject mine in mines)
            {
                SlotManager mine_slot = mine.GetComponentInChildren<SlotManager>();
                if (mine_slot.slot_available)
                {
                    if (agent_behaviour.targeted_slot != null)
                    {
                        agent_behaviour.targeted_slot.slot_available = true;
                    }

                    mine_slot.slot_available = false;
                    agent_behaviour.targeted_slot = mine_slot;

                    if (diamond)
                    {
                        blackboard.SetVariable<Vector3>("diamond_pos", mine_slot.gameObject.transform.position);
                    }
                    else if (rock)
                    {
                        blackboard.SetVariable<Vector3>("rock_pos", mine_slot.gameObject.transform.position);
                    }
                    break;
                }
            }

            return ACTION_RESULT.A_NEXT;
        }

        return ACTION_RESULT.A_CURRENT;
    }
}
