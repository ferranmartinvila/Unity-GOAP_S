using UnityEngine;
using UnityEditor;

//Class used to draw action nodes in the node editor and handle input
public class ActionNode_GS_Editor {

    //Content fields
    [System.NonSerialized] private NodeEditor_GS target_editor = null;
    [System.NonSerialized] private ActionNode_GS target_node = null;
    [System.NonSerialized] private Action_GS target_action = null;
    //UI fields
    [System.NonSerialized] static private int initial_separation = 0; //Used to define the separation between the first ui element and the window title
    [System.NonSerialized] static private int parts_separation = 0; //Used to define the separation between the diferent action node main elements (condition,action,reward)
    [System.NonSerialized] static private int mark_separation = 0; //Used to define the separation between the elements and the window lateral sides
    [System.NonSerialized] private string name_str = ""; //Used in edit state to allocate the new name
    [System.NonSerialized] private string description_str = ""; //Used in edit state to allocate the new description

    //Constructor =====================
    public ActionNode_GS_Editor(ActionNode_GS new_target, NodeEditor_GS new_editor)
    {
        //Set targets
        target_editor = new_editor;
        target_node = new_target;
        if (target_node != null)
        {
            target_action = target_node.GetAction();
        }

        //Set UI values
        parts_separation = 10;
        initial_separation = 30;
        mark_separation = 10;
    }

    //Loop Methods ====================
    public void DrawNodeWindow(int id)
    {
        switch (target_node.GetUIMode())
        {
            case ActionNode_GS.NodeUIMode.EDIT_STATE:
                //Draw window in edit state
                DrawNodeWindowEditState();
                break;

            case ActionNode_GS.NodeUIMode.SET_STATE:
                //Draw window in set state
                DrawNodeWindowSetState();
                break;
        }
    }

    private void DrawNodeWindowEditState()
    {
        GUILayout.BeginVertical();

        //Node name text field
        GUILayout.BeginHorizontal();
        target_node.SetName(GUILayout.TextField(target_node.GetName(), GUILayout.Width(40), GUILayout.ExpandWidth(true)));
        if(GUILayout.Button("Set", target_editor.nodes_UI_configuration.style_selection_button, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
        {
            target_node.SetName(name_str);
        }
        GUILayout.EndHorizontal();

        //Node description text field


        //Close edit mode
        if (GUILayout.Button(
            "Close",
            target_editor.nodes_UI_configuration.style_modify_button,
            GUILayout.Width(120), GUILayout.ExpandWidth(true)))
        {
            target_node.SetUIMode(ActionNode_GS.NodeUIMode.SET_STATE);
        }

        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    private void DrawNodeWindowSetState()
    { 
        GUILayout.BeginHorizontal();
        //Edit
        if (GUILayout.Button(
            "Edit",
            target_editor.nodes_UI_configuration.style_modify_button,
            GUILayout.Width(30),GUILayout.ExpandWidth(true)))
        {
            //Set edit state
            target_node.SetUIMode(ActionNode_GS.NodeUIMode.EDIT_STATE);
        }
        //Delete
        if (GUILayout.Button(
            "Delete",
            target_editor.nodes_UI_configuration.style_modify_button,
            GUILayout.Width(30),GUILayout.ExpandWidth(true)))
        {
            target_editor.GetSelectedAgent().DeleteActionNode(target_node);
        }
        GUILayout.EndHorizontal();

        //Separation
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Separation
        //GUILayout.Space(parts_separation);

        //Condition -------------------
        //Condition null case
        if (GUILayout.Button(
            "Select Condition",
            target_editor.nodes_UI_configuration.style_selection_button,
            GUILayout.Width(150), GUILayout.Height(20),GUILayout.ExpandWidth(true)))
        {

        }
        //-----------------------------

        //Separation
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Action ----------------------
        //Action null case
        if (target_action == null)
        {

            if (GUILayout.Button(
                "Select Action",
                target_editor.nodes_UI_configuration.style_selection_button,
                GUILayout.Width(150), GUILayout.Height(20),
                GUILayout.ExpandWidth(true)))
            {
                Vector2 mousePos = Event.current.mousePosition;
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y - 100,0,0), new GOAP_S.UI.ActionSelectMenu_GS(this));
            }





        }
        //Action set case
        else
        {
            //Action area
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.FlexibleSpace();
            GUILayout.Label(target_node.GetAction().GetName(), target_editor.nodes_UI_configuration.style_elements, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Edit / Delete area
            GUILayout.BeginHorizontal();
            //Edit
            if (GUILayout.Button(
                "Edit",
                target_editor.nodes_UI_configuration.style_modify_button,
                GUILayout.Width(30), GUILayout.ExpandWidth(true)))
            {
                //IDK what to put here but this can be deleted with no problem :v
            }
            //Delete
            if (GUILayout.Button(
                "Delete",
                target_editor.nodes_UI_configuration.style_modify_button,
                GUILayout.Width(30), GUILayout.ExpandWidth(true)))
            {
                target_node.SetAction(null);
            }
            GUILayout.EndHorizontal();
        }
        //-----------------------------

        //Separation
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Reward ----------------------
        //Reward null case
        if (GUILayout.Button(
            "Select Reward",
            target_editor.nodes_UI_configuration.style_selection_button,
            GUILayout.Width(150), GUILayout.Height(20),
            GUILayout.ExpandWidth(true)))
        {

        }
        //-----------------------------

        GUI.DragWindow();
    }

    //Get Methods =====================
    public void SetAction(Action_GS new_action)
    {
        //Set the new action in the target action node
        target_node.SetAction(new_action);
        //Repaint the node editor to update the UI
        target_editor.Repaint();
    }
}
