using UnityEngine;
using UnityEditor;
using GOAP_S.Blackboard;
using GOAP_S.Tools;
using System.Reflection;

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
        private int _selected_index = -1;
        private string[] _paths;
        private string[] _display_paths;
        private int _bind_dropdown_slot = -1;

        //Constructors ====================
        public Variable_GS_Editor(Variable_GS new_variable, Blackboard_GS new_bb)
        {
            //Set target variable
            _target_variable = new_variable;
            //Set target bb
            _target_blackboard = new_bb;
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
                    //First check if are already generated
                    if (_paths == null)
                    {
                        //Get all the properties in the agent gameobject
                        PropertyInfo[] _properties_info = ProTools.FindConcretePropertiesInGameObject(NodeEditor_GS.Instance.selected_agent.gameObject, _target_variable.system_type);
                        //Get all fields in th agent gameobject
                        FieldInfo[] _fields_info = ProTools.FindConcreteFieldsInGameObject(NodeEditor_GS.Instance.selected_agent.gameObject, _target_variable.system_type);

                        //Allocate strings arrays
                        _paths = new string[_properties_info.Length + _fields_info.Length];
                        _display_paths = new string[_properties_info.Length + _fields_info.Length];
                        //Generate properties paths
                        for (int k = 0; k < _properties_info.Length; k++)
                        {
                            _paths[k] = string.Format("{0}.{1}", _properties_info[k].ReflectedType.FullName, _properties_info[k].Name);
                            _display_paths[k] = _paths[k].Replace('.', '/');
                        }
                        //Generate fields paths
                        int fields_k = 0; //Index to iterate fields info array
                        for (int k = _properties_info.Length; k < _properties_info.Length + _fields_info.Length; k++)
                        {
                            _paths[k] = string.Format("{0}.{1}", _fields_info[fields_k].ReflectedType.FullName, _fields_info[fields_k].Name);
                            _display_paths[k] = _paths[k].Replace('.', '/');
                            fields_k += 1;//Update fields index
                        }
                    }

                    //Get dropdown slot
                    _bind_dropdown_slot = ProTools.GetDropdownSlot();
                }
            }

            //Show variable type
            GUILayout.Label(_target_variable.type.ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(60.0f));

            //Show variable name
            GUILayout.Label(_target_variable.name, GUILayout.MaxWidth(80.0f));

            //Show variable value
            //Non binded variable case
            if (!_target_variable.is_binded)
            {
                //Get variable value
                object value = _target_variable.object_value;
                //Generate UI field from type
                ProTools.ValueFieldByVariableType(_target_variable.type, ref value);
                //If value is different we update the variable value
                if (value != _target_variable.object_value)
                {
                    _target_variable.object_value = value;
                };
            }
            //Binded variable case
            else
            {
                //If variables is binded we show the field inheritance path
                GUILayout.Label(_target_variable.display_field_short_path, GUILayout.MaxWidth(90.0f));
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
                //Free dropdown slot
                ProTools.FreeDropdownSlot(_bind_dropdown_slot);

                return;
            }

            //Show variable type
            GUILayout.Label(_target_variable.type.ToShortString(), UIConfig_GS.left_bold_style, GUILayout.MaxWidth(60.0f));

            //Edit variable name
            new_name = EditorGUILayout.TextField(new_name, GUILayout.MaxWidth(80.0f));

            //Show variable value
            //Non binded variable case
            if (!_target_variable.is_binded)
            {
                //Get variable value
                object value = _target_variable.object_value;
                //Generate UI field from type
                ProTools.ValueFieldByVariableType(_target_variable.type, ref value);
                //If value is different we update the variable value
                if (value != _target_variable.object_value)
                {
                    _target_variable.object_value = value;
                }
            }

            ShowBindOptions();
            

            GUILayout.EndHorizontal();
        }

        private void ShowBindOptions()
        {
            //Bind variable
            GUILayout.BeginHorizontal();

            if (!_target_variable.is_binded)
            {
                //Free space margin
                GUILayout.FlexibleSpace();

                //Generate bind selection dropdown
                ProTools.GenerateButtonDropdownMenu(ref _selected_index, _display_paths, "Bind", false, 40.0f, _bind_dropdown_slot);

                //Check if the variable has been binded, so a path index has been set
                if (!target_variable.is_binded && _selected_index != -1)
                {
                    //Bind variable to the new field using the selected path
                    EditorApplication.delayCall += () => _target_variable.BindField(_paths[_selected_index], null);
                }
            }
            else
            {
                //Show current bind
                GUILayout.Label(_target_variable.display_field_short_path,GUILayout.MaxWidth(90.0f));

                //Free space margin
                GUILayout.FlexibleSpace();

                //UnBind button
                if (GUILayout.Button("UnBind",GUILayout.MaxWidth(50.0f)))
                {
                    //Unbind varaible resetting  getter/setter and field path
                    _target_variable.UnbindField();
                    //Reset path index in the dropdown
                    _selected_index = -1;
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
