using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using GOAP_S.Tools;
using GOAP_S.Planning;
using System.Collections.Generic;

namespace GOAP_S.AI
{
    [Serializable]
    public class ActionNode_GS : ISerializationCallbackReceiver
    {
        //UI fields
        [SerializeField] private Rect _window_rect; //Position of the node window in the editor
        [SerializeField] private string _id = ""; //Node ID used to set window 
        //Content fields
        [SerializeField] private string _name = "Action Node"; //Node name
        [SerializeField] private string _description = ""; //Node description
        [NonSerialized] private Property_GS[] _conditions = null; //Conditions array
        [NonSerialized] private int _conditions_num = 0; //Number of conditions to execute this node action
        [NonSerialized] private Property_GS[] _effects = null; //Effects array
        [NonSerialized] private int _effects_num = 0; //Number of effects on action execute
        [NonSerialized] private Action_GS _action = null; //Action linked to the action node
        [NonSerialized] private Agent_GS _agent = null; //The agent is this action node in
        //Serialization fields
        [SerializeField] private List<UnityEngine.Object> _obj_refs; //List that contains the references to the objects serialized
        [SerializeField] private string serialized_conditions; //String where the serialized conditions are stored                                                       
        [SerializeField] private string serialized_effects; //String where the serialized effects are stored
        [SerializeField] private string serialized_action; //String where the serialized action is stored

        //Constructor =================
        public ActionNode_GS()
        {
            //Allocate conditions array
            _conditions = new Property_GS[ProTools.INITIAL_ARRAY_SIZE];
            //Allocate effects array
            _effects = new Property_GS[ProTools.INITIAL_ARRAY_SIZE];
        }

        //Loop Methods ================
        public void Start()
        {

        }

        public void Update()
        {

        }

        //Get/Set Methods =============
        public Rect window_rect
        {
            get
            {
                return _window_rect;
            }
            set
            {
                _window_rect = value;
            }
        }

        public Vector2 window_position
        {
            get
            {
                return new Vector2(_window_rect.x, _window_rect.y);
            }
            set
            {
                _window_rect.x = value.x;
                _window_rect.y = value.y;
            }
        }

        public Vector2 window_size
        {
            get
            {
                return new Vector2(_window_rect.width, _window_rect.height);
            }
            set
            {
                _window_rect.width = value.x;
                _window_rect.height = value.y;
            }
        }

        public int id
        {
            get
            {
                //If id is null we generate a new one
                if (string.IsNullOrEmpty(_id))
                {
                    _id = Guid.NewGuid().ToString();
                }
                return _id.GetHashCode();
            }
        }

        public int index
        {
            get
            {
                for (int k = 0; k < _agent.action_nodes_num; k++)
                {
                    if (_agent.action_nodes[k] == this) return k;
                }
                return -1;
            }
        }

        public int conditions_num
        {
            get
            {
                return _conditions_num;
            }
        }

        public Property_GS [] conditions
        {
            get
            {
                return _conditions;
            }
        }

        public int effects_num
        {
            get
            {
                return _effects_num;
            }
        }

        public Property_GS[] effects
        {
            get
            {
                return _effects;
            }
        }

        public Action_GS action
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

        public int action_cost
        {
            get
            {
                if(_action == null)
                {
                    return int.MaxValue; //Return max value to avoid null actions during planning
                }
                return _action.cost;
            }
        }

        public Agent_GS agent
        {
            get
            {
                return _agent;
            }
            set
            {
                _agent = value;
            }
        }

        public string name
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

