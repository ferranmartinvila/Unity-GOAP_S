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
    [Serializable]
    public class Agent_GS : MonoBehaviour, ISerializationCallbackReceiver
    {
        /*
         Agent state enum used to modify agent reactions depending of its state
        */
        public enum AGENT_STATE
        {
            AG_IDLE = 0,
            AG_ACTION
        }
        
        //Identification fields
        [SerializeField] private string _name = "un_named"; //Agent name(usefull for the user to recognize the behaviours)
        [NonSerialized] private string _id = null; //Agent id used to generate world state and differentiate agents
        //Content fields
        [NonSerialized] private AGENT_STATE _state = AGENT_STATE.AG_IDLE; //Current agent state
        [NonSerialized] private AgentBehaviour_GS _behaviour = null; //The behaviour is the agent using
        [NonSerialized] private ActionNode_GS[] _action_nodes = null; //Action nodes array, serialized specially so unity call OnBefore and After methods and we create our custom serialization methods
        [NonSerialized] private int _action_nodes_num = 0; //The number of nodes placed in the array
        [NonSerialized] private Blackboard_GS _blackboard = null; //The blackboard is the agent using
        [NonSerialized] private BlackboardComp_GS _blackboard_component = null; //Inspector representation of the blackboard
        [NonSerialized] private WorldState_GS _goal_world_state = null; //Agent goal world state defined in the agent behaviour
        [NonSerialized] private Planner_GS _planner; //Class that holds the planning algorithm
        [NonSerialized] private Stack<ActionNode_GS> _current_plan = null; //Current actions plan generated from the goal world state
        [NonSerialized] private List<ActionNode_GS> _popped_actions = null; //List with the already completed actions from the current plan
        [NonSerialized] private ActionNode_GS _current_action = null; //Current action from the plan in execution
        [NonSerialized] private Action_GS _idle_action = null; //Action executed when the agent is in idle state
        //Callbacks
        public delegate void AgentCallbackFunction(); //Agent delegate used to provide a basic callback system for some events
        [NonSerialized] public AgentCallbackFunction on_agent_plan_change_delegate; //Agent plan change event callback
        //Serialization fields
        [SerializeField] private List<UnityEngine.Object> obj_refs = null; //List that contains the references to the objects serialized
        [SerializeField] private string serialized_behaviour = null; //String where the agent behaviour is serialized
        [SerializeField] private string serialized_action_nodes = null; //String where the action nodes are serialized
        [SerializeField] private string serialized_blackboard = null; //String where the blackboard is serialized
        [SerializeField] private string serialized_idle_action = null; //String where the idle action is serialized

        //Constructors ====================
        public Agent_GS()
        {
            //Allocate nodes array
            _action_nodes = new ActionNode_GS[ProTools.INITIAL_ARRAY_SIZE];
            //Allocate the current plan
            _current_plan = new Stack<ActionNode_GS>();
            //Allocate the poped actions list
            _popped_actions = new List<ActionNode_GS>();
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
            if(_behaviour != null && _idle_action != null)
            {
                //Start the behaviour
                _behaviour.agent = this;
                _behaviour.Start();
                _idle_action.agent = this;
                _idle_action.ActionAwake();
            }
            else
            {
                //Alert message
                Debug.LogError("Agent: " + _name + " behaviour or idle action is null!");
                //In null case the agent is disabled
                enabled = false;
            }
        }

        private void Update()
        {
            //Switch between agent idle and action state
            switch (_state)
            {
                case AGENT_STATE.AG_IDLE:
                    {
                        if (_current_plan.Count == 0)
                        {
                            //The behaviour basically defines the goal world state
                            _behaviour.Update();

                            //Generate the plan
                            _current_plan = planner.GeneratePlan(this);

                            //In avaliable plan case
                            if (_current_plan.Count > 0)
                            {
                                if (on_agent_plan_change_delegate != null)
                                {
                                    on_agent_plan_change_delegate();
                                }
                            }
                            //In no action to execute case we execute the agent idle action
                            else
                            {
                                _idle_action.Execute();
                                //At this point the idle action is exectued in an endless update
                                if (_idle_action.state > Action_GS.ACTION_STATE.A_UPDATE)
                                {
                                    _idle_action.state = Action_GS.ACTION_STATE.A_UPDATE;
                                }
                            }
                        }
                        else
                        {
                            //We need to finish the idle action
                            _idle_action.Execute();

                            //WHen the idle action ends we can execute the agent plan
                            if (_idle_action.state == Action_GS.ACTION_STATE.A_COMPLETE)
                            {
                                //Agent state is setted to action state
                                _state = AGENT_STATE.AG_ACTION;

                                //Awake all the plan actions
                                ActionNode_GS[] plan_actions = _current_plan.ToArray();
                                foreach (ActionNode_GS action_node in plan_actions)
                                {
                                    action_node.action.ActionAwake();
                                }

                                //Focus first plan action
                                _current_action = _current_plan.Pop();
                                _current_action.agent = this;
                                _popped_actions.Add(_current_action);
                            }
                        }
                    }
                    break;
                case AGENT_STATE.AG_ACTION:
                    {
                        //Current action execution result
                        Action_GS.ACTION_RESULT execution_result = _current_action.action.Execute();

                        //React with the action result
                        if (execution_result == Action_GS.ACTION_RESULT.A_NEXT && _current_action.action.state == Action_GS.ACTION_STATE.A_COMPLETE)
                        {
                            //Apply action effects
                            _current_action.ApplyActionNodeEffects();

                            //Check current plan
                            if (_current_plan.Count > 0)
                            {
                                //Avaliable action case
                                _current_action = _current_plan.Pop();
                                _current_action.agent = this;
                                _popped_actions.Add(_current_action);
                            }
                            else
                            {
                                //Plan completed case
                                _current_action = null;
                                _current_plan.Clear();
                                _popped_actions.Clear();
                                _state = AGENT_STATE.AG_IDLE;
                                return;
                            }
                        }
                        else if (execution_result == Action_GS.ACTION_RESULT.A_ERROR)
                        {
                            //In action error the plan is cancelled
                            _current_action = null;
                            _current_plan.Clear();
                            _state = AGENT_STATE.AG_IDLE;
                        }
                    }
                    break;
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

        public void InitializePlanProperties()
        {
            for(int k = 0; k < action_nodes_num;k++)
            {
                ActionNode_GS target = action_nodes[k];
                for(int i = 0; i < target.conditions_num; i++)
                {
                    if (string.IsNullOrEmpty(target.conditions[i].B_key) == false)
                    {
                        string[] B_key_info = target.conditions[i].B_key.Split('/');
                        if (string.Compare(B_key_info[0], "Global") == 0)
                        {
                            target.conditions[i].value = GlobalBlackboard_GS.blackboard.GetObjectVariable(B_key_info[1]);
                        }
                        else
                        {
                            target.conditions[i].value = this.blackboard.GetObjectVariable(B_key_info[1]);
                        }
                    }
                }
                for(int j = 0; j < target.effects_num; j++)
                {
                    if (string.IsNullOrEmpty(target.effects[j].B_key) == false)
                    {
                        string[] B_key_info = target.effects[j].B_key.Split('/');
                        if (string.Compare(B_key_info[0], "Global") == 0)
                        {
                            target.effects[j].value = GlobalBlackboard_GS.blackboard.GetObjectVariable(B_key_info[1]);
                        }
                        else
                        {
                            target.effects[j].value = this.blackboard.GetObjectVariable(B_key_info[1]);
                        }
                    }
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

        public Action_GS idle_action
        {
            get
            {
                return _idle_action;
            }
            set
            {
                _idle_action = value;
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

        public Stack<ActionNode_GS> current_plan
        {
            get
            {
                return _current_plan;
            }
        }

        public List<ActionNode_GS> popped_actions
        {
            get
            {
                return _popped_actions;
            }
        }

        //Serialization Methods ===========
        public void OnBeforeSerialize() //Serialize
        {
            //Allocate object references list
            obj_refs = new List<UnityEngine.Object>();

            //Serialize behaviour
            if (_behaviour != null)
            {
                serialized_behaviour = Serialization.SerializationManager.Serialize(_behaviour, typeof(AgentBehaviour_GS), obj_refs);
            }
            else
            {
                serialized_behaviour = null;
            }

            //Serialize idle action
            if (_idle_action != null)
            {
                serialized_idle_action = Serialization.SerializationManager.Serialize(_idle_action, typeof(Action_GS), obj_refs);
            }
            else
            {
                serialized_idle_action = null;
            }

            //Serialize blackboard
            if (_blackboard != null)
            {
                serialized_blackboard = Serialization.SerializationManager.Serialize(_blackboard, typeof(Blackboard_GS), obj_refs);
            }
            else
            {
                serialized_blackboard = null;
            }

            //Serialize action nodes
            if (_action_nodes != null)
            {
                serialized_action_nodes = Serialization.SerializationManager.Serialize(_action_nodes, typeof(ActionNode_GS[]), obj_refs);
            }
            else
            {
                serialized_action_nodes = null;
            }
        }

        public void OnAfterDeserialize() //Deserialize
        {
            //Deserialize behaviour
            if (string.IsNullOrEmpty(serialized_behaviour))
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

            //Deserialize idle action
            if (string.IsNullOrEmpty(serialized_idle_action))
            {
                //If serialization srtring is null idle action is null
                _idle_action = null;
            }
            else
            {
                //If serialization string is not null lets deserialize the idle action
                _idle_action = (Action_GS)Serialization.SerializationManager.Deserialize(typeof(Action_GS), serialized_idle_action, obj_refs);
            }

            //Deserialize blackboard
            if (string.IsNullOrEmpty(serialized_blackboard))
            {
                _blackboard = new Blackboard_GS(this);
            }
            else
            {
                _blackboard = (Blackboard_GS)Serialization.SerializationManager.Deserialize(typeof(Blackboard_GS), serialized_blackboard, obj_refs);
                if (_blackboard == null)
                {
                    _blackboard = new Blackboard_GS(this);
                }
                _blackboard.target_agent = this;
            }

            //Deserialize action nodes
            if (string.IsNullOrEmpty(serialized_action_nodes))
            {
                _action_nodes = new ActionNode_GS[ProTools.INITIAL_ARRAY_SIZE];
                _action_nodes_num = 0;
            }
            else
            { 
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
            }
        }
    }
}