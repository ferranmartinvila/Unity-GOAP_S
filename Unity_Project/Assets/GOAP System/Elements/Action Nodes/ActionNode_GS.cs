using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ActionNode_GS : ISerializationCallbackReceiver {

    public enum NodeUIMode
    {
        SET_STATE, //State in which the user can set action node attributes
        EDIT_STATE //State in which the user can set node description/name
    }

    //UI fields
    [SerializeField] private Rect canvas_window; //Position of the node window in the editor
    [SerializeField] private bool editable_pos = true; //True means that the user can move the window
    [SerializeField] private string node_id = "null_id"; //Node ID used to set window 
    [System.NonSerialized] private NodeUIMode nodeUI_mode = NodeUIMode.SET_STATE;
    //Content fields
    [SerializeField] private string name = "Action Node"; //Node name
    [SerializeField] private string description = ""; //Node description
    [System.NonSerialized] private Action_GS action = null; //Action linked to the action node
    //Serialization fields
    [SerializeField] private string serialized_action; //String where the serialized data is stored

    //Constructor =====================
    public ActionNode_GS()
    {

    }

    //Loop Methods ====================
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

    public NodeUIMode GetUIMode()
    {
        return nodeUI_mode;
    }

    public Rect GetCanvasWindow()
    {
        return canvas_window;
    }

    public int GetNodeID()
    {
        return node_id.GetHashCode();
    }

    public Action_GS GetAction()
    {
        return action;
    }

    public string GetName()
    {
        return name;
    }

    public string GetDescription()
    {
        return description;
    }

    //Set methods =====================
    public void SetEditablePos(bool val)
    {
        editable_pos = val;
    }

    public void SetCanvasWindow(Rect new_canvas)
    {
        canvas_window = new_canvas;
    }

    public void SetCanvasPos(Vector2 new_pos)
    {
        canvas_window.x = new_pos.x;
        canvas_window.y = new_pos.y;
    }

    public void SetCanvasSize(Vector2 new_size)
    {
        canvas_window.width = new_size.x;
        canvas_window.height = new_size.y;
    }

    public void SetAction(Action_GS new_action)
    {
        //Set the new action
        action = new_action;
        //Mark scene dirty
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    public void SetUIMode(NodeUIMode new_mode)
    {
        nodeUI_mode = new_mode;
    }

    public void CalculateUUID()
    {
        node_id = System.Guid.NewGuid().ToString();
    }

    public void SetName(string new_name)
    {
        name = new_name;
    }

    public void SetDescription(string new_description)
    {
        description = new_description;
    }

    //Serialization Methods ===========
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
