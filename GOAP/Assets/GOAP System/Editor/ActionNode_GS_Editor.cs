using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Class used to draw action nodes in the node editor and handle input
public class ActionNode_GS_Editor {

    //Content fields
    private ActionNode_GS target;

    //Constructor =====================
    public ActionNode_GS_Editor(ActionNode_GS new_target)
    {
        target = new_target;
    }

    //Loop Methods ====================
    public void DrawNodeWindow(int id)
    {
        Action_GS action = target.GetAction();
        //Action null case
        if (action == null)
        {
            if (GUI.Button(new Rect(10, 20, 100, 20), "Add Action"))
            {
                Vector2 mousePos = Event.current.mousePosition;
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y - 100,0,0), new GOAP_S.UI.ActionSelectMenu_GS(target));
            }
        }
        //Action set case
        else
        {
            //GUILayout.
            if(GUI.Button(new Rect(10,20,80,20),"Remove Action"))
            {
                target.SetAction(null);
            }

            if (GUI.Button(new Rect(10, 50, 80, 20), "Edit Action"))
            {
                action.BlitUI();
            }
        }

        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            /*Object obj = EditorGUIUtility.GetObjectPickerControlID(); //Next step get the action correctly
            action = (Action_GS)obj; ;*/
        }

        GUI.DragWindow();
    }


    //Set Methods =====================
    public void SetTarget(ActionNode_GS new_target)
    {
        target = new_target;
    }
}
