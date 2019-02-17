using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MoveAction_GS : Action_GS
{
    public GameObject target_obj = null;

    //Called on the first action loop
    public override ACTION_RESULT ActionStart()
    {
        return ACTION_RESULT.CONTINUE;
    }

    //Called on the action update
    public override ACTION_RESULT ActionUpdate()
    {
        target_obj.transform.position.Set(target_obj.transform.position.x + 0.5f, target_obj.transform.position.y, target_obj.transform.position.z);

        return ACTION_RESULT.CONTINUE;
    }

    //Called when the action ends correctly
    public override void ActionEnd()
    {
        
    }

    /*
    Called when the action process ends with errors
    When an action ends with errors the agent dont look for the next action connected with this
    The actions path is recalculed from the start action node
    */
    public override void ActionBreak()
    {

    }

    public override void BlitUI()
    {
        EditorGUIUtility.ShowObjectPicker<GameObject>(target_obj, true, "", EditorGUIUtility.GetControlID(FocusType.Passive) + 100);

        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            Debug.Log(EditorGUIUtility.GetObjectPickerObject());
        }

        //EditorGUI.ObjectField(new Rect(10, 20, 100, 20), target_obj,typeof(GameObject));
    }
}
