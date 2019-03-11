using UnityEngine;
using UnityEditor;
using GOAP_S.PT;
using GOAP_S.AI;
using GOAP_S.Planning;
using System.IO;

namespace GOAP_S.UI
{
    //Class used to draw action nodes in the node editor and handle input
    public class ActionNode_GS_Editor
    {

        //Target fields
        private ActionNode_GS _target_node = null;
        //UI fields
        private GUIContent _description_label = null;
        private Vector2 _label_size = Vector2.zero;
        private EditorUIMode _UI_mode = EditorUIMode.SET_STATE;

        //Content fields
        private Property_GS_Editor[] _condition_editors = null;
        private int _condition_editors_num = 0;

        //Constructor =====================
        public ActionNode_GS_Editor(ActionNode_GS new_target)
        {
            //Set targets
            _target_node = new_target;
            //Generate new description ui content
            _description_label = new GUIContent(_target_node.description);
            //Calculate new ui content size
            _label_size = UIConfig_GS.Instance.node_description_style.CalcSize(_description_label);
            //Allocate condition editors array
            _condition_editors = new Property_GS_Editor[ProTools.INITIAL_ARRAY_SIZE];
            //Generate conditions UI
            for (int k = 0; k < _target_node.conditions_num; k++)
            {
                AddConditionEditor(_target_node.conditions[k]);
            }
        }

        //Loop Methods ====================
        public void DrawUI(int id)
        {
            switch (_UI_mode)
            {
                case EditorUIMode.EDIT_STATE:
                    //Draw window in edit state
                    DrawNodeWindowEditState();
                    break;

                case EditorUIMode.SET_STATE:
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
                _label_size = UIConfig_GS.Instance.node_description_style.CalcSize(_description_label);
            }
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            //Close edit mode
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(120), GUILayout.ExpandWidth(true)))
            {
                _UI_mode = EditorUIMode.SET_STATE;
            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void DrawNodeWindowSetState()
        {
            GUILayout.BeginHorizontal();
            //Edit
            if (GUILayout.Button("Edit", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
            {
                //Set edit state
                _UI_mode = EditorUIMode.EDIT_STATE;
            }
            //Delete
            if (GUILayout.Button("Delete", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
            {
                //Delete node in the target agent
                NodeEditor_GS.Instance.selected_agent.DeleteActionNode(_target_node);
                //Delete node editor in the target editor
                NodeEditor_GS.Instance.DeleteTargetAgentNodeUI(this);

                return;
            }
            GUILayout.EndHorizontal();

            //Separation
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Condition -------------------
            //Conditions Title
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Conditions", UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //Show current conditions
            for (int k = 0; k < _condition_editors_num; k++)
            {
                _condition_editors[k].DrawUI();
            }
            //Condition add button
            if (GUILayout.Button("Add Condition", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {
                Vector2 mousePos = Event.current.mousePosition;
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ConditionSelectMenu_GS(this));
            }
            //-----------------------------

            //Separation
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Action ----------------------
            //Action null case
            if (_target_node.action == null)
            {
                if (GUILayout.Button("Select Action", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ActionSelectMenu_GS(this));
                }
            }
            //Action set case
            else
            {
                //Action area
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label(_target_node.action.name, UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //Edit / Delete area
                GUILayout.BeginHorizontal();
                //Edit
                if (GUILayout.Button("Edit", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
                {
                    //Get asset path by adding folder and type
                    string[] file_names = Directory.GetFiles(Application.dataPath, _target_node.action.GetType().ToString() + ".cs", SearchOption.AllDirectories);
                    //Check if there's more than one asset or no asset, in both cases the result is negative
                    if (file_names.Length == 0 || file_names.Length > 1)
                    {
                        Debug.LogError("Asset not found!");
                    }
                    //Asset found case
                    else
                    {
                        //Get asset full path
                        string final_file_name = Path.GetFullPath(file_names[0]);
                        //Open asset in the correct file editor
                        UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(final_file_name, 1);
                    }
                }
                //Delete
                if (GUILayout.Button("Delete", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
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
            if (GUILayout.Button("Select Reward", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {

            }
            //-----------------------------

            GUI.DragWindow();
        }

        //Planning Methods ============
        public void AddCondition(Property_GS new_condition)
        {
            //Add the new condition to the target action node
            _target_node.AddCondition(new_condition);
            //Generate and add a condition editor to this node editor
            AddConditionEditor(new_condition);
        }

        private void AddConditionEditor(Property_GS condition)
        {
            //Generate an editor for the new condition
            Property_GS_Editor property_editor = new Property_GS_Editor(condition, NodeEditor_GS.Instance.selected_agent.blackboard, PropertyUIMode.IS_CONDITION);
            //Add the editor to the correct array
            _condition_editors[_condition_editors_num] = property_editor;
            //Update condition editors count
            _condition_editors_num += 1;
        }

        public void SetAction(Action_GS new_action)
        {
            //Set the new action in the target action node
            _target_node.action = new_action;
            //Repaint the node editor to update the UI
            NodeEditor_GS.Instance.Repaint();
        }

        //Get/Set Methods =================
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
