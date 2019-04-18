using System.Collections.Generic;
using UnityEngine;
using GOAP_S.AI;
using System.Linq;
using GOAP_S.Tools;

namespace GOAP_S.Planning
{
    public class Planner_GS
    {
        private int _id_number = 0;
        private int _current_iterations = 0; //Iterations count, avoid infinite loop in impossible paths

        SortedDictionary<int, List<PlannerNode_GS>> _open = new SortedDictionary<int, List<PlannerNode_GS>>();
        SortedDictionary<int,PlannerNode_GS> _closed = new SortedDictionary<int, PlannerNode_GS>();

        //Planning Methods ================
        public Stack<ActionNode_GS> GeneratePlan(Agent_GS agent)
        {
            //First get the world state states
            //Current
            WorldState_GS current_world_state = agent.blackboard.GenerateWorldState();
            //Goal
            //Get current as base world state
            WorldState_GS goal_world_state = new WorldState_GS(current_world_state);
            //Apply the goals on the current to get the goal world state
            goal_world_state.MixGoals(agent.goal_world_state);
            
            //Check if the current world state and the goal world state coincide
            int start_goal_distance = current_world_state.DistanceTo(goal_world_state);
            if(start_goal_distance == 0)
            {
                Debug.LogWarning("The current world state coincides with the goal world state: " + goal_world_state.name + " !");
                return new Stack<ActionNode_GS>();
            }

            //Allocate plan start node
            PlannerNode_GS start_node = new PlannerNode_GS(0, start_goal_distance, new_id_number, 0, current_world_state, null);

            //Add start node to the open list
            _open.Add(start_node.f, new List<PlannerNode_GS> { start_node });

            //Iterate open list till there are no nodes to check
            while(_open.Count > 0)
            {
                //Check and update iterations count
                if(ProTools.ITERATION_LIMIT < _current_iterations)
                {
                    Debug.LogWarning("Planning generation for agent: " + agent.name + " failed");
                    break;
                }
                else
                {
                    _current_iterations += 1;
                }

                //Current closed node
                PlannerNode_GS current_node = CloseNode();

                //Check if the resultant world state of the current node is the goal world state
                if (current_node.resultant_world_state.DistanceTo(goal_world_state) == 0)
                {
                    //Allocate a new queue of actions to store the plan
                    Stack<ActionNode_GS> action_plan = new Stack<ActionNode_GS>();
                    //Enqueue the goal action 
                    action_plan.Push(current_node.action);
                    //Iterate goal node "childs" to start node using the parent id
                    while (current_node.parent_id != 0)
                    {
                        //Update current node 
                        current_node = _closed[current_node.parent_id];
                        //Check if the node has an action assigned
                        if (current_node.action != null)
                        {
                            //Enqueue the new current node
                            action_plan.Push(current_node.action);
                        }
                    }

                    //Flip the generated queue
                    
                    //Return the generated actions queue
                    return action_plan;
                }

                //Iterate all the avaliable actions
                for (int k = 0; k < agent.action_nodes_num; k++)
                {
                    //Scoped action
                    ActionNode_GS scoped_action = agent.action_nodes[k];

                    //Check if this action is reachable from the current world state
                    if(scoped_action.ValidateWorldState(current_node.resultant_world_state) == false)
                    {
                        //If the current world state is not valid for this actions we discard it and keep iterating the others
                        continue;
                    }

                    //If this action can be executed in the current world state, action effects are applied in the current world state
                    WorldState_GS new_current_world_state = scoped_action.EffectWorldState(current_node.resultant_world_state);

                    //Check if there is an action in the open list that can also reach the new current world state
                    PlannerNode_GS in_open_node = null;
                    if(IsInOpen(new_current_world_state, out in_open_node) == true)
                    {
                        //In true case check if the new node is better
                        if(current_node.g + scoped_action.action_cost > in_open_node.g)
                        {
                            //The old node is better than this new one
                            continue;
                        }
                        //The new node is better than the old, lets update the node data
                        in_open_node.parent_id = current_node.id;
                        in_open_node.g = current_node.g + scoped_action.action_cost;
                        in_open_node.h = new_current_world_state.DistanceTo(goal_world_state);
                        in_open_node.action = scoped_action;
                    }
                    else
                    {
                        //In false case generate a new open node
                        PlannerNode_GS new_node = new PlannerNode_GS(current_node.g + scoped_action.action_cost, new_current_world_state.DistanceTo(goal_world_state), new_id_number, current_node.id, new_current_world_state, scoped_action);
                        //Add the new node to the open list
                        AddToOpen(new_node);
                    }

                }
            }

            //In no plan found case we return an empty plan
            return new Stack<ActionNode_GS>();
        }

        //Functionality Methods =======
        private int new_id_number
        {
            get
            {
                return _id_number += 1;
            }
        }

        private void AddToOpen(PlannerNode_GS new_node)
        {
            List<PlannerNode_GS> target_list = null;
            //Try to find the list that stores all the nodes with the same f than the new one
            if (_open.TryGetValue(new_node.f, out target_list))
            {
                //If the list exists we simply add the new node into it
                target_list.Add(new_node);
            }
            else
            {
                //If the new node is the only one with a f value of X we have to allocate a new list for him
                _open.Add(new_node.f, new List<PlannerNode_GS> { new_node });
            }
        }

        private PlannerNode_GS CloseNode()
        {
            //Get open nodes listed with the cheapest cost
            List<PlannerNode_GS> cheapest_nodes = _open[_open.Keys.Min()];
            //Get target node
            PlannerNode_GS target = cheapest_nodes[cheapest_nodes.Count - 1];
            //Store the last cheap node in the closed list
            _closed.Add(target.id, target);
            //Remove the close node from the open list
            cheapest_nodes.RemoveAt(cheapest_nodes.Count - 1);
            //If the removed node was the last we can remove the entire list for this key
            if (cheapest_nodes.Count == 0)
            {
                _open.Remove(_open.Keys.Min());
            }
            //Return the closed node
            return _closed[target.id];
        }

        private bool IsInOpen(WorldState_GS target, out PlannerNode_GS found)
        {
            //Iterate all the open nodes and check if anyone has the target world state as resultant world state
            foreach(KeyValuePair<int,List<PlannerNode_GS>> open_pair in _open)
            {
                foreach(PlannerNode_GS open_node in open_pair.Value)
                {
                    if(open_node.resultant_world_state.DistanceTo(target) == 0)
                    {
                        found = open_node;
                        return true;
                    }
                }
            }
            found = null;
            return false;
        }

        private bool IsInClosed(WorldState_GS target)
        {
            //Iterate all the closed nodes and check if anyone has the target world state as resultant world state
            foreach (KeyValuePair<int, PlannerNode_GS> closed_pair in _closed)
            {
                if(closed_pair.Value.resultant_world_state.DistanceTo(target) == 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
