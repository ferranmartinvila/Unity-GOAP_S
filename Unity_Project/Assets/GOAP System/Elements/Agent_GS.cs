using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using GOAP_S.Blackboard;

namespace GOAP_S.AI
{
    public class Agent_GS : MonoBehaviour, ISerializationCallbackReceiver
    {
        //Content fields
        [SerializeField] private string _name = "un_named"; //Agent name(usefull for the user to recognize the behaviours)
        [SerializeField] internal string _id = "null"; //Agent UUID
        [System.NonSerialized] private ActionNode_GS[] _action_nodes = null; //Action nodes array, serialized specially so unity call OnBefore and After methods and we create our custom serialization methods
        [System.NonSerialized] private int _action_node_num = 0; //The number of nodes placed in the array
        [System.NonSerialized] private Blackboard_GS _blackboard = null;
        //Serialization fields
        [SerializeField] private List<UnityEngine.Object> obj_refs; //List that contains the references to the objects serialized
        [SerializeField] private string serialized_action_nodes; //String where the action nodes are serialized
        [SerializeField] private string serialized_blackboard; //String where the blackboard is serialized

        //Constructors ====================
        public Agent_GS()
        {
            //Allocate nodes array
            _action_nodes = new ActionNode_GS[10];
        }

        //Loop Methods ====================
        private void Start()
        {
            foreach (ActionNode_GS node in action_nodes)
            {
                node.Start();
            }
        }

        private void Update()
        {
            foreach (ActionNode_GS node in action_nodes)
            {
                node.Update();
            }
        }

        //Planning Methods ================
        public void ClearPlanning()
        {
            //Clear action nodes
            int len = action_nodes.Length;
            for(int k = 0; k < len; k++)
            {
                action_nodes[k] = null;
            }
            //Mark scene drity
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public void AddActionNode(float x_pos, float y_pos)
        {
            ActionNode_GS new_node = new ActionNode_GS();
            //Set a position in the node editor canvas
            new_node.window_rect = new Rect(x_pos, y_pos, 100, 100);
            //Add the new node to the action nodes list
            action_nodes[action_nodes.Length] = new_node;
            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        public void DeleteActionNode(ActionNode_GS target)
        {
            int len = action_nodes.Length;
            for(int k = 0; k < len; k++)
            {
                if(action_nodes[k] == target)
                {
                    if (k == len - 1) action_nodes[k] = null;
                    for(int i = k; i < len; i++)
                    {
                        action_nodes[i] = action_nodes[i + 1];
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
                if (string.IsNullOrEmpty(_id))
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

        public Blackboard_GS blackboard
        {
            get
            {
                if (_blackboard == null)
                {
                    _blackboard = new Blackboard_GS();
                }
                return _blackboard;
            }
        }

        //Serialization Methods ===========
        public void OnBeforeSerialize() //Serialize
        {

            obj_refs = new List<UnityEngine.Object>();
            //Serialize action nodes
            serialized_action_nodes = GOAP_S.Serialization.SerializationManager.Serialize(_action_nodes, typeof(List<ActionNode_GS>), obj_refs);
            //Serialize blackboard
            serialized_blackboard = GOAP_S.Serialization.SerializationManager.Serialize(_blackboard, typeof(Blackboard_GS), obj_refs);
        }

        public void OnAfterDeserialize() //Deserialize
        {
            //Deserialize action nodes
            _action_nodes = (ActionNode_GS[])GOAP_S.Serialization.SerializationManager.Deserialize(typeof(ActionNode_GS[]), serialized_action_nodes, obj_refs);
            //Count nodes
            for(int k = 0; k < _action_nodes.Length; k++)
            {
                if (_action_nodes[k] != null)
                {
                    _action_node_num++;
                }

            //Deserialize blackboard
            _blackboard = (Blackboard_GS)GOAP_S.Serialization.SerializationManager.Deserialize(typeof(Blackboard_GS), serialized_blackboard, obj_refs);
            if (_blackboard == null)
            {
                _blackboard = new Blackboard_GS();
            }
        }
    }
}