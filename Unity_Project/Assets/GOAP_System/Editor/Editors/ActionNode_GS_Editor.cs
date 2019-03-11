using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using GOAP_S.Tools;
using GOAP_S.AI;
using GOAP_S.Planning;
using System.IO;

namespace GOAP_S.UI
{
    //Class used to draw action nodes in the node editor and handle input
    public class ActionNode_GS_Editor
    {

        //Target fields
        private ActionNode_GS _target_action_node = null;
        //UI fields
        private GUIContent _description_label = null;
        private Vector2 _label_size = Vector2.zero;
        private bool _label_allocated = false; //Weird boolean to avoid Unity error display
        private EditorUIMode _UI_mode = EditorUIMode.SET_STATE;

        //Content fields
        private Property_GS_Editor[] _condition_editors = null;
        private int _condition_editors_num = 0;
        private Property_GS_Editor[] _effect_editors = null;
        private int _effect_editors_num = 0;

        //Constructor =====================
        public ActionNode_GS_Editor(ActionNode_GS new_target)
        {
            //Set targets
            _target_action_node = new_target;
            //Generate new description ui content
            _description_label = new GUIContent(_target_action_node.description);
            //Calculate new ui content size
            
            //Allocate condition editors array
            _condition_editors = new Property_GS_Editor[ProTools.INITIAL_ARRAY_SIZE];
            //Generate conditions UI
            for (int k = 0; k < _target_action_node.conditions_num; k++)
            {
                AddConditionEditor(_target_action_node.conditions[k]);
            }
            //Allocate effect editors array
            _effect_editors = new Property_GS_Editor[ProTools.INITIAL_ARRAY_SIZE];
            //Generate conditions UI
            for (int k = 0; k < _target_action_node.effects_num; k++)
            {
                AddEffectEditor(_target_action_node.effects[k]);
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
            _target_action_node.name = GUILayout.TextField(_target_action_node.name, GUILayout.Width(90), GUILayout.ExpandWidth(true));
            if(GUILayout.Button("X",GUILayout.Width(20),GUILayout.ExpandWidth(false)))
            {
                _target_action_node.name = "";
            }
            GUILayout.EndHorizontal();

            //Node description text field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Description");
            string prev_description = _target_action_node.description;
            _target_action_node.description = GUILayout.TextArea(_target_action_node.description, GUILayout.Width(250), GUILayout.ExpandWidth(true));
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.ExpandWidth(true)))
            {
                _target_action_node.description = "";
            }
            //Check if description has been modified
            if(_target_action_node.description != prev_description)
            {
                //Generate new description ui content
                _description_label = new GUIContent(_target_action_node.description);
                //Calculate new ui content size
                _label_size = UIConfig_GS.left_white_style.CalcSize(_description_label);
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
                _target_action_node.agent.RemoveActionNode(_target_action_node);
                //Delete node editor in the target editor
                NodeEditor_GS.Instance.RemoveTargetAgentActionNodeEditor(this);
                //Mark scene dirty
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
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
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new PropertySelectMenu_GS(this, PropertyUIMode.IS_CONDITION));
            }
            //-----------------------------

