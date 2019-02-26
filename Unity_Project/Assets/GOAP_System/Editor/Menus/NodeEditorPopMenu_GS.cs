using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    public sealed class NodeEditorPopMenu_GS : PopupWindowContent
    {
        //Constructors ================
        public NodeEditorPopMenu_GS()
        {

        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Planning Menu", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Add action button
            if (GUILayout.Button(
                "Add Action Node",
                GUILayout.ExpandWidth(true),
                GUILayout.Height(25)))
            {
                //Focus the target selected agent and add an action in the target canvas pos
                ActionNode_GS new_action_node = NodeEditor_GS.Instance.selected_agent.AddActionNode(NodeEditor_GS.Instance.mouse_position.x, NodeEditor_GS.Instance.mouse_position.y);
                //Add the new node editor
                NodeEditor_GS.Instance.AddTargetAgentNodeUI(new_action_node);
                //Repaint the target window
                NodeEditor_GS.Instance.Repaint();
                //Close this window
                editorWindow.Close();
            }

            //Clear planning button
            if (GUILayout.Button(
                "Clear Plannig",
                GUILayout.ExpandWidth(true),
                GUILayout.Height(25)))
            {
                //Clear agent planning
                NodeEditor_GS.Instance.selected_agent.ClearPlanning();
                //Clear editor planning
                NodeEditor_GS.Instance.ClearPlanning();
                //Repaint target window
                NodeEditor_GS.Instance.Repaint();
                //Close this window
                editorWindow.Close();
            }
        }
    }
}
