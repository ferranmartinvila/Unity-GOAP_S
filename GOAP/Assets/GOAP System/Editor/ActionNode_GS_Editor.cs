using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Class used to draw action nodes in the node editor and handle input
public class ActionNode_GS_Editor {

    //Content fields
    [System.NonSerialized] private ActionNode_GS target_action_node;
    [System.NonSerialized] private NodeEditor_GS target_editor;

    //Constructor =====================
    public ActionNode_GS_Editor(ActionNode_GS new_target, NodeEditor_GS new_editor)
    {
        target_action_node = new_target;
        target_editor = new_editor;
    }

    //Loop Methods ====================
    public void DrawNodeWindow(int id)
    {
        Action_GS action = target_action_node.GetAction();
        //Action null case
        if (action == null)
        {
            GUIStyle node_title_style = new GUIStyle(GUI.skin.button);

            if (GUI.Button(new Rect(10, 20, 100, 20), "Select Condition", node_title_style))
            {

            }

            
            GUILayout.Space(20);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            Rect last_rect = GUILayoutUtility.GetLastRect();

            if (GUI.Button(new Rect(10, last_rect.y + 15, 100, 20), "Select Action", node_title_style))
            {
                Vector2 mousePos = Event.current.mousePosition;
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y - 100,0,0), new GOAP_S.UI.ActionSelectMenu_GS(this));
            }
            last_rect = GUILayoutUtility.GetLastRect();

            GUILayout.Space(20);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (GUI.Button(new Rect(10, last_rect.y + 15, 100, 20), "Select Reward", node_title_style))
            {

            }

        }
        //Action set case
        else
        {
            GUILayout.Label(target_action_node.UI_configuration.node_title, target_action_node.UI_configuration.node_title_style);

            /*if(GUI.Button(new Rect(10,20,80,20),"Remove Action"))
            {
                target.SetAction(null);
            }

            if (GUI.Button(new Rect(10, 50, 80, 20), "Edit Action"))
            {
                action.BlitUI();
            }*/
        }

        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            /*Object obj = EditorGUIUtility.GetObjectPickerControlID(); //Next step get the action correctly
            action = (Action_GS)obj; ;*/
        }

        GUI.DragWindow();
    }

    //Get Methods =====================
    public void SetAction(Action_GS new_action)
    {
        //Set the new action in the target action node
        target_action_node.SetAction(new_action);
        //Repaint the node editor to update the UI
        target_editor.Repaint();
    }
}
