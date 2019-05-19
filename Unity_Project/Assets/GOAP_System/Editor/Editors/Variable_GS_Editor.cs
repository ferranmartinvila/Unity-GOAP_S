using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;
using GOAP_S.Tools;
using System.Reflection;
using System.Collections.Generic;

namespace GOAP_S.UI
{
    public class Variable_GS_Editor
    {
        //Content fields
        private  Variable_GS _target_variable = null;
        private Blackboard_GS _target_blackboard = null;
        //State fields
        private EditorUIMode _UI_mode = EditorUIMode.SET_STATE;
        private string new_name = null;
        //Bind fields
        private ProTools.DropDownData_GS _variable_dropdown_data = null;
        private ProTools.DropDownData_GS _method_dropdown_data = null;
        private bool _has_method_parameters = false; //If the variable is binded with a method without input parameters, the input selection menu is blocked

        //Constructors ====================
        public Variable_GS_Editor(Variable_GS new_variable, Blackboard_GS new_bb)
        {
            //Set target variable
            _target_variable = new_variable;
            //Set target bb
            _target_blackboard = new_bb;
            //Initialize dropdowns data
            _variable_dropdown_data = new ProTools.DropDownData_GS();
            _method_dropdown_data = new ProTools.DropDownData_GS();
            //Check for binded method parameters
            if (string.IsNullOrEmpty(_target_variable.binded_method_path) == false)
            {
                _has_method_parameters = ProTools.FindMethodFromPath(_target_variable.binded_method_path, _target_variable.system_type, _target_blackboard.target_agent.gameObject).Key.GetParameters().Length > 0;
            }
        }

        //Loop Methods ====================
        public void DrawUI()
        {
            //Draw variable in edit mode
            if (_UI_mode == EditorUIMode.EDIT_STATE)
            {
                DrawInEditMode();
            }
            //Draw variable in show mode
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
                    //Set input focus to null
                    GUI.FocusControl("null");
                    //Set new name string
                    new_name = _target_variable.name;

                    //Generate bind options
                    //Generate variables paths and display paths

                    //Get all the properties in the agent gameobject
                    PropertyInfo[] _properties_info = ProTools.FindConcretePropertiesInGameObject(NodeEditor_GS.Instance.selected_agent.gameObject, _target_variable.system_type);
                    //Get all fields in th agent gameobject
                    FieldInfo[] _fields_info = ProTools.FindConcreteFieldsInGameObject(NodeEditor_GS.Instance.selected_agent.gameObject, _target_variable.system_type);

                    //Allocate strings arrays
                    _variable_dropdown_data.paths = new string[_properties_info.Length + _fields_info.Length];
                    _variable_dropdown_data.display_paths = new string[_properties_info.Length + _fields_info.Length];
                    //Generate properties paths
                    for (int k = 0; k < _properties_info.Length; k++)
                    {
                        _variable_dropdown_data.paths[k] = string.Format("{0}.{1}", _properties_info[k].ReflectedType.FullName, _properties_info[k].Name);
                        _variable_dropdown_data.display_paths[k] = _variable_dropdown_data.paths[k].Replace('.', '/');
                    }
                    //Generate fields paths
                    for (int k = _properties_info.Length, fields_k = 0; k < _properties_info.Length + _fields_info.Length; k++, fields_k++)
                    {
                        _variable_dropdown_data.paths[k] = string.Format("{0}.{1}", _fields_info[fields_k].ReflectedType.FullName, _fields_info[fields_k].Name);
                        _variable_dropdown_data.display_paths[k] = _variable_dropdown_data.paths[k].Replace('.', '/');
                    }

                    //Get dropdown slot
                    if (_variable_dropdown_data.dropdown_slot == -2)
                    {
                        _variable_dropdown_data.dropdown_slot = ProTools.GetDropdownSlot();
                    }

                    //Generate methods paths and display paths

                    //Get all the methods in the agent gameobject
                    KeyValuePair<MethodInfo, object>[] gameobject_data = ProTools.FindConcreteGameObjectMethods(NodeEditor_GS.Instance.selected_agent.gameObject, _target_variable.system_type);

                    //Get all the methods in the agent logic(actions, behaviour)
                    KeyValuePair<MethodInfo, object>[] agent_data = ProTools.FindConcreteAgentMethods(NodeEditor_GS.Instance.selected_agent, _target_variable.system_type);

                    //Allocate strings arrays
                    _method_dropdown_data.paths = new string[gameobject_data.Length + agent_data.Length];
                    _method_dropdown_data.display_paths = new string[gameobject_data.Length + agent_data.Length];

                    //Generate gameobject methods
                    for (int k = 0; k < gameobject_data.Length; k++)
                    {
                        _method_dropdown_data.paths[k] = string.Format("{0}.{1}.{2}", "GameObject", gameobject_data[k].Key.ReflectedType.FullName, gameobject_data[k].Key.Name);
                        _method_dropdown_data.display_paths[k] = _method_dropdown_data.paths[k].Replace('.', '/');
                    }

                    //Generate agent methods
                    for (int k = gameobject_data.Length, agent_k = 0; k < gameobject_data.Length + agent_data.Length; k++, agent_k++)
                    {
                        _method_dropdown_data.paths[k] = string.Format("{0}.{1}.{2}", "Agent", agent_data[agent_k].Key.ReflectedType.FullName, agent_data[agent_k].Key.Name);
                        _method_dropdown_data.display_paths[k] = _method_dropdown_data.paths[k].Replace('.', '/');
                    }

                    //Get dropdown slot
                    if (_method_dropdown_data.dropdown_slot == -2)
                    {
                        _method_dropdown_data.dropdown_slot = ProTools.GetDropdownSlot();
                    }
                }
            }