            //Separation
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Action ----------------------
            //Action null case
            if (_target_action_node.action == null)
            {
                //Action area
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label("No Action", UIConfig_GS.center_big_white_style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //Action select dropdown


                if (GUILayout.Button("Set Action", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new ActionSelectMenu_GS(_target_action_node));
                }
            }
            //Action set case
            else
            {
                //Action area
                GUILayout.BeginHorizontal("HelpBox");
                GUILayout.FlexibleSpace();
                GUILayout.Label(_target_action_node.action.name, UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                //Draw selected action UI
                _target_action_node.action.BlitUI();

                //Edit / Delete area
                GUILayout.BeginHorizontal();
                //Edit
                if (GUILayout.Button("Edit", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
                {
                    //Open target script code editor
                    ProTools.OpenScriptEditor(_target_action_node.action.GetType());
                }
                //Delete
                if (GUILayout.Button("Delete", UIConfig_GS.Instance.node_modify_button_style, GUILayout.Width(30), GUILayout.ExpandWidth(true)))
                {
                    //Set action node action to null
                    _target_action_node.action = null;
                    //Mark scene dirty
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
                GUILayout.EndHorizontal();
            }
            //-----------------------------

            //Separation
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Effects ----------------------
            //Effects Title
            GUILayout.BeginHorizontal("HelpBox");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Effects", UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //Show current effects
            for (int k = 0; k < _effect_editors_num; k++)
            {
                _effect_editors[k].DrawUI();
            }
            //Effect add button
            if (GUILayout.Button("Add Effect", UIConfig_GS.Instance.node_selection_buttons_style, GUILayout.Width(150), GUILayout.Height(20), GUILayout.ExpandWidth(true)))
            {
                Vector2 mousePos = Event.current.mousePosition;
                PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new PropertySelectMenu_GS(this, PropertyUIMode.IS_EFFECT));
            }
            //-----------------------------

            GUI.DragWindow();
        }

        //Planning Methods ============
        public void AddCondition(Property_GS new_condition)
        {
            //Add the new condition to the target action node
            _target_action_node.AddCondition(new_condition);
            //Generate and add a condition editor to this node editor
            AddConditionEditor(new_condition);
        }

        private void AddConditionEditor(Property_GS condition)
        {
            //Generate an editor for the new condition
            Property_GS_Editor property_editor = new Property_GS_Editor(condition, this, _target_action_node.agent.blackboard, PropertyUIMode.IS_CONDITION);
            //Add the editor to the correct array
            _condition_editors[_condition_editors_num] = property_editor;
            //Update condition editors count
            _condition_editors_num += 1;
        }

        public void RemoveConditionEditor(Property_GS_Editor target_condition_editor)
        {
            //First search property editor
            for (int k = 0; k < _condition_editors_num; k++)
            {
                if (_condition_editors[k] == target_condition_editor)
                {
                    //Last condition editor case
                    if (k == _condition_editors.Length - 1)
                    {
                        _condition_editors[k] = null;
                    }
                    else
                    {
                        //When property is found copy values in front of it a slot backwards
                        for (int n = k; n < _condition_editors_num - 1; n++)
                        {
                            _condition_editors[n] = _condition_editors[n + 1];
                        }
                    }
                    //Update condition editors count
                    _condition_editors_num -= 1;
                    //Mark scene dirty
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    //Job is done we dont need to continue the iteration
                    return;
                }
            }
            //Porperty editor not found case
            Debug.LogWarning("Condition: " + target_condition_editor.target_property.A_key + " not found on remove!");
        }

        public void AddEffect(Property_GS new_effect)
        {
            //Add the new effect to the target action node
            _target_action_node.AddEffect(new_effect);
            //Generate and add a effect editor to this node editor
            AddEffectEditor(new_effect);
        }

        private void AddEffectEditor(Property_GS effect)
        {
            //Generate an editor for the new condition
            Property_GS_Editor property_editor = new Property_GS_Editor(effect, this, _target_action_node.agent.blackboard, PropertyUIMode.IS_EFFECT);
            //Add the editor to the correct array
            _effect_editors[_effect_editors_num] = property_editor;
            //Update effect editors count
            _effect_editors_num += 1;
        }

        public void RemoveEffectEditor(Property_GS_Editor target_effect_editor)
        {
            //First search property editor
            for (int k = 0; k < _effect_editors_num; k++)
            {
                if (_effect_editors[k] == target_effect_editor)
                {
                    //Last effect editor case
                    if (k == _effect_editors.Length - 1)
                    {
                        _effect_editors[k] = null;
                    }
                    else
                    {
                        //When property is found copy values in front of it a slot backwards
                        for (int n = k; n < _effect_editors_num - 1; n++)
                        {
                            _effect_editors[n] = _effect_editors[n + 1];
                        }
                    }
                    //Update effect editors count
                    _effect_editors_num -= 1;
                    //Mark scene dirty
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    //Job is done we dont need to continue the iteration
                    return;
                }
            }
            //Porperty editor not found case
            Debug.LogWarning("Effect: " + target_effect_editor.target_property.A_key + " not found on remove!");
        }

        //Get/Set Methods =================
        public ActionNode_GS target_action_node
        {
            get
            {
                return _target_action_node;
            }
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
                if(!_label_allocated)
                {
                    _label_size = UIConfig_GS.left_white_style.CalcSize(_description_label);
                }
                return _label_size;
            }
        }
    }
}
