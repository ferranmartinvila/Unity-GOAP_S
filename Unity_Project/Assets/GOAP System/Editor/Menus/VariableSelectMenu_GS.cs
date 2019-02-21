using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace GOAP_S.UI
{
    public class VariableSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private NodeEditor_GS _target_node_editor = null; //Node editor where this window is shown
        static private string _variable_name;
        PT.VariableType _variable_type = PT.VariableType._undefined;

        //static private System.Enum _variable_type = GOAP_S.PT.NodeUIMode;
        //Content fields 
        private ArrayList listed_elements = new ArrayList();

        Vector2 scrollPos = new Vector2(0, 0);
        string t = "This is a string inside a Scroll view!";
        int selected_item = 0;

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
            _variable_type = (PT.VariableType)EditorGUILayout.EnumPopup(_variable_type,GUILayout.Width(150), GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            //Custom field value
            GUILayout.BeginVertical();
            switch(_variable_type)
            {
                case PT.VariableType._short:
                    {
                        GUILayout.BeginHorizontal();
                        short short_var = 0;
                        //short_var = EditorGUILayout.IntField(short_var)
                        GUILayout.EndHorizontal();
                    }
                    break;
                case PT.VariableType._int:
                    {

                    }
                    break;
                case PT.VariableType._long:
                    {

                    }
                    break;
                case PT.VariableType._float:
                    {

                    }
                    break;
            }
            GUILayout.EndVertical();

            //Test scroll
            GUILayout.BeginVertical();
            /*scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
            foreach(string str in listed_elements)
            {
                GUILayout.Label(str, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            GUILayout.EndScrollView();*/

            //Test
            if (listed_elements.Count > 0)
            {
                string[] stockArr = new string[listed_elements.Count - 1];
                stockArr = (string[])listed_elements.ToArray(typeof(string));
                selected_item = EditorGUILayout.Popup(selected_item, stockArr);
            }
            
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Asm test", GUILayout.Width(100), GUILayout.Height(10), GUILayout.ExpandHeight(true)))
            {
                Assembly[] all_asm = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly asm in all_asm)
                {
                    System.Type[] asm_types = asm.GetTypes();
                    foreach (System.Type type in asm_types)
                    {
                        if (type.IsClass && type.IsPublic)
                        {
                            listed_elements.Add(typeof(Vector2).BaseType.ToString());
                            if (type.BaseType == typeof(Vector2).BaseType)
                            {
                                string[] result = type.ToString().Split('.');
                                //if(result.Length > 3 && result[2] == "Component")
                                listed_elements.Add(result[1]);
                            }
                        }
                    }
                }
                Debug.Log(listed_elements.Count);
            }
            

            if (GUILayout.Button("Clear",GUILayout.ExpandHeight(true)))
            {
                t = "";
            }
            GUILayout.EndHorizontal();

            
           GUILayout.FlexibleSpace(); 

            GUILayout.BeginHorizontal();
            //Add button
            if (GUILayout.Button("Add", _target_node_editor.UI_configuration.node_modify_button_style, GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true)))
            {
                //Add the new variable if the data is correct

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
        /*public override Vector2 GetWindowSize()
        {

        }*/

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