            //Show variable type
            GUILayout.Label(_target_variable.type.ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(60.0f));

            //Show variable name
            GUILayout.Label(_target_variable.name, GUILayout.MaxWidth(110.0f));

            //Show variable value
            //Non binded variable case
            if (_target_variable.is_field_binded)
            {
                //If the variables is field binded we show the field inheritance path
                GUILayout.Label(_target_variable.binded_field_short_path, GUILayout.MaxWidth(90.0f));
            }
            else if(_target_variable.is_method_binded)
            {
                if (_has_method_parameters)
                {
                    //If the variables is method binded we show the field inheritance path
                    if (GUILayout.Button(_target_variable.binded_method_short_path, GUILayout.MaxWidth(90.0f)))
                    {
                        Vector2 mousePos = Event.current.mousePosition;
                        PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new MethodSelectMenu_GS(_target_variable));
                    }
                }
                else
                {
                    GUILayout.Label(_target_variable.binded_method_short_path, GUILayout.MaxWidth(90.0f));
                }
            }
            //Binded variable case
            else
            {
                //Get variable value
                object value = _target_variable.object_value;
                //Generate UI field from type
                ProTools.ValueFieldByVariableType(_target_variable.type, ref value);
                //If value is different we update the variable value
                if (value.Equals(_target_variable.object_value) == false)
                {
                    _target_variable.object_value = value;
                    _target_variable.value = value;
                };
            }

            //Free space margin
            GUILayout.FlexibleSpace();

            //Remove button
            if (!Application.isPlaying)
            {
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    bool global = _target_blackboard.target_agent == null;

                    //Add remove the current var method to accept menu delegates callback
                    SecurityAcceptMenu_GS.on_accept_delegate += () => _target_blackboard.RemoveVariable(_target_variable.name, global);
                    //Add remove current var editor from blackboard editor to accept menu delegates calback
                    if (_target_blackboard == GlobalBlackboard_GS.blackboard)
                    {
                        SecurityAcceptMenu_GS.on_accept_delegate += () => GlobalBlackboard_GS_Editor.blackboard_editor.RemoveVariableEditor(_target_variable.name);
                    }
                    else
                    {
                        SecurityAcceptMenu_GS.on_accept_delegate += () => NodeEditor_GS.Instance.blackboard_editor.RemoveVariableEditor(_target_variable.name);
                    }
                    //Get mouse current position
                    Vector2 mousePos = Event.current.mousePosition;
                    //Open security accept menu on mouse position
                    PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new SecurityAcceptMenu_GS());
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DrawInEditMode()
        {
            GUILayout.BeginHorizontal();

            //Edit button, swap between edit and show state
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20)) || Application.isPlaying)
            {
                //Change state
                _UI_mode = EditorUIMode.SET_STATE;
                //Set input focus to null
                GUI.FocusControl("null");
                //Check name change
                if(string.Compare(new_name,_target_variable.name) != 0)
                {
                    //Repeated name case
                    if (_target_blackboard.variables.ContainsKey(new_name))
                    {
                        Debug.LogWarning("The name '" + new_name + "' already exists!");
                    }
                    //New name case
                    else
                    {
                        //Change key in the dictionary, the variable is now stored with the new key
                        _target_blackboard.variables.RenameKey(_target_variable.name, new_name);
                        //Change variable name
                        _target_variable.name = new_name;
                    }
                }
                //Free dropdown slots
                ProTools.FreeDropdownSlot(_variable_dropdown_data.dropdown_slot);
                ProTools.FreeDropdownSlot(_method_dropdown_data.dropdown_slot);

                return;
            }

            //Show variable type
            GUILayout.Label(_target_variable.type.ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(60.0f));

            //Edit variable name
            new_name = EditorGUILayout.TextField(new_name, GUILayout.MaxWidth(80.0f));

            //Show variable value
            //Non binded variable case
            if (!_target_variable.is_field_binded && !_target_variable.is_method_binded)
            {
                //Get variable value
                object value = _target_variable.object_value;
                //Generate UI field from type
                ProTools.ValueFieldByVariableType(_target_variable.type, ref value);
                //If value is different we update the variable value
                if (value.Equals(_target_variable.object_value) == false)
                {
                    _target_variable.object_value = value;
                    _target_variable.value = value;
                }
            }

            ShowBindOptions();

            GUILayout.EndHorizontal();
        }

        private void ShowBindOptions()
        {
            //Bind variable
            GUILayout.BeginHorizontal();

            if (!_target_variable.is_field_binded && !_target_variable.is_method_binded)
            {
                //Free space margin
                GUILayout.FlexibleSpace();

                //Generate variable bind selection dropdown
                ProTools.GenerateButtonDropdownMenu(ref _variable_dropdown_data.selected_index, _variable_dropdown_data.display_paths, "V", false, 40.0f, _variable_dropdown_data.dropdown_slot);

                //Generate method bind selection dropdown
                ProTools.GenerateButtonDropdownMenu(ref _method_dropdown_data.selected_index, _method_dropdown_data.display_paths, "M", false, 40.0f, _method_dropdown_data.dropdown_slot);

                if (_variable_dropdown_data.selected_index > -1)
                {
                    //Bind variable to the new field using the selected path
                    EditorApplication.delayCall += () => _target_variable.BindField(_variable_dropdown_data.paths[_variable_dropdown_data.selected_index]);
                }
                else if (_method_dropdown_data.selected_index > -1)
                {
                    //Bind the variable to the new method using the selected path
                    EditorApplication.delayCall += () => _target_variable.BindMethod(_method_dropdown_data.paths[_method_dropdown_data.selected_index]);
                    EditorApplication.delayCall += () => _has_method_parameters = ProTools.FindMethodFromPath(_target_variable.binded_method_path, _target_variable.system_type, _target_blackboard.target_agent.gameObject).Key.GetParameters().Length > 0;
                }
            }
            else
            {
                //Show current variable bind
                if (target_variable.is_field_binded)
                {
                    GUILayout.Label(_target_variable.binded_field_short_path, GUILayout.MaxWidth(90.0f));
                }
                //Show current method bind
                else
                {
                    if (_has_method_parameters)
                    {
                        //If the variables is method binded we show the field inheritance path
                        if (GUILayout.Button(_target_variable.binded_method_short_path, GUILayout.MaxWidth(90.0f)))
                        {
                            Vector2 mousePos = Event.current.mousePosition;
                            PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0), new MethodSelectMenu_GS(_target_variable));
                        }
                    }
                    else
                    {
                        GUILayout.Label(_target_variable.binded_method_short_path, GUILayout.MaxWidth(90.0f));
                    }
                }

                //Free space margin
                GUILayout.FlexibleSpace();

                //UnBind button
                if (GUILayout.Button("UnBind", GUILayout.MaxWidth(50.0f)))
                {
                    if (_target_variable.is_field_binded)
                    {
                        //Unbind varaible resetting  getter/setter and field path
                        _target_variable.UnbindField();
                        //Reset path index in the variables dropdown
                        ProTools.SetDropdownIndex(_variable_dropdown_data.dropdown_slot, -1);
                    }
                    else
                    {
                        //Unbind method rersetting getter and method info
                        _target_variable.UnbindMethod();
                        _has_method_parameters = false;
                        //Reset path index in the methods dropdown
                        ProTools.SetDropdownIndex(_method_dropdown_data.dropdown_slot, -1);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        //Get/Set Methods =================
        public Variable_GS target_variable
        {
            get
            {
                return _target_variable;
            }
            set
            {
                _target_variable = value;
            }
        }

        public Blackboard_GS target_blackboard
        {
            get
            {
                return _target_blackboard;
            }
            set
            {
                _target_blackboard = value;
            }
        }
    }
}
