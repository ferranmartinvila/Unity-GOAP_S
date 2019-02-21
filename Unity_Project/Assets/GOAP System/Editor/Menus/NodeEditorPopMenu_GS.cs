using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    public sealed class NodeEditorPopMenu_GS : PopupWindowContent
    {
        //Content Fields ==============
        static private NodeEditor_GS _target_node_editor = null; //Focused node editor menu

        //Constructors ================
        public NodeEditorPopMenu_GS(NodeEditor_GS target)
        {
            _target_node_editor = target;
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Planning Menu", _target_node_editor.UI_configuration.select_menu_title_style, GUILayout.ExpandWidth(true));
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
                ActionNode_GS new_action_node = _target_node_editor.selected_agent.AddActionNode(_target_node_editor.mouse_position.x, _target_node_editor.mouse_position.y);
                //Add the new node editor
                _target_node_editor.AddTargetAgentNodeUI(new_action_node);
                //Repaint the target window
                _target_node_editor.Repaint();
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
                _target_node_editor.selected_agent.ClearPlanning();
                //Clear editor planning
                _target_node_editor.ClearPlanning();
                //Repaint target window
                _target_node_editor.Repaint();
                //Close this window
                editorWindow.Close();
            }
        }
    }
}
