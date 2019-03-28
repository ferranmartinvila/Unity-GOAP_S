using UnityEditor;
using UnityEngine;
using GOAP_S.AI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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
            //Set window size
            editorWindow.minSize = new Vector2(180.0f, 167.5f);
            editorWindow.maxSize = new Vector2(180.0f, 167.5f);

            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Node Editor Menu", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Add action button
            if (GUILayout.Button(new GUIContent("Add Action Node", "Add action node to the selected agent"),
                GUILayout.ExpandWidth(true),
                GUILayout.Height(25)))
            {
                //Focus the target selected agent and add an action in the target canvas pos
                Vector2 mouse_pos = NodeEditor_GS.Instance.ScreenCoordsToZoomCoords(editorWindow.position.position);
                ActionNode_GS new_action_node = NodeEditor_GS.Instance.selected_agent.AddActionNode(mouse_pos);
                //Add the new node editor
                NodeEditor_GS.Instance.AddTargetAgentActionNodeEditor(new_action_node);
                //Repaint the target window
                NodeEditor_GS.Instance.Repaint();
                //Close this window
                editorWindow.Close();
            }

            //Clear planning button
            if (GUILayout.Button(new GUIContent("Clear Plannig", "Remove all action nodes of the selected agent"),
                GUILayout.ExpandWidth(true),
                GUILayout.Height(25)))
            {
                //Add clear agent planning
                SecurityAcceptMenu_GS.on_accept_delegate += () => NodeEditor_GS.Instance.selected_agent.ClearPlanning();
                //Add clear editor planning
                SecurityAcceptMenu_GS.on_accept_delegate += () => NodeEditor_GS.Instance.ClearPlanning();
                //Add repaint target window
                SecurityAcceptMenu_GS.on_accept_delegate += () => NodeEditor_GS.Instance.Repaint();
                //Add close this window
                SecurityAcceptMenu_GS.on_accept_delegate += () => this.editorWindow.Close();
                //Add close this window on cancel
                SecurityAcceptMenu_GS.on_cancel_delegate += () => this.editorWindow.Close();
                //Get mouse current position
                Vector2 mousePos = Event.current.mousePosition;
                //Open security accept menu on mouse position
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new SecurityAcceptMenu_GS());
            }

            //Close agent canvas button
            if (GUILayout.Button(new GUIContent("Close Agent Canvas", "Close GoapSystem agent canvas"),
                GUILayout.ExpandWidth(true),
                GUILayout.Height(25)))
            {
                //Close node editor
                NodeEditor_GS.Instance.Close();
                NodePlanning_GS.Instance.Close();
                //Close this window
                editorWindow.Close();
            }

            //Remove agent button
            GUI.backgroundColor = new Color(1.0f, 0.2f, 0.2f, 1.0f);
            if (GUILayout.Button("Remove Agent", GUILayout.ExpandWidth(true), GUILayout.Height(25))) 
            {
                //Add remove this agent to accept menu delegates callback
                SecurityAcceptMenu_GS.on_accept_delegate += () => Object.DestroyImmediate((Agent_GS)NodeEditor_GS.Instance.selected_agent);
                //Add mark scene dirty to accept menu delegates callback
                SecurityAcceptMenu_GS.on_accept_delegate += () => EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                //Blackboard will detect that there's no agent and will destroy itself
                //Add node planning repaint
                SecurityAcceptMenu_GS.on_accept_delegate += () => NodePlanning_GS.Instance.Repaint();
                //Add node editor repaint
                SecurityAcceptMenu_GS.on_accept_delegate += () => NodeEditor_GS.Instance.Repaint();
                //Add close this window
                SecurityAcceptMenu_GS.on_accept_delegate += () => this.editorWindow.Close();
                //Add close this window on cancel
                SecurityAcceptMenu_GS.on_cancel_delegate += () => this.editorWindow.Close();
                //Get mouse current position
                Vector2 mousePos = Event.current.mousePosition;
                //Open security accept menu on mouse position
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new SecurityAcceptMenu_GS());
            }
            GUI.backgroundColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        }
    }
}
