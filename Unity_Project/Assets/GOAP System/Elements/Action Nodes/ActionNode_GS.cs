using UnityEngine;
using GOAP_S.PT;

public class ActionNode_GS : ISerializationCallbackReceiver {



    //UI fields
    [SerializeField] private Rect _window_rect; //Position of the node window in the editor
    [SerializeField] private bool _editable_pos = true; //True means that the user can move the window
    [SerializeField] private string _id = ""; //Node ID used to set window 
    [System.NonSerialized] private NodeUIMode _UImode = NodeUIMode.SET_STATE;
    //Content fields
    [SerializeField] private string _name = "Action Node"; //Node name
    [SerializeField] private string _description = ""; //Node description
    [System.NonSerialized] private Action_GS _action = null; //Action linked to the action node
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

    //Get/Set methods =====================
    public bool editable_position
    {
        get
        {
            return _editable_pos;
        }
        set
        {
            _editable_pos = value;
        }
    }
    
    public NodeUIMode UImode
    {
        get
        {
            return _UImode;
        }
        set
        {
            _UImode = value;
        }
    }

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
            if(string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString();
            }
            return _id.GetHashCode();
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
