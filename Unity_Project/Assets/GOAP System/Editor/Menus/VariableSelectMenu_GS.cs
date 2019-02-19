using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GOAP_S.UI
{
    public class VariableSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private NodeEditor_GS _target_node_editor = null; //Node editor where this window is shown
        static private string _variable_name;
        //static private System.Enum _variable_type = GOAP_S.PT.NodeUIMode;
        //Content fields 
        private ArrayList listed_elements;

        Vector2 scrollPos = new Vector2(0, 0);
        string t = "This is a string inside a Scroll view!";


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
            _variable_name = EditorGUILayout.TextField("Name",_variable_name,GUILayout.Width(120), GUILayout.ExpandWidth(true));
            //Variable type field
            // _variable_type = EditorGUILayout.EnumFlagsField("Type",_variable_type);
            //Variable value field

            GUILayout.BeginHorizontal();
            //Add button
            if(GUILayout.Button("Add",_target_node_editor.UI_configuration.node_modify_button_style,GUILayout.ExpandWidth(true)))
            {
                //Add the new variable if the data is correct

            }
            //Close button
            if (GUILayout.Button("Close", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.ExpandWidth(true)))
            {
                editorWindow.Close();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.Label(t);
            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("Add More Text", GUILayout.Width(100), GUILayout.Height(100)))
                t += " \nAnd this is more text!";
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Clear"))
                t = "";
        }

        public override void OnClose()
        {
            _variable_name = "";
        }

        //Functionality Methods =======
        public Variable_GS GenerateVariable()
        {
            return null;
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
