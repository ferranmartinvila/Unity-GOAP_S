using UnityEditor;
using UnityEngine;
using GOAP_S.Planning;
using GOAP_S.Blackboard;
using GOAP_S.PT;

namespace GOAP_S.UI
{
    public class Property_GS_Editor
    {
        //Content fields
        private EditorUIMode _UI_mode = EditorUIMode.SET_STATE; //Swap between edit and set state, so the suer can edit values or not
        private PropertyUIMode _property_UI_mode = PropertyUIMode.IS_UNDEFINED; //Depending of the mode the user will be able to modify the properties with different options
        private Property_GS _target_property = null; //The property is this editor working with
        private ActionNode_GS_Editor _target_action_node_editor = null; //The node editor is this property editor showing
        private Blackboard_GS _target_blackboard = null; //The blackboard is this editor working with
        //Edit fields
        private OperatorType[] _valid_operators = null;
        private int _selected_operator_index = -1;
        private string[] _B_variable_keys = null;
        private int _selected_B_key_index = -1;
        private int _operator_dropdown_slot = -1;
        private int _variable_dropdown_slot = -1;
        private object value = null;

        //Constructors
        public Property_GS_Editor(Property_GS new_property,ActionNode_GS_Editor new_action_node_editor, Blackboard_GS new_bb, PropertyUIMode new_ui_mode)
        {
            //Set target property
            _target_property = new_property;
            //Set target action node editor
            _target_action_node_editor = new_action_node_editor;
            //Set target bb
            _target_blackboard = new_bb;
            //Set property editor UI mode
            _property_UI_mode = new_ui_mode;
            //Allocate the valid operators
            if(_property_UI_mode == PropertyUIMode.IS_CONDITION)
            {
                _valid_operators = ProTools.GetValidPassiveOperatorTypesFromVariableType(_target_property.variable_type);
            }
            else if(_property_UI_mode == PropertyUIMode.IS_EFFECT)
            {
                _valid_operators = ProTools.GetValidActiveOperatorTypesFromVariableType(_target_property.variable_type);
            }
            //Get variables in the blackboard with the same type
            _B_variable_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeysByVariableType(_target_property.variable_type);
        }

        //Loop Methods ====================
        public void DrawUI()
        {
            //Draw property in edit mode
            if(_UI_mode == EditorUIMode.EDIT_STATE)
            {
                DrawInEdit();
            }
            //Draw property in show mode
            else
            {
                DrawInShow();
            }
        }

