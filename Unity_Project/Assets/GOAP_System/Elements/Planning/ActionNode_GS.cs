using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using GOAP_S.PT;
using GOAP_S.Planning;
using System.Collections.Generic;

namespace GOAP_S.AI
{
    public class ActionNode_GS : ISerializationCallbackReceiver
    {
        //UI fields
        [SerializeField] private Rect _window_rect; //Position of the node window in the editor
        [SerializeField] private string _id = ""; //Node ID used to set window 
        //Content fields
        [SerializeField] private string _name = "Action Node"; //Node name
        [SerializeField] private string _description = ""; //Node description
        [NonSerialized] private int _conditions_num = 0; //Number of conditions to do this node action
        [NonSerialized] private Property_GS[] _conditions = null; //Conditions array
        [NonSerialized] private Action_GS _action = null; //Action linked to the action node

        //Serialization fields
        [SerializeField] private List<UnityEngine.Object> _obj_refs; //List that contains the references to the objects serialized
        [SerializeField] private string serialized_conditions; //String where the serialized conditions are stored                                                       
        [SerializeField] private string serialized_action; //String where the serialized action is stored

        //Constructor =================
        public ActionNode_GS()
        {
            //Allocate conditions array
            _conditions = new Property_GS[ProTools.INITIAL_ARRAY_SIZE];
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
            conditions[conditions_num] = new_condition;
            //Update conditions count
            _conditions_num += 1;
            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        //Serialization Methods =======
        public void OnBeforeSerialize()
        {
            //Allocate object references list
            _obj_refs = new List<UnityEngine.Object>();
            //Serialize conditions set
            serialized_conditions = Serialization.SerializationManager.Serialize(_conditions, typeof(Property_GS[]), _obj_refs);
            //Serialize the action set
            serialized_action = Serialization.SerializationManager.Serialize(action, typeof(Action_GS), _obj_refs);
        }

        public void OnAfterDeserialize()
        {
            //Deserialize conditions
            _conditions = (Property_GS[])Serialization.SerializationManager.Deserialize(typeof(Property_GS[]), serialized_conditions, _obj_refs);
            //Count nodes
            for (int k = 0; k < _conditions.Length; k++)
            {
                if (_conditions[k] != null)
                {
                    _conditions_num++;
                }
            }
            //Deserialize the action
            action = (Action_GS)Serialization.SerializationManager.Deserialize(typeof(Action_GS), serialized_action, _obj_refs);
        }
    }
}
