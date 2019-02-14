using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Agent_GS : MonoBehaviour, ISerializationCallbackReceiver
{
    //State fields
    [SerializeField] private bool initialized = false; //Check if the class is initialized and everithing is allocated correctly
    //Content fields
    [SerializeField] private string agent_name = "unnamed"; //Agent name(usefull for the user to recognize the behaviours)
    [SerializeField] internal string agent_id = "null"; //Agent UUID
    [System.NonSerialized] public List<ActionNode_GS> action_nodes; //Action ndoes list, serialized specially so unity call OnBefore and After methods and we create our custom serialization methods
    //Serialization fields
    [SerializeField] private string serialized_data; //String where the serialized data is stored
    [SerializeField] private List<Object> obj_refs; //List that contains the references to the objects serialized

    //Loop Methods ====================
    public bool Initialize()
    {
        //Allocate the action nodes array
        if (action_nodes == null) action_nodes = new List<ActionNode_GS>();

        //Place the start node
        AddStartActionNode();

        //Generate the agent UUID
        agent_id = System.Guid.NewGuid().ToString();

        //Set agent as initialized
        return initialized = true;
    }

    //Reset is called when the user hits the Reset button in the Inspector's context menu
    //or when adding the component the first time.
    private void Reset()
    {
        //Initialize the GOAP agent
        Initialize();
    }

    //Planning Methods ================
    public void AddStartActionNode()
    {
        //Initialize the base action node
        ActionNode_GS start_node = new ActionNode_GS();
        start_node.SetCanvasPos(new Rect(25, 25,100,100));
        start_node.SetEditablePos(false);
        start_node.CalculateUUID();
        //Add it to the action nodes array
        action_nodes.Add(start_node);
    }

    public void ClearPlanning()
    {
        action_nodes.Clear();
    }
    

    /*TEMP*/
    public void AddActionNode(float x_pos, float y_pos)
    {
        ActionNode_GS new_node = new ActionNode_GS();
        new_node.SetCanvasPos(new Rect(x_pos, y_pos, 100, 100));
        new_node.CalculateUUID();
        action_nodes.Add(new_node);
    }

    //Get Methods =====================
    public string GetAgentName()
    {
        return agent_name;
    }

    public string GetAgentId()
    {
        return agent_id;
    }

    public bool GetAgentInit()
    {
        return initialized;
    }
    
    //Set Methods =====================
    public void SetAgentName(string name)
    {
        agent_name = name;
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    //Serialization Methods ===========
    public void OnBeforeSerialize() //Serialize
    {
        obj_refs = new List<Object>();
        serialized_data = GOAP_T.Serialization.SerializationManager.Serialize(action_nodes, typeof(List<ActionNode_GS>), obj_refs);
    }

    public void OnAfterDeserialize() //Deserialize
    {
        action_nodes = (List<ActionNode_GS>)GOAP_T.Serialization.SerializationManager.Deserialize(typeof(List<ActionNode_GS>),serialized_data, obj_refs);
        if (action_nodes == null) action_nodes = new List<ActionNode_GS>();
    }
}