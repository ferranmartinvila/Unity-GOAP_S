using System.Collections.Generic;
using UnityEngine;

public class QuarryManager : MonoBehaviour
{
    //This manager provides mining slots

    public int group_id = 0; //Look for quarry action uses this id to only select quarries with the same id group of the agent
    public List<Transform> mining_slots;
    private List<bool> slots_state; //False = empty, true = full

	// Use this for initialization
	void Start ()
    {
        slots_state = new List<bool>();
        foreach(Transform tr in mining_slots)
        {
            slots_state.Add(false);
        }
	}

    public Vector3 GetMiningSlot()
    {
        for(int k = 0; k < mining_slots.Count; k++)
        {
            if(slots_state[k] == false)
            {
                slots_state[k] = true;
                return mining_slots[k].position;
            }
        }

        Debug.LogError("All the mining slots are full!");

        return Vector3.zero;
    }

    public void FreeMiningSlot(Vector3 slot_location)
    {
        for (int k = 0; k < mining_slots.Count; k++)
        {
            if (Vector3.Distance(mining_slots[k].position, slot_location) < 0.1f)
            {
                slots_state[k] = false;
            }
        }

        Debug.LogError("Mining slot with location " + slot_location + " not found!");
    }
}
