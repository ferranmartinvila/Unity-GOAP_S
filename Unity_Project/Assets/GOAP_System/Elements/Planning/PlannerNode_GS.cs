using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP_S.AI;

namespace GOAP_S.Planning
{
    public class PlannerNode_GS
    {

        //Content fields
        int _id = 0; //This node id
        int _parent_id = 0; //Parent node id
        float _g = 0; //Cost of getting to this node from start node
        float _h = 0; //Distance from current node to the goal
        WorldState_GS _resultant_world_state = null; //Resultant world state after this node action 
        ActionNode_GS _action = null; //Node Action 

        //Constructors ================
        public PlannerNode_GS(float node_g, float node_h, int node_id, int parent_node_id, WorldState_GS resultant_ws, ActionNode_GS node_action)
        {
            _g = node_g;
            _h = node_h;
            _id = node_id;
            _parent_id = parent_node_id;
            _resultant_world_state = resultant_ws;
            _action = node_action;
        }

        //Functionality Methods =======


        //Get/Set Methods =============
        public float f
        {
            get
            {
                return _g + _h;
            }
        }

        public float g
        {
            get
            {
                return _g;
            }
            set
            {
                _g = value;
            }
        }

        public float h
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
            }
        }

        public WorldState_GS resultant_world_state
        {
            get
            {
                return _resultant_world_state;
            }
        }

        public ActionNode_GS action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }

        public int id
        {
            get
            {
                return _id;
            }
        }

        public int parent_id
        {
            get
            {
                return _parent_id;
            }
            set
            {
                _parent_id = value;
            }
        }
    }
}
