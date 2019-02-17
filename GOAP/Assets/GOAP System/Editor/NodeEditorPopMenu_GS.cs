using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GOAP_S.UI
{
    public sealed class NodeEditorPopMenu_GS : PopupWindowContent
    {
        [System.NonSerialized] static private NodeEditor_GS target = null; //Focused node editor menu

        //Constructors ================
        public NodeEditorPopMenu_GS(NodeEditor_GS _target)
        {
            target = _target;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300,300);
        }

        public override void OnGUI(Rect rect)
        {
            editorWindow.BeginWindows();

            if (GUILayout.Button("Add Action Node", GUILayout.Width(150), GUILayout.Height(30)))
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

            editorWindow.EndWindows();
        }
    }
}
