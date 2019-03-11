using System.Collections;
using UnityEngine;
using UnityEditor;
using GOAP_S.PT;
using GOAP_S.Blackboard;
using System.Linq;
using System.Reflection;

namespace GOAP_S.UI
{
    public class VariableSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private string _variable_name = null; //New variable name
        VariableType _variable_type = VariableType._undefined; //New variable type
        static private object _variable_value = null; //New variable value
        //Bind properties
        private int _selected_property_index = -1;
        private PropertyInfo[] _properties_info;
        private string[] _properties_paths;

        //Contructors =================
        public VariableSelectMenu_GS()
        {

        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Variable Select", UIConfig_GS.Instance.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Variable name field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.ExpandWidth(true));
            _variable_name = EditorGUILayout.TextField(_variable_name, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            //Variable type field
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.ExpandWidth(true));
            VariableType current_type = _variable_type;
            _variable_type = (VariableType)EditorGUILayout.EnumPopup(_variable_type,GUILayout.Width(150), GUILayout.ExpandWidth(true));
            //Check if var type is changed
            if(current_type != _variable_type)
            {
                AllocateValue();
            }
            GUILayout.EndHorizontal();

            //Custom field value
            if (_variable_type != VariableType._undefined)
            {
                GUILayout.BeginVertical();

                //Show variable value or info label if the variable is binded
                if (_selected_property_index != -1)
                {
                    GUILayout.Label("Value=" + bind_selected_property_display_path, UIConfig_GS.Instance.node_elements_style,GUILayout.ExpandWidth(true));
                }
                else
                {
                    //Define horizontal area
                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(150.0f));

                    //Value label
                    GUILayout.Label("Value", GUILayout.MaxWidth(40.0f));

                    //Generate an input field adapted to the type of the variable
                    switch (_variable_type)
                    {
                        case VariableType._bool:
                            {
                                //Value field
                                _variable_value = GUILayout.Toggle((bool)_variable_value,"");
                            }
                            break;
                        case VariableType._int:
                            {
                                //Value field
                                _variable_value = EditorGUILayout.IntField("", (int)_variable_value, GUILayout.MaxWidth(100.0f));
                            }
                            break;
                        case VariableType._float:
                            {
                                //Value field
                                _variable_value = EditorGUILayout.FloatField("", (float)_variable_value, GUILayout.MaxWidth(100.0f));
                            }
                            break;
                        case VariableType._char:
                            {
                                //Value field
                                _variable_value = EditorGUILayout.TextField("", (string)_variable_value, GUILayout.MaxWidth(40.0f));
                                //Limit value to one char
                                if (!string.IsNullOrEmpty((string)_variable_value))
                                {
                                    _variable_value = ((string)_variable_value).Substring(0, 1);
                                }
                            }
                            break;
                        case VariableType._string:
                            {
                                //Value field
                                _variable_value = EditorGUILayout.TextField("", (string)_variable_value, GUILayout.MaxWidth(120.0f));
                            }
                            break;
                        case VariableType._vector2:
                            {
                                //Value field
                                _variable_value = EditorGUILayout.Vector2Field("", (Vector2)_variable_value, GUILayout.MaxWidth(90.0f));
                            }
                            break;
                        case VariableType._vector3:
                            {
                                //Value field
                                _variable_value = EditorGUILayout.Vector3Field("", (Vector3)_variable_value, GUILayout.MaxWidth(110.0f));
                            }
                            break;
                        case VariableType._vector4:
                            {
                                //Value field
                                _variable_value = EditorGUILayout.Vector4Field("", (Vector4)_variable_value, GUILayout.MaxWidth(150.0f));
                            }
                            break;
                    }
                    GUILayout.EndHorizontal();
                }

                //Bind variable
                GUILayout.BeginHorizontal();
                //Show bind selection dropdown
                if (GUILayout.Button("Bind"))
                {
                    //Get all the fields in the agent gameobject
                    _properties_info = ProTools.FindConcretePropertiesInGameObject(NodeEditor_GS.Instance.selected_agent.gameObject,ProTools.VariableTypeToSystemType(_variable_type));
                    //Get all the paths of the fields found
                    _properties_paths = new string[_properties_info.Length];
                    int index = 0;
                    foreach (PropertyInfo field_info in _properties_info)
                    {
                        //_properties_paths[index] = _properties_info[index].DeclaringType + "/" + _properties_info[index].PropertyType + "/" +  _properties_info[index].Name;// _properties_info[index].Name;
                        _properties_paths[index] = string.Format("{0}.{1}", field_info.ReflectedType.FullName, field_info.Name);
                       // _properties_paths[index] = _properties_paths[index].Replace('.', '/');
                        index += 1;
                    }

                    GenericMenu dropdown = new GenericMenu();
                    for (int k = 0; k < _properties_paths.Length; k++)
                    {
                        //First adapt path to the dropdown format
                        string ui_adapted_path = _properties_paths[k].Replace('.', '/');

                        dropdown.AddItem(
                            //Generate gui content from property path strin
                            new GUIContent(ui_adapted_path),
                            //show the currently selected item as selected
                            k == _selected_property_index,
                            //lambda to set the selected item to the one being clicked
                            selectedIndex => _selected_property_index = (int)selectedIndex,
                            //index of this menu item, passed on to the lambda when pressed.
                            k
                       );
                    }
                    dropdown.ShowAsContext(); //finally show the dropdown
                }
                //UnBind button
                if(GUILayout.Button("UnBind"))
                {
                    _selected_property_index = -1;
                }
                GUILayout.EndHorizontal();

                //Show selected bind path
                if(_selected_property_index == -1)GUILayout.Label(bind_selected_property_display_path);

                GUILayout.EndVertical();
            }



