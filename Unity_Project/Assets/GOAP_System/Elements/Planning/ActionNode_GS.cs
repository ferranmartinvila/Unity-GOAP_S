using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using GOAP_S.PT;
using GOAP_S.Planning;

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
        [SerializeField] private int _conditions_num = 0; //Number of conditions to do this node action
        [SerializeField] private Property_GS[] _conditions = null; //Conditions array
        [System.NonSerialized] private Action_GS _action = null; //Action linked to the action node
                                                                 //Serialization fields
        [SerializeField] private string serialized_action; //String where the serialized data is stored

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
                    _id = System.Guid.NewGuid().ToString();
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
            //Serialize the action set
            serialized_action = GOAP_S.Serialization.SerializationManager.Serialize(action, typeof(Action_GS), null);

        }

        public void OnAfterDeserialize()
        {
            //Deserialize the action
            action = (Action_GS)GOAP_S.Serialization.SerializationManager.Deserialize(typeof(Action_GS), serialized_action, null);
        }
    }
}
