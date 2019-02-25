using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;
using GOAP_S.PT;
using GOAP_S.Blackboard;

namespace GOAP_S.UI
{
    public class VariableSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private NodeEditor_GS _target_node_editor = null; //Node editor where this window is shown
        static private string _variable_name = null; //New variable name
        VariableType _variable_type = VariableType._undefined; //New variable type
        static private object _variable_value = null; //New variable value

        //Contructors =================
        public VariableSelectMenu_GS(NodeEditor_GS target_node_editor)
        {
            //Set the node editor
            _target_node_editor = target_node_editor;
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Variable Select", _target_node_editor.UI_configuration.select_menu_title_style, GUILayout.ExpandWidth(true));
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
            GUILayout.BeginVertical();
            switch(_variable_type)
            {
                case VariableType._int:
                    {
                        _variable_value = EditorGUILayout.IntField("Value", (int)_variable_value);
                    }
                    break;
                case VariableType._float:
                    {
                        _variable_value = EditorGUILayout.FloatField("Value", (float)_variable_value);
                    }
                    break;
            }

            //Bind variable
            if (GUILayout.Button("Bind"))
            {
                //EditorGUILayout.Popup()
            }

            /*GenericMenu dropdown = new GenericMenu();

            dropdown.AddItem(new GUIContent("hola"),true,1,)

            dropdown.ShowAsContext();*/

            GUILayout.EndVertical();



            //Separation
            GUILayout.FlexibleSpace(); 

            GUILayout.BeginHorizontal();
            //Add button
            if (GUILayout.Button("Add", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true)))
            {
                //Add the new variable if the data is correct
                if (!string.IsNullOrEmpty(_variable_name) && _variable_type != VariableType._undefined && _variable_value != null)
                {
                    //Send info to the bb to generate the variable
                    Variable_GS new_variable =_target_node_editor.selected_agent.blackboard.AddVariable(_variable_name, _variable_type, _variable_value);
                    //Send the new variable to the blackboard editor to generate the variable editor
                    _target_node_editor.blackboard_editor.AddVariableEditor(new_variable);
                    //Mark scene dirty
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    //Close this popup and updat bb window
                    editorWindow.Close();
                    _target_node_editor.Repaint();
                }
            }
            //Close button
            if (GUILayout.Button("Close", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
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
            switch(_variable_type)
            {
                case VariableType._undefined:
                    {
                        _variable_value = null;
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
