using UnityEditor;
using UnityEngine;
using GOAP_S.Planning;
using GOAP_S.Blackboard;
using GOAP_S.Tools;

namespace GOAP_S.UI
{
    public class Property_GS_Editor
    {
        //Content fields
        private EditorUIMode _UI_mode = EditorUIMode.SET_STATE; //Swap between edit and set state, so the user can edit values or not
        private PropertyUIMode _property_UI_mode = PropertyUIMode.IS_UNDEFINED; //Depending of the mode the user will be able to modify the properties with different options
        private bool edit_value = true; //True if target property uses value, false if uses a variable
        private Property_GS _target_property = null; //The property is this editor working with
        private ActionNode_GS_Editor _target_action_node_editor = null; //The node editor is this property editor showing
        //Edit fields
        private OperatorType[] _valid_operators = null; //Operators user can choose
        ProTools.DropDownData_GS _operator_dropdown = null; //Operador dropdown data
        ProTools.DropDownData_GS _B_variable_dropdown = null; //B variable dropdown data
        private object value = null; //Value selected by user in property using value case

        //Constructors
        public Property_GS_Editor(Property_GS new_property,ActionNode_GS_Editor new_action_node_editor, Blackboard_GS new_bb, PropertyUIMode new_ui_mode)
        {
            //Set target property
            _target_property = new_property;
            //Set target action node editor
            _target_action_node_editor = new_action_node_editor;
            //Set property editor UI mode
            _property_UI_mode = new_ui_mode;
            //Allocate the valid operators
            if(_property_UI_mode == PropertyUIMode.IS_CONDITION)
            {
                _valid_operators = _target_property.variable_type.GetValidPassiveOperatorTypes();
            }
            else if(_property_UI_mode == PropertyUIMode.IS_EFFECT)
            {
                _valid_operators = _target_property.variable_type.GetValidActiveOperatorTypes();
            }
            //Check if the target property has a B key to adapt UI
            edit_value = string.IsNullOrEmpty(_target_property.B_key);
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
                if (GUILayout.Button("O", GUILayout.Width(30), GUILayout.Height(20),GUILayout.ExpandWidth(false)))
                {
                    //Initialize value
                    value = _target_property.value;
                    //Change UI mode
                    _UI_mode = EditorUIMode.EDIT_STATE;
                    //Allocate B var dropdown
                    _B_variable_dropdown = new ProTools.DropDownData_GS();
                    //Allocate operator dropdown
                    _operator_dropdown = new ProTools.DropDownData_GS();
                    
                    //Get dropdown slots
                    _operator_dropdown.dropdown_slot = ProTools.GetDropdownSlot();
                    _B_variable_dropdown.dropdown_slot = ProTools.GetDropdownSlot();
                    
                    //Generate operators dropdown data
                    _operator_dropdown.paths = _valid_operators.ToShortStrings();
                    for (int k = 0; k < _valid_operators.Length; k++)
                    {
                        if (_valid_operators[k] == _target_property.operator_type)
                        {
                            ProTools.SetDropdownIndex(_operator_dropdown.dropdown_slot, k);
                            _operator_dropdown.selected_index = k;
                        }
                    }

                    //Get local blackboard variables keys with the same type
                    string[] local_keys = NodeEditor_GS.Instance.selected_agent.blackboard.GetKeysByVariableType(_target_property.variable_type);
                    //Get global blackboard variables keys with the same type
                    string[] global_keys = GlobalBlackboard_GS.blackboard.GetKeysByVariableType(_target_property.variable_type);
                    
                    //Generate paths
                    _B_variable_dropdown.paths = new string[local_keys.Length + global_keys.Length];

                    //Add the local keys with a prefix for the dropdown
                    for (int k = 0; k < local_keys.Length; k++)
                    {
                        _B_variable_dropdown.paths[k] = "Local/" + local_keys[k];
                    }
                    //Add the global keys with a prefix for the dropdown
                    for (int k = local_keys.Length, i = 0; k < _B_variable_dropdown.paths.Length; k++, i++)
                    {
                        _B_variable_dropdown.paths[k] = "Global/" + global_keys[i];
                    }

                    //Search and set the already selected B key index
                    //Iterate avaliable B keys
                    for (int k = 0; k < _B_variable_dropdown.paths.Length; k++)
                    {
                        if (string.Compare(_B_variable_dropdown.paths[k], _target_property.B_key) == 0)
                        {
                            //When key is found save index and break the iteration
                            ProTools.SetDropdownIndex(_B_variable_dropdown.dropdown_slot, k);
                            _B_variable_dropdown.selected_index = k;
                            break;
                        }
                    }
                }
            }

