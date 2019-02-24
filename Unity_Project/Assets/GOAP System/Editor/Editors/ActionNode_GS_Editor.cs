using UnityEngine;
using UnityEditor;
using GOAP_S.PT;
using GOAP_S.AI;

namespace GOAP_S.UI
{
    //Class used to draw action nodes in the node editor and handle input
    public class ActionNode_GS_Editor
    {

        //Target fields
        private NodeEditor_GS _target_node_editor = null;
        private ActionNode_GS _target_node = null;
        //Content fields
        private GUIContent _description_label = null;
        private Vector2 _label_size = Vector2.zero;

        //Constructor =====================
        public ActionNode_GS_Editor(ActionNode_GS new_target, NodeEditor_GS new_editor)
        {
            //Set targets
            _target_node_editor = new_editor;
            _target_node = new_target;
            //Generate new description ui content
            _description_label = new GUIContent(_target_node.description);
            //Calculate new ui content size
            _label_size = _target_node_editor.UI_configuration.node_description_style.CalcSize(_description_label);
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
            string prev_description = _target_node.description;
            _target_node.description = GUILayout.TextArea(_target_node.description, GUILayout.Width(250), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.ExpandWidth(true)))
            {
                _target_node.description = "";
            }
            //Check if description has been modified
            if(_target_node.description != prev_description)
            {
                //Generate new description ui content
                _description_label = new GUIContent(_target_node.description);
                //Calculate new ui content size
                _label_size = _target_node_editor.UI_configuration.node_description_style.CalcSize(_description_label);
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            //Close edit mode
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.Width(120), GUILayout.ExpandWidth(true)))
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
            if (_target_node.action == null)
            {

                if (GUILayout.Button("Select Action", _target_node_editor.UI_configuration.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ActionSelectMenu_GS(this, _target_node_editor));
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

        public GUIContent description_label
        {
            get
            {
                return _description_label;
            }
        }

        public Vector2 label_size
        {
            get
            {
                return _label_size;
            }
        }
    }
}
