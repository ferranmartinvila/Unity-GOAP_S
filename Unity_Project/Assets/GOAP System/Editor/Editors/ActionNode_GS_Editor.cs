using UnityEngine;
using UnityEditor;
using GOAP_S.PT;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    //Class used to draw action nodes in the node editor and handle input
    public class ActionNode_GS_Editor
    {

        //Content fields
        private NodeEditor_GS _target_node_editor = null;
        private ActionNode_GS _target_node = null;
        private Action_GS _target_action = null;

        //Constructor =====================
        public ActionNode_GS_Editor(ActionNode_GS new_target, NodeEditor_GS new_editor)
        {
            //Set targets
            _target_node_editor = new_editor;
            _target_node = new_target;
            if (_target_node != null)
            {
                _target_action = _target_node.action;
            }
        }

        //Loop Methods ====================
        public void DrawUI(int id)
        {
            switch (_target_node.UImode)
            {
                case NodeUIMode.EDIT_STATE:
                    //Draw window in edit state
                    DrawNodeWindowEditState();
                    break;

                case NodeUIMode.SET_STATE:
                    //Draw window in set state
                    DrawNodeWindowSetState();
                    break;
            }
        }

        private void DrawNodeWindowEditState()
        {
            //Node name text field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            _target_node.name = GUILayout.TextField(_target_node.name, GUILayout.Width(90), GUILayout.ExpandWidth(true));
            if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.ExpandWidth(false)))
            {
                _target_node.name = "";
            }
            GUILayout.EndHorizontal();

            //Node description text field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Description");
            _target_node.description = GUILayout.TextArea(_target_node.description, GUILayout.Width(250), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.ExpandWidth(true)))
            {
                _target_node.description = "";
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            //Close edit mode
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(
                "Close",
                _target_node_editor.UI_configuration.node_modify_button_style,
                GUILayout.Width(120), GUILayout.ExpandWidth(true)))
            {
                _target_node.UImode = NodeUIMode.SET_STATE;
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void DrawNodeWindowSetState()
        {
            GUILayout.BeginHorizontal();
            //Edit
            if (GUILayout.Button("Edit", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
            {
                //Set edit state
                _target_node.UImode = NodeUIMode.EDIT_STATE;
            }
            //Delete
            if (GUILayout.Button("Delete", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
            {
                //Delete node in the target agent
                _target_node_editor.selected_agent.DeleteActionNode(_target_node);
                //Delete node editor in the target editor
                _target_node_editor.DeleteTargetAgentNodeUI(this);

                return;
            }
            GUILayout.EndHorizontal();

            //Separation
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Separation
            //GUILayout.Space(parts_separation);

            //Condition -------------------
            //Condition null case
            if (GUILayout.Button("Select Condition", _target_node_editor.UI_configuration.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {

            }
            //-----------------------------

            //Separation
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Action ----------------------
            //Action null case
            if (_target_action == null)
            {

                if (GUILayout.Button("Select Action", _target_node_editor.UI_configuration.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new GOAP_S.UI.ActionSelectMenu_GS(this, _target_node_editor));
                }
            }
            //Action set case
            else
            {
                //Action area
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label(_target_node.action.name, _target_node_editor.UI_configuration.node_elements_style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //Edit / Delete area
                GUILayout.BeginHorizontal();
                //Edit
                if (GUILayout.Button("Edit", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
                {
                    //IDK what to put here but this can be deleted with no problem :v
                }
                //Delete
                if (GUILayout.Button("Delete", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
                {
                    _target_node.action = null;
                }
                GUILayout.EndHorizontal();
            }
            //-----------------------------

            //Separation
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Reward ----------------------
            //Reward null case
            if (GUILayout.Button("Select Reward", _target_node_editor.UI_configuration.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {

            }
            //-----------------------------

            GUI.DragWindow();
        }

        //Get/Set Methods =================
        public void SetAction(Action_GS new_action)
        {
            //Set the new action in the target action node
            _target_node.action = new_action;
            //Repaint the node editor to update the UI
            _target_node_editor.Repaint();
        }
    }
}
