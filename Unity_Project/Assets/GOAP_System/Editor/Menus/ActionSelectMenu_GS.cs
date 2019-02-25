using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using GOAP_S.PT;

namespace GOAP_S.UI
{
    public class ActionSelectMenu_GS : PopupWindowContent
    {
        //Content fields
        static private ActionNode_GS_Editor _target_action_node = null; //Focused node action
        static private NodeEditor_GS _target_node_editor = null; //Focused node editor
        static private Dictionary<string, UnityEngine.Object> all_action_scripts = new Dictionary<string, UnityEngine.Object>(); //Action scripts dic

        //Constructors ================
        public ActionSelectMenu_GS(ActionNode_GS_Editor target_action_node, NodeEditor_GS target_node_editor)
        {
            //Focus the action node
            _target_action_node = target_action_node;
            //Focus the node editor
            _target_node_editor = target_node_editor;
            //List all action scripts
            ListActionScripts();
        }

        //Loop Methods ================
        public override void OnGUI(Rect rect)
        {
            //Menu title
            GUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            GUILayout.Label("Action Select",_target_node_editor.UI_configuration.select_menu_title_style, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //Separator
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //Show all the action scripts listed
            GUILayout.BeginVertical();
            foreach (KeyValuePair<string, UnityEngine.Object> script in all_action_scripts)
            {
                if (GUILayout.Button(script.Value.name, GUILayout.Height(25), GUILayout.ExpandWidth(true))) 
                {
                    //Allocate a class with the same type of script value
                    Action_GS new_script = ProTools.AllocateClass<Action_GS>(script.Value);
                    //Set the class name to the new allocated action
                    new_script.name = script.Key;
                    //Set the allocated class to the action node
                    _target_action_node.SetAction(new_script);
                    //Close the pop window when the action is selected & set
                    editorWindow.Close();
                }
                GUILayout.Space(2);
            }
            GUILayout.EndHorizontal();
        }

        //Functionality Methods =======
        public static void ListActionScripts()
        {
            //Clear the action scripts dic to place the current action scripts
            all_action_scripts.Clear();

            //A list of all the object assets imported to the project
            List<MonoScript> object_assets = ProTools.FindAssetsByType<MonoScript>();

            //The action attribute that we use to identify if a class inherit from action class       
            object action_attribute = typeof(Action_GS).GetCustomAttributes(false)[0];

            //Iterate the assets list
            foreach (MonoScript script in object_assets)
            {
                //Get class type
                System.Type class_ty = ((MonoScript)script).GetClass();
                if (class_ty == null) continue;

                //Array with the class custom attributes
                object[] script_attributes = class_ty.GetCustomAttributes(true);

                //Iterate the script attributes
                foreach (object script_attribute in script_attributes)
                {
                    //If there's the action attribute the script inherit from action script
                    if (script_attribute.Equals(action_attribute) && class_ty != typeof(Action_GS))
                    {
                        //Add the script to the action scripts dic
                        all_action_scripts.Add(script.name, script);
                    }
                }
            }
        }
    }
}
