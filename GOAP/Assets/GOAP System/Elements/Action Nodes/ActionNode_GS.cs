using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ActionNode_GS : ISerializationCallbackReceiver {

    //UI fields
    [System.NonSerialized] public ActionNodeUIConfig_GS UI_configuration = new ActionNodeUIConfig_GS(); //The UI configuration of the action node
    [SerializeField] private Rect canvas_pos; //Position of the node window in the editor
    [SerializeField] private bool editable_pos = true; //True means that the user can move the window
    [SerializeField] private string node_id = "null_id"; //Node ID used to set window id
    //Content fields
    [System.NonSerialized] private Action_GS action = null; //Action linked to the action node
    //State fields
    [System.NonSerialized] private bool initialized = false;
    //Serialization fields
    [SerializeField] private string serialized_action; //String where the serialized data is stored
    [SerializeField] private string serialized_UIconfig; //String where the serialized data is stored

    //Constructor =====================
    public ActionNode_GS()
    {

    }

    //Loop Methods ====================
    public void Initialize()
    {
        //Initialize the node UI configuration
        UI_configuration.InitializeConfig();

        initialized = true;
    }

    public void Start()
    {

    }

    public void Update()
    {

    }

    //Get methods =====================
    public bool GetEditablePos()
    {
        return editable_pos;
    }

    public Rect GetCanvasPos()
    {
        canvas_pos = new Rect(canvas_pos.x, canvas_pos.y, 125, 150);
        return canvas_pos;
    }

    public int GetNodeID()
    {
        return node_id.GetHashCode();
    }

    public bool GetInitialized()
    {
        return initialized;
    }

    public Action_GS GetAction()
    {
        return action;
    }

    //Set methods =====================
    public void SetEditablePos(bool val)
    {
        editable_pos = val;
    }

    public void SetCanvasPos(Rect new_pos)
    {
        canvas_pos = new_pos;
    } 
    
    public void SetAction(Action_GS new_action)
    {
        //Set the new action
        action = new_action;
        //Mark scene dirty
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    public void CalculateUUID()
    {
        node_id = System.Guid.NewGuid().ToString();
    }

    //Serialization Methods ===========
    public void OnBeforeSerialize()
    {
        //Serialize the node UI configuration
        serialized_UIconfig = GOAP_S.Serialization.SerializationManager.Serialize(UI_configuration, typeof(ActionNodeUIConfig_GS), null);
        //Serialize the action set
        serialized_action = GOAP_S.Serialization.SerializationManager.Serialize(action, typeof(Action_GS), null);

    }

    public void OnAfterDeserialize()
    {
        //Deserialize the node UI configuration
        UI_configuration = (ActionNodeUIConfig_GS)GOAP_S.Serialization.SerializationManager.Deserialize(typeof(ActionNodeUIConfig_GS), serialized_UIconfig, null);
        //Deserialize the action
        action = (Action_GS)GOAP_S.Serialization.SerializationManager.Deserialize(typeof(Action_GS), serialized_action, null);
    }
}