        private void DrawInShow()
        {
            GUILayout.BeginHorizontal();

            //Edit button, swap between edit and show state(hide on play)
            if (!Application.isPlaying)
            {
                if (GUILayout.Button("O", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    //Change UI mode
                    _UI_mode = EditorUIMode.EDIT_STATE;
                    //Get dropdown slots
                    _operator_dropdown_slot = ProTools.GetDropdownSlot();
                    _variable_dropdown_slot = ProTools.GetDropdownSlot();
                    //Initialize value
                    value = _target_property.value;
                    //Search and set the already selected B key index
                    //Iterate avaliable B keys
                    for (int k = 0; k < _B_variable_keys.Length; k++)
                    {
                        if (string.Compare(_B_variable_keys[k], _target_property.B_key) == 0)
                        {
                            //When key is found save index and break the iteration
                            _selected_B_key_index = k;
                            ProTools.SetDropdownIndex(_variable_dropdown_slot, _selected_B_key_index);
                            break;
                        }
                    }
                }
            }

            //A key label
            GUILayout.Label(_target_property.A_key, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(60.0f));

            //Operator label
            GUILayout.Label(" " + _target_property.operator_type.ToShortString() + " ", GUILayout.MaxWidth(30.0f));

            //B value label
            if(string.IsNullOrEmpty(_target_property.B_key))
            {
                //Value case
                GUILayout.Label(_target_property.display_value, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(60.0f));
            }
            else
            {
                //Variable case
                GUILayout.Label(_target_property.B_key, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(60.0f));
            }

            //Delete button(hide on play)
            if (!Application.isPlaying)
            {
                if (GUILayout.Button(new GUIContent("X", "Remove"), GUILayout.MaxWidth(20.0f)))
                {
                    //We need to check if the target property is a condition or an effect to delete it from the correct container
                    switch(_property_UI_mode)
                    {
                        case PropertyUIMode.IS_CONDITION:
                            {
                                //Add remove the current property from target action node conditions
                                SecurityAcceptMenu_GS.on_accept_delegate += () => _target_action_node_editor.target_action_node.RemoveCondition(_target_property);
                                //Add remove current property editor from target acton node editor conditions editors
                                SecurityAcceptMenu_GS.on_accept_delegate += () => _target_action_node_editor.RemoveConditionEditor(this);
                            }
                            break;
                        case PropertyUIMode.IS_EFFECT:
                            {
                                break;
                            }
                    }
                    //Get mouse current position
                    Vector2 mousePos = Event.current.mousePosition;
                    //Open security accept menu on mouse position
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new SecurityAcceptMenu_GS());
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawInEdit()
        {
            GUILayout.BeginHorizontal();

            //Edit button, swap between edit and show state
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)) || Application.isPlaying)
            {
                //Change UI mode
                _UI_mode = EditorUIMode.SET_STATE;
                //Set input focus to null
                GUI.FocusControl("null");
                //Free dropdown slots
                ProTools.FreeDropdownSlot(_operator_dropdown_slot);
                ProTools.FreeDropdownSlot(_variable_dropdown_slot);
                //Check B key
                if(string.Compare(_target_property.B_key,"Not Set") == 0 || string.IsNullOrEmpty(_target_property.B_key))
                {
                    //If property B key is not ser property will use value
                    _target_property.B_key = null;
                    //If value is different we update the variable value
                    if (value != _target_property.value)
                    {
                        _target_property.value = value;
                    };
                }
                return;
            }

            //A key label
            GUILayout.Label(_target_property.A_key, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(60.0f));

            //Operator dropdown
            ProTools.GenerateButtonDropdownMenu(ref _selected_operator_index, _valid_operators.ToShortString(), _target_property.operator_type.ToShortString(), true, 30.0f, _operator_dropdown_slot);
            //Check operator change
            if (_selected_operator_index!= -1 && _valid_operators[_selected_operator_index] != _target_property.operator_type)
            {
                _target_property.operator_type = _valid_operators[_selected_operator_index];
            }

            //Value area
            //First check if the target property uses a value or a variable
            if(string.IsNullOrEmpty(_target_property.B_key))
            {
                //Generate UI field from type
                ProTools.ValueFieldByVariableType(_target_property.variable_type, ref value);
                
                //Change to variable button
                if(GUILayout.Button(new GUIContent(" ","Change to variable"),GUILayout.MaxWidth(10.0f)))
                {
                    //If target property variable string is null we set an informative(not set)
                    if (string.IsNullOrEmpty(_target_property.B_key))
                    {
                        _target_property.B_key = "Not Set";
                    }
                }
            }
            else
            {
                //Generate enumerator popup with the avaliable B keys
                ProTools.GenerateButtonDropdownMenu(ref _selected_B_key_index, _B_variable_keys, "Not Set", true, 80.0f, _variable_dropdown_slot);
                //Check B key change
                if (_selected_B_key_index != -1 && string.Compare(_target_property.B_key,_B_variable_keys[_selected_B_key_index]) != 0)
                {
                    //Update B key on change
                    _target_property.B_key = _B_variable_keys[_selected_B_key_index];
                }

                //Change to value button
                if (GUILayout.Button(new GUIContent(" ", "Change to value"), GUILayout.MaxWidth(10.0f)))
                {
                    _target_property.B_key = null;
                }
            }
            GUILayout.EndHorizontal();
        }

        //Get/Set Methods =============
        public Property_GS target_property
        {
            get
            {
                return _target_property;
            }
        }
    }
}
