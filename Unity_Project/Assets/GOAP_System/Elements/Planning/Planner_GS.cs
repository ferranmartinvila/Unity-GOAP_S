using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP_S.AI;

namespace GOAP_S.Planning
{
    public class Planner_GS {

        //Static instance of this class
        private static Planner_GS _Instance;

        //Property to get static instance
        public static Planner_GS Instance
        {
            get
            {
                //Check if the instance is null, in null case generates a new one
                if (_Instance == null)
                {
                    _Instance = new Planner_GS();
                }
                return _Instance;
            }
        }

        //Planning Methods ================
        public Queue<ActionNode_GS> GeneratePlan(Agent_GS agent, HashSet<Property_GS> goal_world_state)
        {
            //Allocate a new queue of actions
            Queue<ActionNode_GS> action_plan = new Queue<ActionNode_GS>();

            //First get the current world state
            HashSet<Property_GS> world_state = agent.blackboard.GenerateWorldState();

            //TODO

            //Finally return the generated actions queue
            return action_plan;
        }
    }
}
