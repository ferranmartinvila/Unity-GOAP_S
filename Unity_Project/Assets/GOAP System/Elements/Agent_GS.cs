using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class Agent_GS : MonoBehaviour, ISerializationCallbackReceiver
{
    //State fields
    [SerializeField] private bool initialized = false; //Check if the class is initialized and everithing is allocated correctly
    //Content fields
    [SerializeField] new private string name = "unnamed"; //Agent name(usefull for the user to recognize the behaviours)
    [SerializeField] internal string id = "null"; //Agent UUID
    [System.NonSerialized] private List<ActionNode_GS> action_nodes; //Action nodes list, serialized specially so unity call OnBefore and After methods and we create our custom serialization methods
    //Serialization fields
    [SerializeField] private string serialized_action_nodes; //String where the serialized data is stored
    [SerializeField] private List<Object> obj_refs; //List that contains the references to the objects serialized

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
        new_node.SetCanvasWindow(new Rect(x_pos, y_pos, 100, 100));
        //Calculate the UUID
        new_node.CalculateUUID();
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
    public string s_name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
            //Mark scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    public string s_id
    {
        get
        {
            if(string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }
            return id;
        }

    }
    public string GetAgentId()
    {
        return agent_id;
    }

    public bool GetAgentInit()
    {
        return initialized;
    }

    //Serialization Methods ===========
    public void OnBeforeSerialize() //Serialize
    {
        obj_refs = new List<Object>();
        serialized_action_nodes = GOAP_S.Serialization.SerializationManager.Serialize(action_nodes, typeof(List<ActionNode_GS>), obj_refs);
    }

    public void OnAfterDeserialize() //Deserialize
    {
        action_nodes = (List<ActionNode_GS>)GOAP_S.Serialization.SerializationManager.Deserialize(typeof(List<ActionNode_GS>),serialized_action_nodes, obj_refs);
        if (action_nodes == null) action_nodes = new List<ActionNode_GS>();
    }
}