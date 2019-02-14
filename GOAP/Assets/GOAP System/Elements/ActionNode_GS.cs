using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionNode_GS { //Object?

    [SerializeField] private Rect canvas_pos;
    [SerializeField] private bool editable_pos = true;
    [SerializeField] private string node_id = "null_id";
    [SerializeField] private Action_GS action = null;

    //Loop Methods ====================
    void Start ()
    {
      
	}
	
	void Update () {

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
    }

    public void CalculateUUID()
    {
        node_id = System.Guid.NewGuid().ToString();
    }
}