            //A key label
            GUILayout.Label(_target_property.A_key, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(200.0f), GUILayout.ExpandWidth(true));

            //Operator label
            GUILayout.Label(" " + _target_property.operator_type.ToShortString() + " ", GUILayout.MaxWidth(30.0f), GUILayout.ExpandWidth(true));

            //B value label
            if(string.IsNullOrEmpty(_target_property.B_key))
            {
                //Value case
                GUILayout.Label(_target_property.display_value, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(200.0f), GUILayout.ExpandWidth(true));
            }
            else
            {
                //Variable case
                GUILayout.Label(_target_property.B_key, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(200.0f), GUILayout.ExpandWidth(true));
            }

            //Delete button(hide on play)
            if (!Application.isPlaying)
            {
                if (GUILayout.Button(new GUIContent("X", "Remove"), GUILayout.MaxWidth(25.0f)))
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
                                //Add remove the current property from target action node effects
                                SecurityAcceptMenu_GS.on_accept_delegate += () => _target_action_node_editor.target_action_node.RemoveEffect(_target_property);
                                //Add remove current property editor from target acton node editor effects editors
                                SecurityAcceptMenu_GS.on_accept_delegate += () => _target_action_node_editor.RemoveEffectEditor(this);
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
            if (GUILayout.Button("-", GUILayout.Width(30), GUILayout.Height(20), GUILayout.ExpandWidth(false)) || Application.isPlaying)
            {
                //Change UI mode
                _UI_mode = EditorUIMode.SET_STATE;
                //Set input focus to null
                GUI.FocusControl("null");
                //Free dropdown slots
                ProTools.FreeDropdownSlot(_operator_dropdown.dropdown_slot);
                ProTools.FreeDropdownSlot(_B_variable_dropdown.dropdown_slot);
                _operator_dropdown = null;
                _B_variable_dropdown = null;
                //Check B key
                if(edit_value)
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
            GUILayout.Label(_target_property.A_key, UIConfig_GS.center_normal_style, GUILayout.MaxWidth(200.0f), GUILayout.ExpandWidth(true));

            //Operator dropdown
            ProTools.GenerateButtonDropdownMenu(ref _operator_dropdown.selected_index, _operator_dropdown.paths,_operator_dropdown.paths[_operator_dropdown.selected_index], true, 30.0f, _operator_dropdown.dropdown_slot);
            //Check operator change
            if (_operator_dropdown.selected_index != -1 && _valid_operators[_operator_dropdown.selected_index] != _target_property.operator_type)
            {
                _target_property.operator_type = _valid_operators[_operator_dropdown.selected_index];
            }

            //Value area
            //First check if the target property uses a value or a variable
            if(edit_value)
            {
                //Generate UI field from type
                ProTools.ValueFieldByVariableType(_target_property.variable_type, ref value);
                
                //Change to variable button
                if(GUILayout.Button(new GUIContent(" ","Change to variable"),GUILayout.MaxWidth(10.0f)))
                {
                    edit_value = false;
                }
            }
            else
            {
                //Generate enumerator popup with the avaliable B keys
                ProTools.GenerateButtonDropdownMenu(ref _B_variable_dropdown.selected_index, _B_variable_dropdown.paths, "Not Set", _B_variable_dropdown.selected_index != -1, 80.0f, _B_variable_dropdown.dropdown_slot);
                //Check B key change
                if (_B_variable_dropdown.selected_index != -1 && string.Compare(_target_property.B_key, _B_variable_dropdown.paths[_B_variable_dropdown.selected_index]) != 0)
                {
                    //Update B key on change
                    _target_property.B_key = _B_variable_dropdown.paths[_B_variable_dropdown.selected_index];
                    //Set value as B variable
                    string[] B_key_info = _target_property.B_key.Split('/');
                    if (string.Compare(B_key_info[0], "Global") == 0)
                    {
                        _target_property.value = GlobalBlackboard_GS.blackboard.GetObjectVariable(B_key_info[1]);
                    }
                    else
                    {
                        _target_property.value = NodeEditor_GS.Instance.selected_agent.blackboard.GetObjectVariable(B_key_info[1]);
                    }
                }

                //Change to value button
                if (GUILayout.Button(new GUIContent(" ", "Change to value"), GUILayout.MaxWidth(10.0f)))
                {
                    edit_value = true;
                    _target_property.B_key = null;
                    ProTools.AllocateFromVariableType(_target_property.variable_type, ref value);
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