        public string description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        //Planning Methods ============
        public void AddCondition(Property_GS new_condition)
        {
            //Check if we need to allocate more items in the array
            if (_conditions_num == _conditions.Length)
            {
                //Double array capacity
                Property_GS[] new_array = new Property_GS[_conditions_num * 2];
                //Copy values
                for (int k = 0; k < _conditions_num; k++)
                {
                    new_array[k] = _conditions[k];
                }
            }

            //Add the new condition to the array
            _conditions[_conditions_num] = new_condition;
            //Update conditions count
            _conditions_num += 1;
            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public void RemoveCondition(Property_GS target_condition)
        {
            //First search property
            for(int k = 0; k < _conditions_num; k++)
            {
                if (_conditions[k] == target_condition)
                {
                    //Last condition case
                    if (k == _conditions.Length - 1)
                    {
                        _conditions[k] = null;
                    }
                    else
                    {
                        //When property is found copy values in front of it a slot backwards
                        for (int n = k; n < _conditions.Length - 1; n++)
                        {
                            _conditions[n] = _conditions[n + 1];
                        }
                    }
                    //Update conditions count
                    _conditions_num -= 1;
                    //Mark scene dirty
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    //Job is done we dont need to continue the iteration
                    return;
                }
            }
            //Porperty not found case
            Debug.LogWarning("Condition: " + target_condition.A_key + " not found on remove!");
        }

        public void AddEffect(Property_GS new_effect)
        {
            //Check if we need to allocate more items in the array
            if (_effects_num == _effects.Length)
            {
                //Double array capacity
                Property_GS[] new_array = new Property_GS[_effects_num * 2];
                //Copy values
                for (int k = 0; k < _effects_num; k++)
                {
                    new_array[k] = _effects[k];
                }
            }

            //Add the new effects to the array
            _effects[_effects_num] = new_effect;
            //Update effects count
            _effects_num += 1;
            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        }

        public void RemoveEffect(Property_GS target_effect)
        {
            //First search property
            for (int k = 0; k < _effects_num; k++)
            {
                if (_effects[k] == target_effect)
                {
                    //Last effect case
                    if (k == _effects.Length - 1)
                    {
                        _effects[k] = null;
                    }
                    else
                    {
                        //When property is found copy values in front of it a slot backwards
                        for (int n = k; n < _effects.Length - 1; n++)
                        {
                            _effects[n] = _effects[n + 1];
                        }
                    }
                    //Update effects count
                    _effects_num -= 1;
                    //Mark scene dirty
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    //Job is done we dont need to continue the iteration
                    return;
                }
            }
            //Porperty not found case
            Debug.LogWarning("Effect: " + target_effect.A_key + " not found on remove!");
        }

        public bool ValidateWorldState(WorldState_GS current)
        {
            for(int k = 0; k < _conditions_num; k++)
            {
                if(_conditions[k].DistanceTo(current.GetProperty(_conditions[k].A_key)) == false)
                {
                    //If one current world state property does not fit in the action conditions return false
                    return false;
                }
            }
            //If all current world state properties are correct return true
            return true;
        }

        public WorldState_GS EffectWorldState(WorldState_GS current)
        {
            //Allocate the new world state based in the current
            WorldState_GS new_world_state = new WorldState_GS(current);
            //Iterate and apply all the action effects to the new world state
            for(int k = 0; k < _effects_num; k++)
            {
                new_world_state.SetGoal(_effects[k].A_key, _effects[k]);
            }
            //Return the resultant world state
            return new_world_state;
        }

        //Serialization Methods =======
        public void OnBeforeSerialize()
        {
            //Allocate object references list
            _obj_refs = new List<UnityEngine.Object>();

            //Serialize conditions set
            if (_conditions != null)
            {
                serialized_conditions = Serialization.SerializationManager.Serialize(_conditions, typeof(Property_GS[]), _obj_refs);
            }
            else
            {
                serialized_conditions = null;
            }

            //Serialize effects set
            if (_effects != null)
            {
                serialized_effects = Serialization.SerializationManager.Serialize(_effects, typeof(Property_GS[]), _obj_refs);
            }
            else
            {
                serialized_effects = null;
            }

            //Serialize the action set
            if (_action != null)
            {
                serialized_action = Serialization.SerializationManager.Serialize(action, typeof(Action_GS), _obj_refs);
            }
            else
            {
                serialized_action = null;
            }
        }

        public void OnAfterDeserialize()
        {

            //Deserialize conditions
            if (string.IsNullOrEmpty(serialized_conditions))
            {
                _conditions = new Property_GS[ProTools.INITIAL_ARRAY_SIZE];
                _conditions_num = 0;
            }
            else
            {
                _conditions = (Property_GS[])Serialization.SerializationManager.Deserialize(typeof(Property_GS[]), serialized_conditions, _obj_refs);
                //Count conditions
                for (int k = 0; k < _conditions.Length; k++)
                {
                    if (_conditions[k] != null)
                    {
                        _conditions_num++;
                    }
                }
            }

            //Deserialize effects
            if (string.IsNullOrEmpty(serialized_effects))
            {
                _effects = new Property_GS[ProTools.INITIAL_ARRAY_SIZE];
                _effects_num = 0;
            }
            else
            {
                _effects = (Property_GS[])Serialization.SerializationManager.Deserialize(typeof(Property_GS[]), serialized_effects, _obj_refs);
                //Count effects
                for (int k = 0; k < _effects.Length; k++)
                {
                    if (_effects[k] != null)
                    {
                        _effects_num++;
                    }
                }
            }

            //Deserialize the action
            if (string.IsNullOrEmpty(serialized_action))
            {
                _action = null;
            }
            else
            {
                _action = (Action_GS)Serialization.SerializationManager.Deserialize(typeof(Action_GS), serialized_action, _obj_refs);
            }
            
        }
    }
}
