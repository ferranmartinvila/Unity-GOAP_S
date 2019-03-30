using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using GOAP_S.Blackboard;
using GOAP_S.Planning;
using GOAP_S.Tools;

namespace GOAP_S.AI
{
    public class Agent_GS : MonoBehaviour, ISerializationCallbackReceiver
    {
        //Identification fields
        [SerializeField] private string _name = "un_named"; //Agent name(usefull for the user to recognize the behaviours)
        [NonSerialized] private string _id = null; //Agent id used to generate world state and differentiate agents
        [NonSerialized] private AgentBehaviour_GS _behaviour = null; //The behaviour is the agent using
        //Content fields
        [NonSerialized] private ActionNode_GS[] _action_nodes = null; //Action nodes array, serialized specially so unity call OnBefore and After methods and we create our custom serialization methods
        [NonSerialized] private int _action_nodes_num = 0; //The number of nodes placed in the array
        [NonSerialized] private Blackboard_GS _blackboard = null; //The blackboard is the agent using
        [NonSerialized] private BlackboardComp_GS _blackboard_component = null; //Inspector representation of the blackboard
        [NonSerialized] private WorldState_GS _goal_world_state = null; //Agent goal world state defined in the agent behaviour
        [NonSerialized] private Planner_GS _planner; //Class that holds the planning algorithm
        [NonSerialized] private Queue<ActionNode_GS> _current_plan = null; //Current actions plan generated from the goal world state
        //Callbacks
        public delegate void AgentCallbackFunction(); //Agent delegate used to provide a basic callback system for some events
        [NonSerialized]public AgentCallbackFunction on_agent_plan_change_delegate; //Agent plan change event callback
        //Serialization fields
        [SerializeField] private List<UnityEngine.Object> obj_refs = null; //List that contains the references to the objects serialized
        [SerializeField] private string serialized_behaviour = null; //String where the agent behaviour is serialized
        [SerializeField] private string serialized_action_nodes = null; //String where the action nodes are serialized
        [SerializeField] private string serialized_blackboard = null; //String where the blackboard is serialized
        
        //Constructors ====================
        public Agent_GS()
        {
            //Allocate nodes array
            _action_nodes = new ActionNode_GS[ProTools.INITIAL_ARRAY_SIZE];
            //Allocate the current plan
            _current_plan = new Queue<ActionNode_GS>();
        }

        //Loop Methods ====================
        private void OnValidate()
        {
            //Check if the agent have a blackboard
            if (_blackboard == null)
            {
                //If not generate one for him
                EditorApplication.delayCall += () => _blackboard = blackboard;
            }
        }

        private void Start()
        {
            //First check if this agent have a defined behaviour
            if(_behaviour != null)
            {
                //Start the behaviour
                _behaviour.Start();
            }
            else
            {
                //Alert message
                Debug.LogError("Agent: " + _name + " behaviour is null!");
                //In null case the agent is disabled
                enabled = false;
            }
        }

        private void Update()
        {
            if (_current_plan.Count == 0)
            {
                _behaviour.Update();

                _current_plan = planner.GeneratePlan(this);
                if (on_agent_plan_change_delegate != null)
                {
                    on_agent_plan_change_delegate();
                }
            }
        }

