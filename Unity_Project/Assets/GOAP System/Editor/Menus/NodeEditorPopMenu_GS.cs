using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP_S.UI
{
    public sealed class NodeEditorPopMenu_GS : PopupWindowContent
    {
        //Content Fields ==============
        static private NodeEditor_GS _target_editor = null; //Focused node editor menu

        //Constructors ================
        public NodeEditorPopMenu_GS(NodeEditor_GS target)
        {
            _target_editor = target;
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Planning Menu", _target_editor.UI_configuration.select_menu_title_style, GUILayout.ExpandWidth(true));
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
                //Get the mouse pos inside the target canvas
                Vector2 mousePos = _target_editor.mouse_pos;
                //Focus the target selected agent and add an action in the target canvas pos
                _target_editor.selected_agent.AddActionNode(mousePos.x, mousePos.y);
                //Repaint the target window
                _target_editor.Repaint();
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
                _target_editor.selected_agent.ClearPlanning();
                //Repaint target window
                _target_editor.Repaint();
                //Close this window
                editorWindow.Close();
            }
        }
    }
}
