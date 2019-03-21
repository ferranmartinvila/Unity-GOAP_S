using System.Collections;
using UnityEngine;
using UnityEditor;
using GOAP_S.Tools;
using GOAP_S.Blackboard;
using System.Linq;
using System.Reflection;

namespace GOAP_S.UI
{
    public class VariableSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        private string _variable_name = null; //New variable name
        VariableType _variable_type = VariableType._undefined_var_type; //New variable type
        static private object _variable_value = null; //New variable value
        //Bind properties/fields
        private int _selected_index = -1;
        private string[] _paths;
        private string[] _display_paths;
        private int _bind_dropdown_slot = -1;

        //Contructors =================
        public VariableSelectMenu_GS()
        {
            //Get dropdown slots
            _bind_dropdown_slot = ProTools.GetDropdownSlot();
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
                //Allocate value from variable type
                ProTools.AllocateFromVariableType(_variable_type, ref _variable_value);

                //Get all the properties in the agent gameobject
                PropertyInfo[] _properties_info = ProTools.FindConcretePropertiesInGameObject(NodeEditor_GS.Instance.selected_agent.gameObject, _variable_type.ToSystemType());
                //Get all fields in th agent gameobject
                FieldInfo[] _fields_info = ProTools.FindConcreteFieldsInGameObject(NodeEditor_GS.Instance.selected_agent.gameObject, _variable_type.ToSystemType());

                //Allocate strings arrays
                _paths = new string[_properties_info.Length + _fields_info.Length];
                _display_paths = new string[_properties_info.Length + _fields_info.Length];
                //Generate properties paths
                for(int k = 0; k < _properties_info.Length; k++)
                {
                    _paths[k] = string.Format("{0}.{1}", _properties_info[k].ReflectedType.FullName, _properties_info[k].Name);
                    _display_paths[k] = _paths[k].Replace('.', '/');
                }
                //Generate fields paths
                int fields_k = 0; //Index to iterate fields info array
                for(int k = _properties_info.Length; k < _properties_info.Length + _fields_info.Length; k++)
                {
                    _paths[k] = string.Format("{0}.{1}", _fields_info[fields_k].ReflectedType.FullName, _fields_info[fields_k].Name);
                    _display_paths[k] = _paths[k].Replace('.', '/');
                    fields_k += 1;//Update fields index
                }
            }
            GUILayout.EndHorizontal();

            //Custom field value
            if (_variable_type != VariableType._undefined_var_type)
            {
                GUILayout.BeginVertical();

                //Show variable value or info label if the variable is binded
                if (_selected_index != -1)
                {
                    GUILayout.Label("Value=" + bind_selected_display_path, UIConfig_GS.Instance.node_elements_style, GUILayout.ExpandWidth(true));
                }
                else
                {
                    //Define horizontal area
                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(150.0f));

                    //Value label
                    GUILayout.Label("Value", GUILayout.MaxWidth(40.0f));

                    //Generate an input field adapted to the type of the variable
                    ProTools.ValueFieldByVariableType(_variable_type, ref _variable_value);

                    GUILayout.EndHorizontal();
                }

                //Bind variable
                GUILayout.BeginHorizontal();
                //Generate bind selection dropdown
                ProTools.GenerateButtonDropdownMenu(ref _selected_index, _display_paths, "Bind", false, 90.0f, 0);
                //UnBind button
                if (GUILayout.Button("UnBind"))
                {
                    _selected_index = -1;
                    //Reset dropdowns tool
                    ProTools.ResetDropdowns();
                }
                GUILayout.EndHorizontal();

                //Show selected bind path
                if (_selected_index == -1) GUILayout.Label(bind_selected_display_path);

                GUILayout.EndVertical();
            }



            //Separation
            GUILayout.FlexibleSpace(); 

            GUILayout.BeginHorizontal();
            //Add button
            if (GUILayout.Button("Add", UIConfig_GS.Instance.node_modify_button_style, GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true)))
            {
                //Add the new variable if the data is correct
                if (!string.IsNullOrEmpty(_variable_name) && _variable_type != VariableType._undefined_var_type && _variable_value != null)
                {
                    //Send info to the bb to generate the variable
                    Variable_GS new_variable = NodeEditor_GS.Instance.selected_agent.blackboard.AddVariable(_variable_name, _variable_type, _variable_value);

                    //If add var return null is because the name is invalid(exists a variable with the same name)
                    if (new_variable != null)
                    {
                        //Check if var have to be bind
                        if (_selected_index != -1)
                        {
                            //Bind new variable
                            new_variable.BindField(_paths[_selected_index], null);
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
            //Free dropdown slots
            ProTools.FreeDropdownSlot(_bind_dropdown_slot);
        }

        //Get/Set methods =============
        private string bind_selected_display_path
        {
            get
            {
                if (_selected_index == -1 || _selected_index > _paths.Length - 1)
                {
                    return "Property bind not set";
                }
                else
                {
                    string[] parts = _paths[_selected_index].Split('.');
                    return ("." + parts.Last());
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