        //Planning Methods ================
        public void ClearPlanning()
        {
            //Clear action nodes
            for (int k = 0; k < _action_nodes_num; k++)
            {
                _action_nodes[k] = null;
            }
            _action_nodes_num = 0;
            //Mark scene drity
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public ActionNode_GS AddActionNode(Vector2 position)
        {
            //Check if we need to allocate more items in the array
            if (_action_nodes_num == _action_nodes.Length)
            {
                //Double array capacity
                ActionNode_GS[] new_array = new ActionNode_GS[_action_nodes_num * 2];
                //Copy values
                for (int k = 0; k < _action_nodes_num; k++)
                {
                    new_array[k] = _action_nodes[k];
                }
                //Set new array
                _action_nodes = new_array;
            }

            ActionNode_GS new_node = new ActionNode_GS();
            //Set a position in the node editor canvas
            new_node.window_rect = new Rect(position.x, position.y, 100, 100);
            //Add the new node to the action nodes array
            _action_nodes[_action_nodes_num] = new_node;
            //Add node count
            _action_nodes_num += 1;
            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

            return new_node;
        }

        public void RemoveActionNode(ActionNode_GS target)
        {
            for (int k = 0; k < action_nodes.Length; k++)
            {
                if (action_nodes[k] == target)
                {
                    if (k == action_nodes.Length - 1)
                    {
                        action_nodes[k] = null;
                    }
                    else
                    {
                        for (int i = k; i < action_nodes.Length - 1; i++)
                        {
                            action_nodes[i] = action_nodes[i + 1];
                        }
                    }
                    //Update node count
                    _action_nodes_num -= 1;
                }
            }
        }

        //Get/Set Methods =================
        new public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string id
        {
            get
            {
                //If id is null generate a new one
                if(string.IsNullOrEmpty(_id))
                {
                    _id = System.Guid.NewGuid().ToString();
                }

                return _id;
            }
        }

        public ActionNode_GS[] action_nodes
        {
            get
            {
                return _action_nodes;
            }
        }

        public int action_nodes_num
        {
            get
            {
                return _action_nodes_num;
            }
        }

        public Blackboard_GS blackboard
        {
            get
            {
                //Generate blackboard in null case
                if (_blackboard == null)
                {
                    _blackboard = new Blackboard_GS(this);
                }
                
                //Generate blackboard component in null case
                if (_blackboard_component == null)
                {
                    _blackboard_component = gameObject.GetComponent<BlackboardComp_GS>();
                    if (_blackboard_component == null)
                    {
                        _blackboard_component = gameObject.AddComponent<BlackboardComp_GS>();
                    }
                    _blackboard_component.agent = this;
                }

                return _blackboard;
            }
        }

        public BlackboardComp_GS blackboard_comp
        {
            get
            {
                //Generate blackboard component in null case
                if (_blackboard_component == null)
                {
                    _blackboard_component = gameObject.AddComponent<BlackboardComp_GS>();
                    _blackboard_component.agent = this;
                }

                return _blackboard_component;
            }
        }

        public AgentBehaviour_GS behaviour
        {
            get
            {
                return _behaviour;
            }
            set
            {
                _behaviour = value;
            }
        }

        public WorldState_GS goal_world_state
        {
            get
            {
                if(_goal_world_state == null)
                {
                    _goal_world_state = new WorldState_GS();
                }
                return _goal_world_state;
            }
        }

        private Planner_GS planner
        {
            get
            {
                if(_planner == null)
                {
                    _planner = new Planner_GS();
                }
                return _planner;
            }
        }

        public Queue<ActionNode_GS> current_plan
        {
            get
            {
                return _current_plan;
            }
        }

        //Serialization Methods ===========
        public void OnBeforeSerialize() //Serialize
        {
            //Allocate object references list
            obj_refs = new List<UnityEngine.Object>();
            //Serialize action nodes
            serialized_action_nodes = Serialization.SerializationManager.Serialize(_action_nodes, typeof(ActionNode_GS[]), obj_refs);
            //Serialize blackboard
            serialized_blackboard = Serialization.SerializationManager.Serialize(_blackboard, typeof(Blackboard_GS), obj_refs);
            //Serialize behaviour
            if(_behaviour != null)
            {
                serialized_behaviour = Serialization.SerializationManager.Serialize(_behaviour, typeof(AgentBehaviour_GS), obj_refs);
            }
        }

        public void OnAfterDeserialize() //Deserialize
        {
            //Deserialize action nodes
            _action_nodes = (ActionNode_GS[])Serialization.SerializationManager.Deserialize(typeof(ActionNode_GS[]), serialized_action_nodes, obj_refs);
            //Count nodes
            for (int k = 0; k < _action_nodes.Length; k++)
            {
                if (_action_nodes[k] != null)
                {
                    _action_nodes_num++;
                    //Set action node agent
                    _action_nodes[k].agent = this;
                    //If the action node have an action set the action agent
                    if (_action_nodes[k].action != null)
                    {
                        _action_nodes[k].action.agent = this;
                    }
                }
            }

            //Deserialize blackboard
            _blackboard = (Blackboard_GS)Serialization.SerializationManager.Deserialize(typeof(Blackboard_GS), serialized_blackboard, obj_refs);
            if (_blackboard == null)
            {
                _blackboard = new Blackboard_GS(this);
            }
            _blackboard.target_agent = this;

            //Deserialize behaviour
            if(string.IsNullOrEmpty(serialized_behaviour))
            {
                //If serailzation string is null behaviour is null
                _behaviour = null;
            }
            else
            {
                //If serialization string is not null lets deserialize the behaviour
                _behaviour = (AgentBehaviour_GS)Serialization.SerializationManager.Deserialize(typeof(AgentBehaviour_GS), serialized_behaviour, obj_refs);
                _behaviour.agent = this;
            }
        }
    }
}