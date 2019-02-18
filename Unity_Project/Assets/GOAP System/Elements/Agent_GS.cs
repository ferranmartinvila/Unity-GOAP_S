using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Agent_GS : MonoBehaviour, ISerializationCallbackReceiver
{
    //Content fields
    [SerializeField] private string _name = "un_named"; //Agent name(usefull for the user to recognize the behaviours)
    [SerializeField] internal string _id = "null"; //Agent UUID
    [System.NonSerialized] private List<ActionNode_GS> _action_nodes; //Action nodes list, serialized specially so unity call OnBefore and After methods and we create our custom serialization methods
    [System.NonSerialized] private Blackboard_GS _blackboard;
    //Serialization fields
    [SerializeField] private string serialized_action_nodes; //String where the serialized data is stored
    [SerializeField] private List<UnityEngine.Object> obj_refs; //List that contains the references to the objects serialized

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
        action_nodes.Clear();
        //Mark scene drity
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
    
    public void AddActionNode(float x_pos, float y_pos)
    {
        ActionNode_GS new_node = new ActionNode_GS();
        //Set a position in the node editor canvas
        new_node.window_rect = new Rect(x_pos, y_pos, 100, 100);
        //Add the new node to the action nodes list
        action_nodes.Add(new_node);
        //Mark scene dirty
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    public void DeleteActionNode(ActionNode_GS target)
    {
        action_nodes.Remove(target);
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
            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    public string id
    {
        get
        {
            if(string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString();
            }
            return _id;
        }

    }

    public List<ActionNode_GS> action_nodes
    {
        get
        {
            if(_action_nodes == null)
            {
                _action_nodes = new List<ActionNode_GS>();
            }
            return _action_nodes;
        }
    }

    //Serialization Methods ===========
    public void OnBeforeSerialize() //Serialize
    {
        obj_refs = new List<UnityEngine.Object>();
        serialized_action_nodes = GOAP_S.Serialization.SerializationManager.Serialize(_action_nodes, typeof(List<ActionNode_GS>), obj_refs);
    }

    public void OnAfterDeserialize() //Deserialize
    {
        _action_nodes = (List<ActionNode_GS>)GOAP_S.Serialization.SerializationManager.Deserialize(typeof(List<ActionNode_GS>),serialized_action_nodes, obj_refs);
        if (_action_nodes == null)
        {
            _action_nodes = new List<ActionNode_GS>();
        }
    }
}