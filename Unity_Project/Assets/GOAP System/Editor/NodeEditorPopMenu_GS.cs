using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP_S.UI
{
    public sealed class NodeEditorPopMenu_GS : PopupWindowContent
    {
        [System.NonSerialized] static private NodeEditor_GS target = null; //Focused node editor menu
        [System.NonSerialized] static private Vector2 window_size = new Vector2(150,120); //Used to modify window size

        //Constructors ================
        public NodeEditorPopMenu_GS(NodeEditor_GS _target)
        {
            target = _target;
        }

        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Planning Menu",target.nodes_UI_configuration.GetPopWindowsStyle(), GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Add action button
            if (GUILayout.Button(
                "Add Action Node",
                GUILayout.ExpandWidth(true),
                GUILayout.Height(30)))
            {
                //Get the mouse pos inside the target canvas
                Vector2 mousePos = target.GetMousePos();
                //Focus the target selected agent and add an action in the target canvas pos
                target.GetSelectedAgent().AddActionNode(mousePos.x, mousePos.y);
                //Repaint the target window
                target.Repaint();
                //Close this window
                editorWindow.Close();
            }

            //Clear planning button
            if (GUILayout.Button(
                "Clear Plannig",
                GUILayout.ExpandWidth(true),
                GUILayout.Height(30)))
            {
                target.GetSelectedAgent().ClearPlanning();
            }
        }

        //Set Methods =================
        public override Vector2 GetWindowSize()
        {
            return new Vector2(window_size.x, window_size.y);
        }
    }
}
