using UnityEngine;

public class ActionNode_GS : ISerializationCallbackReceiver {

    //UI fields
    [SerializeField] private Rect canvas_pos; //Position of the node window in the editor
    [SerializeField] private bool editable_pos = true; //True means that the user can move the window
    [SerializeField] private string node_id = "null_id"; //Node ID used to set window id
    //Content fields
    [System.NonSerialized] private Action_GS action = null; //Action linked to the action node
    //State fields
    [System.NonSerialized] private bool modified = false;
    //Serialization fields
    [SerializeField] private string serialized_data; //String where the serialized data is stored


    //Loop Methods ====================
    void Start()
    {

    }

    void Update()
    {
        //In editor update
        if (Application.isEditor)
        {
            if (modified)
            {
                modified = false;
            }
        }
    }

    //Get methods =====================
    public bool GetEditablePos()
    {
        return editable_pos;
    }

    public Rect GetCanvasPos()
    {
        return canvas_pos;
    }

    public int GetNodeID()
    {
        return node_id.GetHashCode();
    }

    public bool GetModified()
    {
        return modified;
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
        action = new_action;

        modified = true; //Now node is detected as modified
    }

    public void CalculateUUID()
    {
        node_id = System.Guid.NewGuid().ToString();
    }

    //Serialization Methods ===========
    public void OnBeforeSerialize()
    {
        serialized_data = GOAP_S.Serialization.SerializationManager.Serialize(action, typeof(Action_GS), null);
    }

    public void OnAfterDeserialize()
    {
        action = (Action_GS)GOAP_S.Serialization.SerializationManager.Deserialize(typeof(Action_GS), serialized_data, null);
    }
}