            //Separation
            GUILayout.FlexibleSpace(); 

            GUILayout.BeginHorizontal();
            //Add button
            if (GUILayout.Button("Add", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true)))
            {
                //Add the new variable if the data is correct
                if (!string.IsNullOrEmpty(_variable_name) && _variable_type != VariableType._undefined && _variable_value != null)
                {
                    //Send info to the bb to generate the variable
                    Variable_GS new_variable = NodeEditor_GS.Instance.selected_agent.blackboard.AddVariable(_variable_name, _variable_type, _variable_value);

                    //If add var return null is because the name is invalid(exists a variable with the same name)
                    if (new_variable != null)
                    {
                        //Check if var have to be bind
                        if (_selected_property_index != -1)
                        {
                            //Bind new variable
                            new_variable.BindField(bind_selected_property_path, null);
                        }
                        //Send the new variable to the blackboard editor to generate the variable editor
                        NodeEditor_GS.Instance.blackboard_editor.AddVariableEditor(new_variable);
                        //Close this popup and updat bb window
                        editorWindow.Close();
                        NodeEditor_GS.Instance.Repaint();
                    }
                    else
                    {
                        //Add a rep prefix to the variable name, so the user can see that the name is repeated
                        _variable_name = "Rep:" + _variable_name;
                    }
                }
            }
            //Close button
            if (GUILayout.Button("Close", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
            {
                editorWindow.Close();
            }
            GUILayout.EndHorizontal();
        }

        public override void OnClose()
        {
            _variable_name = "";
        }

        //Functionality Methods =======
        private void AllocateValue()
        {
            //Here we basically allocate diferent elements depending of the variable type and set the allocated field to the variable value
            switch(_variable_type)
            {
                case VariableType._undefined:
                    {
                        _variable_value = null;
                    }
                    break;
                case VariableType._bool:
                    {
                        bool new_bool = false;
                        _variable_value = new_bool;
                    }
                    break;
                case VariableType._int:
                    {
                        int new_int = 0;
                        _variable_value = new_int;
                    }
                    break;
                case VariableType._float:
                    {
                        float new_float = 0.0f;
                        _variable_value = new_float;
                    }
                    break;
                case VariableType._char:
                    {
                        string new_char = "";
                        _variable_value = new_char;
                    }
                    break;
                case VariableType._string:
                    {
                        string new_string = "";
                        _variable_value = new_string;
                    }
                    break;
                case VariableType._vector2:
                    {
                        Vector2 new_vector2 = new Vector2(0.0f, 0.0f);
                        _variable_value = new_vector2;
                    }
                    break;
                case VariableType._vector3:
                    {
                        Vector3 new_vector3 = new Vector3(0.0f, 0.0f, 0.0f);
                        _variable_value = new_vector3;
                    }
                    break;
                case VariableType._vector4:
                    {
                        Vector4 new_vector4 = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                        _variable_value = new_vector4;
                    }
                    break;
            }
        }

        //Get/Set methods =============
        private string bind_selected_property_path
        {
            get
            {
                if (_selected_property_index == -1 || _selected_property_index > _properties_paths.Length - 1)
                {
                    return "Property bind not set";
                }
                else
                {
                    return _properties_paths[_selected_property_index];
                }
            }
        }

        private string bind_selected_property_display_path
        {
            get
            {
                if (_selected_property_index == -1 || _selected_property_index > _properties_paths.Length - 1)
                {
                    return "Property bind not set";
                }
                else
                {
                    string[] parts = bind_selected_property_path.Split('.');
                    return (parts[parts.Length - 2] + "." + parts.Last());
                }
            }
        }


        public int GetAllClassInstances(System.Type type, out ArrayList elements)
        {
            //Allocate a new array
            elements = new ArrayList();
            //Get the root gameobjects of the current scene
            GameObject[] root_gameojects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            //Iterate root gameobjects
            foreach(GameObject g_obj in root_gameojects)
            {
                //Get all the child components with the specified type
                Component[] childs_components = g_obj.GetComponentsInChildren(type);
                foreach(Component comp in childs_components)
                {
                    //Add the found components to the array
                    elements.Add(comp);
                }
            }

            return elements.Count;
        }
    }
}
